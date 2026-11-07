#!/usr/bin/env python3
"""Compare a fresh shadow-workload BDN run against a baseline (#166).

Usage:
    python3 compare.py <results-directory> <baseline> [options]

<results-directory> is BDN's `BenchmarkDotNet.Artifacts/results/`. The
`--exporters json` full exporter writes one `*-report-full-compressed.json`
per benchmark CLASS, so this script globs and merges all of them.

<baseline> is either:
  * another BDN results directory: the same scenarios run against the
    baseline release in the same job (shadow.yaml sets the MSBuild property
    ShadowLibraryVersion=<version> as an environment variable, so BDN's own
    child build picks it up too). Same machine, same run, so latency
    is comparable and gated by default; or
  * a JSON file such as docs/shadow-baseline.json
    ({"scenarios": {name: {meanNanoseconds, allocatedBytes}}}), captured on
    another machine. Latency is advisory by default.

Options (each one can also come from an environment variable; the command
line wins):
  --latency-tolerance F     fractional latency regression allowed
                            (env SHADOW_LATENCY_TOLERANCE, default 0.20)
  --allocation-tolerance F  fractional allocation regression allowed
                            (env SHADOW_ALLOCATION_TOLERANCE, default 0.50)
  --latency-mode MODE       gate | advisory (env SHADOW_LATENCY_MODE;
                            default gate for a results-directory baseline,
                            advisory for a JSON baseline)
  --report PATH             also write the table as markdown (for an issue)

A scenario that allocated nothing in the baseline and allocates now is an
allocation regression whatever the tolerance.

Exit codes: 0 = every gated metric within tolerance, 1 = regression (or no
benchmarks found), 2 = usage error.
"""

from __future__ import annotations

import argparse
import glob
import json
import math
import os
import sys

DEFAULT_LATENCY_TOLERANCE = 0.20
DEFAULT_ALLOCATION_TOLERANCE = 0.50


def load_bdn_reports(results_directory: str) -> dict[str, dict[str, float]]:
    results: dict[str, dict[str, float]] = {}
    report_paths = sorted(glob.glob(os.path.join(results_directory, "*-report-full-compressed.json")))

    for path in report_paths:
        with open(path, encoding="utf-8") as f:
            data = json.load(f)

        for benchmark in data.get("Benchmarks", []):
            name = benchmark["Method"]
            stats = benchmark.get("Statistics", {}) or {}
            memory = benchmark.get("Memory", {}) or {}
            results[name] = {
                "meanNanoseconds": stats.get("Mean", 0.0),
                "allocatedBytes": memory.get("BytesAllocatedPerOperation", 0),
            }

    return results


def load_baseline(path: str) -> dict[str, dict[str, float]]:
    if os.path.isdir(path):
        return load_bdn_reports(path)
    with open(path, encoding="utf-8") as f:
        data = json.load(f)
    return data.get("scenarios", {})


def relative_delta(current: float, base: float) -> float:
    if base:
        return (current - base) / base
    return math.inf if current > 0 else 0.0


def format_delta(delta: float) -> str:
    return "from 0" if math.isinf(delta) else f"{delta:+.1%}"


def env_float(name: str, default: float) -> float:
    raw = os.environ.get(name, "").strip()
    return float(raw) if raw else default


def parse_args(argv: list[str]) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Compare shadow workloads against a baseline.")
    parser.add_argument("results_directory")
    parser.add_argument("baseline")
    parser.add_argument(
        "--latency-tolerance",
        type=float,
        default=env_float("SHADOW_LATENCY_TOLERANCE", DEFAULT_LATENCY_TOLERANCE),
    )
    parser.add_argument(
        "--allocation-tolerance",
        type=float,
        default=env_float("SHADOW_ALLOCATION_TOLERANCE", DEFAULT_ALLOCATION_TOLERANCE),
    )
    parser.add_argument(
        "--latency-mode",
        choices=("gate", "advisory"),
        default=os.environ.get("SHADOW_LATENCY_MODE", "").strip() or None,
    )
    parser.add_argument("--report")
    args = parser.parse_args(argv)
    if args.latency_mode is None:
        args.latency_mode = "gate" if os.path.isdir(args.baseline) else "advisory"
    return args


def main(argv: list[str]) -> int:
    args = parse_args(argv)
    fresh = load_bdn_reports(args.results_directory)
    baseline = load_baseline(args.baseline)

    if not fresh:
        print(f"::error::No benchmarks found under {args.results_directory} - the run likely failed silently.")
        return 1
    if not baseline:
        print(f"::error::No baseline scenarios found in {args.baseline}.")
        return 1

    gate_latency = args.latency_mode == "gate"
    failed: list[str] = []
    rows: list[str] = []

    thresholds = (
        f"latency {args.latency_tolerance:.0%} ({args.latency_mode}), "
        f"allocation {args.allocation_tolerance:.0%} (gate)"
    )
    print(f"Thresholds: {thresholds}")
    print(
        f"{'Scenario':<45} {'Alloc (B)':>12} {'Baseline':>12} {'Delta':>8}  "
        f"{'Latency (ns)':>14} {'Baseline':>14} {'Delta':>8}"
    )
    print("-" * 120)

    for name in sorted(set(fresh) | set(baseline)):
        current = fresh.get(name)
        base = baseline.get(name)
        if current is None:
            print(f"{name:<45} (in the baseline only - removed or renamed, not gated)")
            rows.append(f"| `{name}` | _removed_ | | | | | |")
            continue
        if base is None:
            print(f"{name:<45} (no baseline entry - informational only, not gated)")
            rows.append(
                f"| `{name}` | {current['allocatedBytes']:.0f} | _new_ | | "
                f"{current['meanNanoseconds']:.0f} | _new_ | |"
            )
            continue

        alloc_delta = relative_delta(current["allocatedBytes"], base["allocatedBytes"])
        lat_delta = relative_delta(current["meanNanoseconds"], base["meanNanoseconds"])
        alloc_bad = alloc_delta > args.allocation_tolerance
        lat_bad = lat_delta > args.latency_tolerance

        print(
            f"{name:<45} {current['allocatedBytes']:>12.0f} {base['allocatedBytes']:>12.0f} "
            f"{format_delta(alloc_delta):>7}{'*' if alloc_bad else ' '} "
            f"{current['meanNanoseconds']:>14.0f} {base['meanNanoseconds']:>14.0f} "
            f"{format_delta(lat_delta):>7}{'*' if lat_bad else ' '}"
        )

        latency_flag = ""
        if lat_bad:
            latency_flag = " **FAIL**" if gate_latency else " (warn)"
        rows.append(
            f"| `{name}` | {current['allocatedBytes']:.0f} | {base['allocatedBytes']:.0f} | "
            f"{format_delta(alloc_delta)}{' **FAIL**' if alloc_bad else ''} | "
            f"{current['meanNanoseconds']:.0f} | {base['meanNanoseconds']:.0f} | "
            f"{format_delta(lat_delta)}{latency_flag} |"
        )

        if alloc_bad:
            failed.append(
                f"{name}: allocation {format_delta(alloc_delta)} "
                f"({base['allocatedBytes']:.0f} B -> {current['allocatedBytes']:.0f} B), "
                f"tolerance {args.allocation_tolerance:.0%}"
            )
        if lat_bad and gate_latency:
            failed.append(
                f"{name}: latency {format_delta(lat_delta)} "
                f"({base['meanNanoseconds']:.0f} ns -> {current['meanNanoseconds']:.0f} ns), "
                f"tolerance {args.latency_tolerance:.0%}"
            )
        elif lat_bad:
            print(f"::warning::{name}: latency regressed {format_delta(lat_delta)} (advisory, not gated)")

    if args.report:
        lines = [
            f"Thresholds: {thresholds}.",
            "",
            "| Scenario | Alloc (B) | Baseline | Alloc delta | Latency (ns) | Baseline | Latency delta |",
            "|---|---:|---:|---:|---:|---:|---:|",
            *rows,
        ]
        with open(args.report, "w", encoding="utf-8") as f:
            f.write("\n".join(lines) + "\n")

    if failed:
        print("\n::error::Shadow workload regression(s) beyond tolerance:")
        for line in failed:
            print(f"  - {line}")
        return 1

    print("\nAll gated metrics within tolerance.")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))

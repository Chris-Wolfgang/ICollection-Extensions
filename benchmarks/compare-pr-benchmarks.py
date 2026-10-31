#!/usr/bin/env python3
"""Compare a PR-head BenchmarkDotNet run against a PR-base run and render
a markdown delta table for a PR comment (#186).

Usage:
    python3 compare-pr-benchmarks.py <head-report.json> <base-report.json> <output.md>

Thresholds (fractions; configurable through the environment so the
workflow, or a maintainer re-running it, can tune them per metric):
    PERF_TIME_TOLERANCE   time regression allowed   (default 0.20 = 20 % slower)
    PERF_ALLOC_TOLERANCE  allocation growth allowed (default 0.50 = 50 % more)

Exit codes: 0 = every benchmark within both tolerances,
3 = at least one benchmark's time or allocations regressed beyond its
tolerance, anything else (e.g. 2 for usage errors, or an uncaught
exception) = crash. Both metrics gate. A benchmark that allocated nothing
on the base and allocates on the head is an allocation regression whatever
the tolerance. The caller (pr-benchmarks.yaml) treats exit 3 as waived
when the PR carries the `perf-impact-acknowledged` label; this script
only computes and reports, it doesn't know about labels.
"""

from __future__ import annotations

import json
import math
import os
import sys

DEFAULT_TIME_TOLERANCE = 0.20  # 20% slower
DEFAULT_ALLOC_TOLERANCE = 0.50  # 50% more allocated


def tolerance(name: str, default: float) -> float:
    raw = os.environ.get(name, "").strip()
    return float(raw) if raw else default


def load(path: str) -> dict[str, dict[str, float]]:
    with open(path, encoding="utf-8") as f:
        data = json.load(f)

    results = {}
    for benchmark in data.get("Benchmarks", []):
        name = benchmark.get("FullName") or benchmark.get("Method") or "?"
        stats = benchmark.get("Statistics", {}) or {}
        memory = benchmark.get("Memory", {}) or {}
        results[name] = {
            "meanNanoseconds": stats.get("Mean", 0.0),
            "allocatedBytes": memory.get("BytesAllocatedPerOperation", 0),
        }
    return results


def relative_delta(head: float, base: float) -> float:
    if base:
        return (head - base) / base
    return math.inf if head > 0 else 0.0


def format_delta(delta: float) -> str:
    return "from 0" if math.isinf(delta) else f"{delta:+.1%}"


def format_ns(ns: float) -> str:
    if ns >= 1_000_000:
        return f"{ns / 1_000_000:.2f} ms"
    if ns >= 1_000:
        return f"{ns / 1_000:.2f} us"
    return f"{ns:.1f} ns"


def main() -> int:
    if len(sys.argv) != 4:
        print(f"Usage: {sys.argv[0]} <head-report.json> <base-report.json> <output.md>", file=sys.stderr)
        return 2

    time_tolerance = tolerance("PERF_TIME_TOLERANCE", DEFAULT_TIME_TOLERANCE)
    alloc_tolerance = tolerance("PERF_ALLOC_TOLERANCE", DEFAULT_ALLOC_TOLERANCE)

    head_path, base_path, output_path = sys.argv[1], sys.argv[2], sys.argv[3]
    head = load(head_path)
    base = load(base_path)

    rows = []
    any_regression = False

    for name in sorted(set(head) | set(base)):
        h = head.get(name)
        b = base.get(name)

        if h is None:
            # Benchmark exists only in the base run. The table's alloc column
            # is head-side, so show the base time in the base column and em
            # dashes for the head-side cells rather than misfiling base data.
            rows.append(f"| `{name}` | _removed_ | {format_ns(b['meanNanoseconds'])} | — | — | — |")
            continue
        if b is None:
            rows.append(f"| `{name}` | {format_ns(h['meanNanoseconds'])} | _new_ | — | {h['allocatedBytes']:.0f} B | — |")
            continue

        time_delta = relative_delta(h["meanNanoseconds"], b["meanNanoseconds"])
        alloc_delta = relative_delta(h["allocatedBytes"], b["allocatedBytes"])

        time_regressed = time_delta > time_tolerance
        alloc_regressed = alloc_delta > alloc_tolerance
        if time_regressed or alloc_regressed:
            any_regression = True

        rows.append(
            f"| `{name}` | {format_ns(h['meanNanoseconds'])} | {format_ns(b['meanNanoseconds'])} | "
            f"{format_delta(time_delta)}{' 🔴' if time_regressed else ''} | {h['allocatedBytes']:.0f} B | "
            f"{format_delta(alloc_delta)}{' 🔴' if alloc_regressed else ''} |"
        )

    lines = [
        "<!-- pr-benchmarks-delta -->",
        "## Benchmark delta vs base branch",
        "",
        "| Benchmark | Head time | Base time | Time Δ | Head alloc | Alloc Δ |",
        "|---|---:|---:|---:|---:|---:|",
        *rows,
        "",
        f"🔴 = time regressed > {time_tolerance:.0%} or allocations grew > {alloc_tolerance:.0%} "
        "(**fails the check** unless this PR carries the `perf-impact-acknowledged` label). "
        "Both runs share one runner and one job, but shared-runner timings are still noisy: "
        "re-run the workflow before acting on a time-only regression.",
    ]

    with open(output_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines) + "\n")

    # Print status only (not the table itself) to stdout — some local
    # Windows consoles are cp1252 and choke on the U+0394/emoji glyphs
    # above; the actual markdown file is UTF-8 and that's what the
    # workflow posts as the PR comment.
    print(
        f"Wrote {output_path}: {'REGRESSION' if any_regression else 'within tolerance'} "
        f"(time {time_tolerance:.0%}, allocations {alloc_tolerance:.0%})"
    )

    # 3 (not 1) so the caller can tell "regression beyond tolerance" apart
    # from a crash: 0 = clean, 3 = regression, other = crash.
    return 3 if any_regression else 0


if __name__ == "__main__":
    sys.exit(main())

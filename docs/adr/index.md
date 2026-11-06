# Architecture Decision Records

| ADR | Title | Status |
|-----|-------|--------|
| [0001](0001-pin-assemblyversion-to-1.0.0.0.md) | Pin AssemblyVersion to a fixed 1.0.0.0 baseline | Accepted |
| [0002](0002-extend-icollection-and-defer-to-the-collection.md) | Extend `ICollection<T>` and defer every semantic to the receiving collection | Accepted |
| [0003](0003-snapshot-items-when-they-alias-the-source.md) | Snapshot `items` when it is the same instance as `source` | Accepted |
| [0004](0004-fast-paths-only-where-semantics-are-identical.md) | Type-specific fast paths only where the semantics are identical | Accepted |
| [0005](0005-banned-symbols-enforce-async-first.md) | `BannedSymbols.txt` enforces the fleet's async-first rules | Accepted |

New ADRs land in the same PR as the decision they record, and are reviewed with it. Copy
[TEMPLATE.md](TEMPLATE.md), take the next number, and add a row here. A replaced decision keeps
its file with the status "Superseded by ADR-NNNN".

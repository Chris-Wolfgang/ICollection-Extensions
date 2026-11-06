# ADR-0002: Extend `ICollection<T>` and defer every semantic to the receiving collection

## Status

Accepted

## Context

Helpers like `AddRange`, `RemoveWhere` or "add if missing" exist on some concrete types
(`List<T>.AddRange`, `HashSet<T>.RemoveWhere`, `ISet<T>.Add`) but not on the interface code
usually holds, so callers end up writing the same loops against `ICollection<T>` over and over.
Three receivers were possible:

- `IList<T>`: excludes `HashSet<T>`, `SortedSet<T>` and `LinkedList<T>`, which are exactly the
  collections that lack these helpers.
- `IEnumerable<T>`: has no `Add`/`Remove`, so the methods could not mutate.
- `ICollection<T>`: the smallest interface with `Add`, `Remove`, `Contains`, `Clear`, `Count`
  and `IsReadOnly`, implemented by every mutable BCL collection and by arrays.

A second question was whose rules apply: the library could impose its own equality, de-dup
and read-only checks, or pass everything through to the collection.

## Decision

Every method extends `ICollection<T>` and uses only that interface's members, so the
collection's own semantics decide the outcome:

- equality and duplicates come from the collection (`Contains`, `Remove`, `ISet<T>.Add`), so a
  `HashSet<T>` with a custom comparer de-duplicates by that comparer;
- read-only receivers throw the collection's own `NotSupportedException` when a mutation is
  attempted; the library adds no `IsReadOnly` pre-check;
- nothing is culture-sensitive, and no method is thread-safe beyond what the collection is.

## Consequences

- One API works across `List<T>`, `HashSet<T>`, `SortedSet<T>`, `LinkedList<T>`,
  `Collection<T>` and arrays, with no surprises relative to calling the collection directly.
- Results can legitimately differ by receiver (`AddRange` on a `HashSet<T>` drops duplicates),
  and the XML docs say so per method.
- A read-only receiver only throws when an item would actually be added or removed (empty
  input returns normally), which is why the `NotSupportedException` docs are phrased that way.
- Faster type-specific paths are allowed only when they keep these semantics; see ADR-0004.

# ADR-0003: Snapshot `items` when it is the same instance as `source`

## Status

Accepted (v0.3.0)

## Context

Several methods enumerate an `items` sequence while mutating `source`. When a caller passes the
same instance for both (`list.AddRange(list)`, `list.RemoveRange(list)`,
`list.ReplaceAll(list)`), the BCL enumerators reject the mutation with
`InvalidOperationException` ("Collection was modified"), and `ReplaceAll` would silently clear
the data before reading it. Callers write these forms on purpose ("append a copy",
"remove everything", "no-op replace"), so throwing or wiping data is the wrong answer.

Copying `items` on every call would make the common, non-aliased case pay for the rare one.

## Decision

`AddRange`, `AddRangeIf`, `RemoveRange` and `ReplaceAll` check `ReferenceEquals(items, source)`
and, only in that case, copy `source` into a new `List<T>` before mutating. The resulting
behaviour is documented on each method: append a copy, append the matching copy, empty the
collection, leave the contents unchanged.

`AddIfNotContains(IEnumerable<T>)` takes no snapshot: when the sequences alias, every item is
already present, so no `Add` happens during enumeration and the call returns 0.

## Consequences

- Self-aliasing calls are well defined and safe for every `ICollection<T>`.
- The non-aliased path stays allocation-free apart from what the operation itself needs.
- Aliasing through a different object (e.g. a wrapper around the same list) is not detected;
  that is the same contract as the BCL.
- Each guard and its documented result is pinned by a unit test (`*_when_items_is_same_instance_as_source_*`).

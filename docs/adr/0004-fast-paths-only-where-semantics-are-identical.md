# ADR-0004: Type-specific fast paths only where the semantics are identical

## Status

Accepted (v0.3.0)

## Context

Some receivers already implement an operation natively: `HashSet<T>.RemoveWhere(Predicate<T>)`
removes in one pass without a temporary list, `ISet<T>.Add` answers "was it added?" in one
lookup instead of `Contains` + `Add`, and `List<T>` has a settable `Capacity` and
`RemoveAll`. Each type check adds a branch to every call, and a fast path that behaves even
slightly differently from the generic path would break ADR-0002.

## Decision

Three fast paths are taken, each only because its observable result equals the generic path:

- `AddRange`: when `source` is a `List<T>` and `items` implements `ICollection<T>`, grow
  `Capacity` once to `Count + items.Count` before adding.
- `RemoveWhere`: when `source` is a `HashSet<T>`, delegate to `HashSet<T>.RemoveWhere`.
- `AddIfNotContains(T)`: when `source` is any `ISet<T>`, delegate to `ISet<T>.Add`.

`List<T>.RemoveAll` is deliberately not special-cased: the generic two-pass path already
handles every other `ICollection<T>` uniformly, and the saving at typical sizes does not pay
for the extra type check on every call.

## Consequences

- The common set and list cases avoid an allocation or a second lookup.
- Each fast path is checked for parity with the generic path: the unit tests compare results
  on both, the fuzz properties (`Tests.Fuzz`, added in #330) compare `List` and `HashSet` receivers against
  reference implementations, and the Stryker disables on the two fast-path blocks state why
  removing them is unobservable.
- Adding another fast path needs the same evidence: identical return value and final
  contents, including duplicates, ordering and custom comparers.

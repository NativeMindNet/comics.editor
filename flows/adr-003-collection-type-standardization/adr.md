# ADR-003: Collection Type Standardization (ObservableCollection vs List)

## Meta

- **Number**: ADR-003
- **Type**: constraining
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The WPF and Unity implementations use different collection types for animations:

| Platform | Collection Type |
|----------|----------------|
| WPF (Comics.Editor) | `ObservableCollection<Anim>` |
| Unity (UnityComicsEditor) | `List<Anim>` |

**Problem**:
- `ObservableCollection<T>` provides `CollectionChanged` events for WPF binding
- `List<T>` is simpler but has no change notification
- Shared code (like `Anim.Add<T>()`) uses `IList<Anim>` interface, but behavior differs
- Serialization outputs the same JSON, but runtime semantics differ

The `Anim` base class factory methods accept `IList<Anim>`:
```csharp
public static T Add<T>(IList<Anim> anims, double scroll) where T : Anim, new()
```

This works for both, but UI binding in WPF relies on the observable nature.

## Decision Drivers

- **Code sharing**: Maximize shared logic between WPF and Unity
- **UI binding**: WPF requires observable collections for live updates
- **Simplicity**: Unity doesn't need observable (IMGUI repaints fully)
- **Serialization**: JSON output must be identical regardless of runtime type

## Considered Options

### Option 1: Keep Platform-Specific (Status Quo)

**Description**: WPF uses ObservableCollection; Unity uses List. Shared code uses IList interface.

**Pros**:
- No changes required
- Each platform uses idiomatic types
- Serialization works (both serialize as JSON arrays)

**Cons**:
- Subtle behavior differences possible
- Shared code must be careful not to assume observable behavior

**Estimated Effort**: None

### Option 2: Standardize on List Everywhere

**Description**: Change WPF to use `List<Anim>` and wrap with `BindingList` or manual refresh.

**Pros**:
- Uniform behavior
- Simpler model layer

**Cons**:
- WPF UI binding breaks without manual refresh
- Significant WPF refactoring
- Goes against WPF idioms

**Estimated Effort**: High

### Option 3: Standardize on ObservableCollection Everywhere

**Description**: Change Unity to use `ObservableCollection<Anim>`.

**Pros**:
- Uniform behavior
- Could enable future Unity UIToolkit binding
- Familiar to .NET developers

**Cons**:
- Unnecessary overhead in Unity (no binding)
- ObservableCollection has no `AddRange()` (minor)

**Estimated Effort**: Low

### Option 4: Custom INotifyingList Interface

**Description**: Create a common interface that both can implement:

```csharp
interface INotifyingList<T> : IList<T>, INotifyCollectionChanged { }
```

**Pros**:
- Abstraction allows any implementation
- Future-proof

**Cons**:
- Overengineered for current needs
- Extra abstraction layer

**Estimated Effort**: Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 1 (Status Quo)** for now. The interface-based approach already works. If Unity adopts UIToolkit with binding, consider **Option 3**.

Key guideline: Shared code should always use `IList<Anim>` and never assume observable behavior.

## Consequences

### Positive

- No immediate work required
- Each platform uses idiomatic types

### Negative

- Must document the difference for contributors
- Risk of subtle bugs if shared code assumes collection behavior

### Neutral

- Serialization unchanged

## Implementation Notes

- Ensure all shared code uses `IList<T>` interface
- Add code comment warning about observable vs non-observable
- If adding UI binding to Unity, revisit this ADR

## Related Decisions

- ADR-001: Serialization (collection type doesn't affect JSON)

## Related Specs

- `flows/sdd-unity-animation-timeline-ui/`: May influence if UIToolkit binding needed

## References

- ObservableCollection: https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1

## Tags

architecture data-model collections

---

## Approval

### Review History

| Date | Reviewer | Status | Comments |
|------|----------|--------|----------|
| - | - | pending | - |

### Final Decision

- [ ] Approved by: -
- [ ] Decided on: -
- [ ] Implementation assigned to: -

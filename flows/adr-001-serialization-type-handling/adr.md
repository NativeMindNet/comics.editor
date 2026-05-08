# ADR-001: Serialization Type Handling for Polymorphic Animations

## Meta

- **Number**: ADR-001
- **Type**: constraining
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The animation system uses polymorphic types (TranslateAnim, RotateAnim, ScaleAnim, AlphaAnim, SoundAnim) that inherit from abstract `Anim` base class. Current serialization uses Newtonsoft.Json with `TypeNameHandling.Auto`, which embeds full .NET type names in JSON:

```json
{
  "$type": "Comics.Editor.Models.TranslateAnim, Comics.Editor",
  "start": 0,
  "end": 200,
  "x": 50,
  "y": 100
}
```

**Problem**: If class names or namespaces change (e.g., during Unity port refactoring), existing documents become unreadable. The type string `Comics.Editor.Models.TranslateAnim, Comics.Editor` is tightly coupled to assembly and namespace.

## Decision Drivers

- **Backward compatibility**: Existing `.comics`/`.puzzle` files must remain readable after codebase refactoring
- **Cross-platform portability**: WPF and Unity use different assembly names
- **Schema evolution**: Future animation types should be addable without breaking existing files
- **Human readability**: JSON should be inspectable without decoding type metadata

## Considered Options

### Option 1: Keep TypeNameHandling.Auto (Status Quo)

**Description**: Continue using Newtonsoft.Json's automatic type embedding.

**Pros**:
- No code changes required
- Works today for both WPF and Unity (same serializer)

**Cons**:
- Namespace/class rename breaks all existing documents
- Assembly name appears in JSON (fragile)
- Not human-readable

**Estimated Effort**: None

### Option 2: Explicit Type Discriminator

**Description**: Add an explicit `animType` enum property to JSON schema:

```json
{
  "animType": "translate",
  "start": 0,
  "end": 200,
  "x": 50,
  "y": 100
}
```

Use a custom JsonConverter to deserialize based on discriminator.

**Pros**:
- Decoupled from .NET type names
- Human-readable
- Easy to extend with new types
- Cross-platform safe

**Cons**:
- Requires migration script for existing files
- Need to maintain discriminator → type mapping
- Breaking change (v2 format)

**Estimated Effort**: Medium

### Option 3: Custom Binder with Alias Mapping

**Description**: Use `SerializationBinder` to map old type names to new types:

```csharp
binder.Map("Comics.Editor.Models.TranslateAnim", typeof(TranslateAnim));
binder.Map("ComicsUnity.Models.TranslateAnim", typeof(TranslateAnim));
```

**Pros**:
- Backward compatible with existing files
- No file migration needed
- Can support multiple legacy names

**Cons**:
- Still embeds type names (grows over time)
- Each rename adds more aliases
- Doesn't solve human readability

**Estimated Effort**: Low

## Decision

**[PENDING DECISION]**

Recommended: **Option 2 (Explicit Type Discriminator)** combined with **Option 3 (Binder for Legacy)** for migration period:

1. Implement explicit discriminator for v2 format (new files)
2. Use custom binder to read legacy v1 files
3. On save, always write v2 format

## Consequences

### Positive

- Future-proof against refactoring
- Human-readable JSON
- Clear schema for documentation

### Negative

- Migration effort required
- Dual read path during transition
- v2 files not readable by legacy WPF editor

### Neutral

- File size slightly smaller (no assembly names)

## Implementation Notes

- Add `schemaVersion` field to document root (see ADR-005)
- Create `AnimTypeConverter : JsonConverter<Anim>` that reads `animType` discriminator
- Register legacy type aliases in `SerializationBinder` for backward compat

## Related Decisions

- ADR-005: Schema Versioning (required for v1/v2 discrimination)

## Related Specs

- `flows/vdd-legacy-format/`: Document format specification
- `flows/sdd-unity-asset-pipeline-fidelity/`: Round-trip validation

## References

- Newtonsoft.Json TypeNameHandling: https://www.newtonsoft.com/json/help/html/SerializeTypeNameHandling.htm
- Custom SerializationBinder: https://www.newtonsoft.com/json/help/html/SerializeSerializationBinder.htm

## Tags

serialization architecture json compatibility

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

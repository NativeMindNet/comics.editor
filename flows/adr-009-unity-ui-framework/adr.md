# ADR-009: Unity UI Framework (IMGUI vs UIToolkit)

## Meta

- **Number**: ADR-009
- **Type**: constraining
- **Status**: DRAFT
- **Created**: 2026-05-08
- **Decided**: -
- **Author**: /legacy analysis
- **Reviewers**: -

## Context

The Unity Comics Editor currently uses **IMGUI** (OnGUI immediate mode):

```csharp
public class ComicsEditorWindow : EditorWindow
{
    private void OnGUI()
    {
        if (GUILayout.Button("New Comics"))
            CreateNew(false);

        _session.Scroll = EditorGUILayout.Slider("Scroll", ...);
        // ... all UI is imperative
    }
}
```

**Problem**:
- IMGUI works but is limited for complex layouts
- Animation timeline requires structured track/segment UI
- WPF uses declarative XAML; IMGUI has no equivalent
- UIToolkit is Unity's modern alternative (declarative, data-binding)

**Question**: Should we continue with IMGUI or migrate to UIToolkit for the animation timeline and inspector?

## Decision Drivers

- **Development speed**: Choose framework that enables faster iteration
- **Unity compatibility**: Must work in Editor context (not runtime)
- **Layout complexity**: Timeline UI needs tracks, segments, handles
- **Team familiarity**: Current codebase uses IMGUI
- **Future-proofing**: UIToolkit is Unity's recommended direction

## Considered Options

### Option 1: Continue with IMGUI

**Description**: Build all new UI (timeline, inspector) using IMGUI.

**Pros**:
- No new framework to learn
- Immediate mode is simple for dynamic content
- All existing code uses it
- No migration needed

**Cons**:
- Layout is manual (no flex, grid)
- Styling is limited
- No data binding
- Hard to maintain complex hierarchies

**Estimated Effort**: Medium (per feature)

### Option 2: Full Migration to UIToolkit

**Description**: Rewrite EditorWindow using UIToolkit (UXML + USS).

**Pros**:
- Modern, declarative, data-binding
- Better layout (flexbox-like)
- Styleable (USS = CSS-like)
- Unity's recommended path

**Cons**:
- Learning curve
- Migration effort for existing UI
- Newer (less community examples)
- Editor API gaps possible

**Estimated Effort**: High (one-time migration)

### Option 3: Hybrid Approach

**Description**: Use UIToolkit for new complex UI (timeline); keep IMGUI for simple parts.

```csharp
public class ComicsEditorWindow : EditorWindow
{
    private VisualElement _root;

    private void CreateGUI()  // UIToolkit entry
    {
        _root = rootVisualElement;
        _root.Add(new TimelineView());  // UIToolkit
    }

    private void OnGUI()  // IMGUI for simple parts
    {
        // Legacy or simple controls
    }
}
```

**Pros**:
- Best of both worlds
- Gradual migration
- Use right tool for each task

**Cons**:
- Two systems to maintain
- Potential style inconsistency
- Integration complexity

**Estimated Effort**: Medium

### Option 4: Third-Party UI Library

**Description**: Use Odin Inspector, Editor Console Pro, or similar.

**Pros**:
- Rich out-of-box components
- Faster development for standard patterns

**Cons**:
- External dependency
- License cost
- May not fit custom timeline needs

**Estimated Effort**: Low to Medium

## Decision

**[PENDING DECISION]**

Recommended: **Option 3 (Hybrid Approach)**

Rationale:
- Timeline UI benefits from UIToolkit's layout system
- Simple toolbar/buttons can stay IMGUI
- Allows gradual migration and learning
- Reduces risk compared to full rewrite

## Consequences

### Positive

- Modern UI framework for complex components
- Gradual migration reduces risk
- Better timeline UX possible

### Negative

- Two frameworks to maintain
- Team needs to learn UIToolkit

### Neutral

- Existing IMGUI code continues to work

## Implementation Notes

- Start with timeline component in UIToolkit
- Create shared data model accessible to both systems
- Use `VisualElement` custom controls for timeline tracks
- Consider `ListView` for layer list (virtual scrolling)

**Example Timeline Structure (UIToolkit)**:

```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
  <ui:VisualElement class="timeline">
    <ui:VisualElement class="tracks">
      <ui:VisualElement class="track" />
      <ui:VisualElement class="track" />
    </ui:VisualElement>
    <ui:VisualElement class="playhead" />
  </ui:VisualElement>
</ui:UXML>
```

## Related Decisions

- (none)

## Related Specs

- `flows/sdd-unity-animation-timeline-ui/`: Timeline implementation
- `flows/sdd-unity-canvas-preview-transforms/`: Preview canvas

## References

- UIToolkit Manual: https://docs.unity3d.com/Manual/UIElements.html
- IMGUI to UIToolkit Migration: https://docs.unity3d.com/Manual/UIE-migration-guides.html

## Tags

ui unity architecture

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

# animation timeline (move/rotate/scale/opacity/sound)

> Client-Facing Documentation  
> Last Updated: 2026-05-08  
> Version: 1.0

## What This Feature Does

This work defines the timeline system that lets you animate layers and sounds over time, and preview the result by playing or scrubbing.

---

## How It Works

**In Simple Terms:**

1. You place “segments” on a timeline (like blocks).
2. Each segment tells the editor what should change during that time (move, rotate, fade, etc.).
3. The playhead shows the current moment in time, and the editor draws the scene for that moment.

---

## Key Benefits

- **Clear timing**: you can see exactly when things happen.
- **Fast preview**: scrub to any point and instantly see the result.
- **Expandable**: the model can grow to support more advanced animation later.

---

## Getting Started

1. Select a layer.
2. Add an animation segment on the timeline.
3. Play or scrub to preview.

---

**Note for Stakeholders**: Technical details and progress are tracked in `05-implementation-log.md`.

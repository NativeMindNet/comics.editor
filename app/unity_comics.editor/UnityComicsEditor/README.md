# Unity Comics Editor

Unity Editor port of the legacy **Comics.Editor** WPF tool (`.comics` / `.puzzle` zip bundles with `data.json`, `layers/`, `sounds/`).

## Requirements

- Unity **2022.3 LTS** (or compatible)
- Package **Newtonsoft JSON** (added via `Packages/manifest.json`)

## Open the project

1. Start Unity Hub → **Add** → select folder  
   `app/unity_comics.editor/UnityComicsEditor`
2. Wait for scripts to compile.
3. Menu: **Window → Comics → Comics Editor**

## Features (v1)

- New Comics / New Puzzle workspace
- Open / Save `.comics` and `.puzzle` via `System.IO.Compression.ZipFile` (no `7za.exe`)
- JSON model compatible with legacy settings (`TypeNameHandling.Auto`, camelCase)
- Image tiling via **Texture2D** + **RenderTexture** (no ImageMagick)
- Layer list, reorder, delete, scroll preview, culture selection, basic translate segment add
- Sound import (copies `mp3` into bundle; playback not wired in this window)

## Original WPF sources

The copied solution under `app/unity_comics.editor/Comics.*` is the original Windows project for reference only.

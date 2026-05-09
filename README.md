# comics.editor

Unity Editor для создания и редактирования интерактивных `.comics` / `.puzzle` документов со scroll-driven анимациями.

## Возможности

| Функция | Статус |
|---------|--------|
| Создание Comics / Puzzle документов | Done |
| Открытие и сохранение архивов | Done |
| Управление слоями (add/delete/reorder) | Done |
| Тайлинг изображений | Done |
| Мультиязычность (En, Ru, Hi) | Done |
| Animation Timeline UI | Done |
| Все типы анимаций (translate, rotate, scale, alpha) | Done |
| Composed preview с трансформациями | Done |
| Undo / Redo (Ctrl+Z / Ctrl+Y) | Done |
| Audio preview при скролле | Done |
| Импорт звуков (mp3) | Done |

## Быстрый старт

### Установка

1. Unity **2022.3 LTS** или новее
2. Откройте Unity Hub → Add → выберите папку:
   ```
   app/unity_comics.editor/UnityComicsEditor
   ```
3. Дождитесь компиляции скриптов
4. Меню: **Window → Comics → Comics Editor**

### Интерфейс

```
┌─────────────────────────────────────────────────────────────────────────┐
│ [New Comics] [New Puzzle] [Open] [Save]  [Undo] [Redo]  [Sound] [Sync]  │
├──────────────────────────────┬──────────────────────────────────────────┤
│ Document                     │  Preview                                 │
│   Width: [1080]              │  ┌────────────────────────────────────┐  │
│   Height: [2160]             │  │                                    │  │
│   Scroll: [━━━○━━━] 1234     │  │      [Composed Preview]            │  │
│   Culture: [En ▼]            │  │                                    │  │
│                              │  └────────────────────────────────────┘  │
│ [+ Layer] [+ Sound]          ├──────────────────────────────────────────┤
│                              │  Timeline                                │
│ Layers:                      │  ┌────────────────────────────────────┐  │
│   [Layer 0        ] [↑↓×]    │  │ Ruler: 0   1000   2000   3000      │  │
│   [Layer 1 (sel)  ] [↑↓×]    │  │ ═══════╠════════╣══════════════    │  │
│   [Layer 2        ] [↑↓×]    │  │        ▲ selected                  │  │
│                              │  └────────────────────────────────────┘  │
│ Sounds:                      ├──────────────────────────────────────────┤
│   [bgm.mp3        ] [↑↓×]    │  Inspector                               │
│                              │  ┌────────────────────────────────────┐  │
│ Add Animation:               │  │ Type: Translate                    │  │
│   [Translate] [Rotate]       │  │ Start: [1000]  End: [2000]         │  │
│   [Scale]    [Alpha]         │  │ X: [100]  Y: [500]                 │  │
│                              │  └────────────────────────────────────┘  │
│ [Delete Selected Anim]       │                                          │
└──────────────────────────────┴──────────────────────────────────────────┘
```

## Workflow

### Создание нового документа

1. Нажмите **New Comics** или **New Puzzle**
2. Добавьте изображения через **+ Layer**
3. Выберите слой → добавьте анимации (Translate, Rotate, Scale, Alpha)
4. Настройте параметры в Inspector
5. Используйте Timeline для визуализации и редактирования
6. Сохраните через **Save**

### Редактирование анимаций

1. Выберите слой в списке Layers
2. Нажмите кнопку типа анимации (Translate / Rotate / Scale / Alpha)
3. В Timeline появится новый сегмент
4. Кликните на сегмент для выбора
5. Редактируйте параметры в Inspector:
   - **Start/End** — диапазон скролла
   - **X, Y** — для Translate
   - **Angle, PivotX, PivotY** — для Rotate
   - **ScaleX, ScaleY, PivotX, PivotY** — для Scale
   - **Alpha** — для прозрачности

### Работа с Timeline

- **Клик** на сегмент — выбор анимации
- **Drag краёв** — изменение Start/End
- **Drag середины** — перемещение всего диапазона
- **Колесо мыши** — zoom
- **Перетаскивание пустой области** — pan

### Keyboard Shortcuts

| Клавиша | Действие |
|---------|----------|
| **Ctrl+Z** / **Cmd+Z** | Undo |
| **Ctrl+Y** / **Ctrl+Shift+Z** | Redo |
| **Delete** / **Backspace** | Удалить выбранную анимацию |

## Undo / Redo

Система поддерживает:

- Изменения диапазонов анимаций (с coalescing за 500ms)
- Добавление / удаление анимаций
- Изменения параметров анимаций
- Изменения изображений слоёв (с backup файлов)

Ограничения:
- История очищается при Save / Open / New
- Максимум 50 операций в истории
- Backup файлы хранятся в `.undo/` директории

## Audio Preview

- **Sound toggle** в toolbar включает/выключает звук
- Звуки воспроизводятся при скролле через их диапазон
- Looping звуки (Start ≠ End) играют пока скролл в диапазоне
- One-shot звуки (Start = End) играют один раз при пересечении точки
- 50ms debounce предотвращает "дребезг" при быстром скролле

## Структура проекта

```
UnityComicsEditor/Assets/ComicsUnity/Editor/
├── ComicsEditorWindow.cs      # Главное окно
├── ComicsEditorSession.cs     # Состояние сессии
├── FileManagerUnity.cs        # Работа с файлами
├── TileGeneratorUnity.cs      # Генерация тайлов
├── ZipUtility.cs              # Работа с ZIP
│
├── Commands/                  # Undo/Redo система
│   ├── IEditCommand.cs        # Интерфейс команды
│   ├── UndoStack.cs           # Стек истории
│   ├── UpdateAnimRangeCommand.cs
│   ├── AddAnimCommand.cs
│   ├── RemoveAnimCommand.cs
│   ├── UpdateAnimParamsCommand.cs
│   └── SetLayerImageCommand.cs
│
├── Audio/                     # Audio preview
│   └── EditorAudioManager.cs  # Менеджер звуков
│
├── Inspector/                 # Инспекторы
│   ├── AnimationInspector.cs  # Редактор анимаций
│   └── LayerInspector.cs      # Редактор слоёв
│
├── Timeline/                  # Timeline компонент
│   └── AnimationTimeline.cs   # Визуализация анимаций
│
├── Preview/                   # Composed preview
│   ├── ComicsPreviewWindow.cs
│   └── ComicsStagePreview.cs
│
└── Models/                    # Модели данных
    ├── ComicsDocument.cs
    ├── LayerModel.cs
    ├── ImageModel.cs
    ├── Anim.cs
    ├── TranslateAnim.cs
    ├── RotateAnim.cs
    ├── ScaleAnim.cs
    ├── AlphaAnim.cs
    ├── SoundAnim.cs
    └── SoundModel.cs
```

## Формат документов

```
my.comics (ZIP)
├── data.json          # Метаданные + анимации
├── layers/
│   ├── image.png      # Простое изображение
│   └── big_1000_0_0.png  # Тайлы: {zoom*1000}_{col}_{row}.png
└── sounds/
    └── bgm.mp3
```

### JSON сериализация

```csharp
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Auto,
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Formatting = Formatting.Indented
};
```

## Связанные документы

### Спецификации (flows/)

| Flow | Описание | Статус |
|------|----------|--------|
| `sdd-comics.editor-animation-timeline-ui` | Timeline UI | Complete |
| `sdd-comics.editor-canvas-preview-transforms` | Composed preview | Complete |
| `sdd-comics.editor-audio-preview` | Audio preview | Complete |
| `sdd-comics.editor-undo-redo` | Undo/Redo система | Complete |
| `sdd-comics.editor-parity-gaps-overview` | Обзор паритета с WPF | Reference |

### Runtime движок

- `app/unity_comics.engine/` — Unity package для просмотра `.comics`
- См. `../comics.engine/README.md` для API и примеров

## WPF Reference

Директория `app/unity_comics.editor/Comics.*` содержит оригинальный WPF проект для справки при портировании.

## Лицензия

Proprietary - NativeMind

# Comics Editor (Unity)

Unity Editor порт оригинального **Comics.Editor** (WPF) для создания и редактирования интерактивных `.comics` / `.puzzle` документов.

## Возможности (v1)

- Создание новых Comics / Puzzle документов
- Открытие и сохранение `.comics` / `.puzzle` архивов
- Управление слоями (добавление, удаление, перемещение)
- Тайлинг изображений через Texture2D + RenderTexture
- Выбор культуры (En, Ru, Hi)
- Базовое редактирование анимаций (translate)
- Импорт звуков (mp3)
- Preview со скроллом

## Требования

- Unity **2022.3 LTS** или совместимая версия
- Пакет **Newtonsoft.Json** (добавлен через `Packages/manifest.json`)

## Установка

1. Откройте Unity Hub → **Add** → выберите папку:
   ```
   app/unity_comics.editor/UnityComicsEditor
   ```
2. Дождитесь компиляции скриптов
3. Меню: **Window → Comics → Comics Editor**

## Интерфейс

```
┌─────────────────────────────────────────────────────────────┐
│  [New Comics] [New Puzzle] [Open] [Save]     Culture: [En▼] │
├─────────────────────────────────────────────────────────────┤
│  Layers:                    │  Preview:                     │
│  ┌───────────────────────┐  │  ┌─────────────────────────┐  │
│  │ Layer 0          [↑↓×]│  │  │                         │  │
│  │ Layer 1          [↑↓×]│  │  │    [Scroll Preview]     │  │
│  │ Layer 2          [↑↓×]│  │  │                         │  │
│  └───────────────────────┘  │  └─────────────────────────┘  │
│  [+ Add Image]              │  Scroll: [━━━━━━━━○━━] 1234   │
├─────────────────────────────┴───────────────────────────────┤
│  Animations:                                                │
│  [+ Translate] (другие типы в разработке)                   │
│  Start: [____] End: [____] X: [____] Y: [____]              │
├─────────────────────────────────────────────────────────────┤
│  Sounds:                                                    │
│  [+ Add Sound]                                              │
│  sound.mp3  Start: [____] End: [____]                       │
└─────────────────────────────────────────────────────────────┘
```

## Структура проекта

```
UnityComicsEditor/
├── Assets/
│   └── ComicsUnity/
│       └── Editor/
│           ├── ComicsEditorWindow.cs    # Главное окно
│           ├── ComicsEditorSession.cs   # Состояние сессии
│           ├── ComicsJson.cs            # Сериализация JSON
│           ├── FileManagerUnity.cs      # Работа с файлами
│           ├── TileGeneratorUnity.cs    # Генерация тайлов
│           ├── ZipUtility.cs            # Работа с ZIP
│           ├── PreviewTextureBuilder.cs # Построение превью
│           └── Models/
│               ├── ComicsDocument.cs    # Модель документа
│               ├── LayerModel.cs        # Модель слоя
│               ├── ImageModel.cs        # Модель изображения
│               ├── Anim.cs              # Базовый класс анимации
│               ├── TranslateAnim.cs     # Анимация позиции
│               ├── RotateAnim.cs        # Анимация вращения
│               ├── ScaleAnim.cs         # Анимация масштаба
│               ├── AlphaAnim.cs         # Анимация прозрачности
│               ├── PivotAnim.cs         # Анимация pivot
│               ├── SoundAnim.cs         # Звуковая анимация
│               ├── SoundModel.cs        # Модель звука
│               └── Cultures.cs          # Перечисление культур
└── Packages/
    └── manifest.json                    # Зависимости
```

## Workflow

### Создание нового документа

1. **Window → Comics → Comics Editor**
2. Нажмите **New Comics** или **New Puzzle**
3. Добавьте изображения через **Add Image**
4. Настройте анимации для каждого слоя
5. Сохраните через **Save**

### Редактирование существующего

1. Нажмите **Open**
2. Выберите `.comics` или `.puzzle` файл
3. Редактируйте слои и анимации
4. Сохраните изменения

### Добавление анимаций

1. Выберите слой в списке
2. Нажмите **+ Translate** (или другой тип)
3. Укажите Start/End scroll позиции
4. Задайте параметры (X, Y для translate)
5. Используйте слайдер Scroll для предпросмотра

## Сравнение с WPF версией

| Функция | WPF | Unity v1 | Статус |
|---------|-----|----------|--------|
| Модель данных | Полная | Полная | 100% |
| ZIP I/O | 7za.exe + ImageMagick | ZipFile + Texture2D | Работает |
| Preview canvas | Composed transforms | Stacked images | 20% |
| Translate anim | Полный редактор | Один button | 10% |
| Rotate/Scale/Alpha | Полные редакторы | Не в UI | 0% |
| Sound playback | MediaPlayer | Только импорт | 0% |
| Undo/Redo | Нет | Нет | Planned |

## Известные ограничения

1. **Preview не показывает трансформации** — слои отображаются без rotate/scale/alpha
2. **Нет редактора для Rotate, Scale, Alpha, Pivot** — только translate
3. **Нет воспроизведения звука** — звуки только копируются в архив
4. **Нет hit-testing** — нельзя выбрать слой кликом на preview

## Планируемые улучшения

### Фаза 0: Engine Integration
- Использование `comics.engine` для preview
- Общий `AnimationProcessor` гарантирует идентичность runtime

### Фаза 1: Canvas Preview
- Composed transforms в реальном времени
- Hit-testing для выбора слоев
- См. `flows/sdd-comics.editor-canvas-preview-transforms/`

### Фаза 2: Animation Timeline UI
- Полные редакторы для всех типов анимаций
- Timeline визуализация
- См. `flows/sdd-comics.editor-animation-timeline-ui/`

### Фаза 3: Audio + Safety
- Воспроизведение звуков в редакторе
- Undo/Redo система
- См. `flows/sdd-comics.editor-audio-preview/`, `flows/sdd-comics.editor-undo-redo/`

## Временные пути

| Платформа | Путь |
|-----------|------|
| WPF | `%LocalAppData%\Comics Editor\Temp` |
| Unity | `Application.temporaryCachePath/ComicsUnityEditor` |

## JSON сериализация

Настройки совместимы с WPF версией:

```csharp
var settings = new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Auto,
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Formatting = Formatting.Indented
};
```

## Связанные документы

### Flows
- Обзор паритета: `flows/sdd-comics.editor-parity-gaps-overview/`
- Canvas preview: `flows/sdd-comics.editor-canvas-preview-transforms/`
- Animation UI: `flows/sdd-comics.editor-animation-timeline-ui/`
- Audio preview: `flows/sdd-comics.editor-audio-preview/`
- Undo/Redo: `flows/sdd-comics.editor-undo-redo/`

### VDD (Visual Design)
- Формат: `flows/vdd-comics.editor-format/`
- Rendering: `flows/vdd-comics.editor-rendering/`
- Animation timeline: `flows/vdd-comics.editor-animation-timeline/`

### ADR
- ADR-006: Transform composition order
- ADR-007: Hit-testing implementation
- ADR-009: Unity UI Framework (IMGUI vs UIToolkit)

## Оригинальные WPF исходники

Директория `app/unity_comics.editor/Comics.*` содержит оригинальный WPF проект:

```
Comics.Core/       # Модели данных (Layer, Anim, Image)
Comics.Editor/     # WPF редактор (MainWindow, ViewModels)
Comics.Web/        # Web API (не используется в Unity)
```

Эти файлы служат справочным материалом для портирования.

## Лицензия

Proprietary - NativeMind

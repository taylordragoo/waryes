**Title:** Architecture — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/architecture](https://v3.onejs.com/docs/core-concepts/architecture)

---

# Page Structure Map
```text
Architecture — OneJS Docs
├── Architecture Overview#
├── The JavaScript Engine#
├── The React Reconciler#
├── Component Mapping#
├── C# Interop#
├── Event System#
├── Scheduling#
├── Memory Management#
├── Application Lifecycle#
└── WebGL Differences#
```

---

OneJS connects JavaScript to Unity through several layers.

## Architecture Overview#

```
┌─────────────────────────────────────────────────┐
│                  Your React Code                │
├─────────────────────────────────────────────────┤
│              onejs-react Reconciler             │
├─────────────────────────────────────────────────┤
│           QuickJS JavaScript Engine             │
├─────────────────────────────────────────────────┤
│     QuickJSUIBridge (Events, Scheduling)        │
├─────────────────────────────────────────────────┤
│              UI Toolkit Elements                │
└─────────────────────────────────────────────────┘
```

## The JavaScript Engine#

OneJS uses **QuickJS**, a small embeddable JavaScript engine. It runs your code in an isolated context with:

-   Full ES2020 support
-   Async/await and Promises
-   Modules (ESM)

QuickJS is interpreted, not JIT-compiled, so it works on iOS and other AOT/IL2CPP platforms where JIT is prohibited. For WebGL builds, OneJS uses the browser's native JavaScript engine instead.

## The React Reconciler#

`onejs-react` is a custom React reconciler. When you write:

```
<View style={{ padding: 20 }}>
    <Label text="Hello" />
</View>
```

The reconciler:

1.  Creates a `VisualElement` for the View
2.  Creates a `Label` element
3.  Sets the style properties
4.  Adds the Label as a child of the View

React's diffing algorithm handles updates efficiently. Only changed properties are updated.

## Component Mapping#

| React Component | UI Toolkit Element |
| --- | --- |
| `<View>` | `VisualElement` |
| `<Text>` | `TextElement` |
| `<Label>` | `Label` |
| `<Button>` | `Button` |
| `<TextField>` | `TextField` |
| `<Toggle>` | `Toggle` |
| `<Slider>` | `Slider` |
| `<ScrollView>` | `ScrollView` |
| `<Image>` | `Image` |
| `<ListView>` | `ListView` |
| `<FrostedGlass>` | `FrostedGlassElement` |
| Raw text | `TextElement` |

**Text handling:** Raw text children (e.g., `<View>Hello</View>`) automatically create `TextElement` instances. Use `<Text>` for primary text display and `<Label>` for form labels.

## C# Interop#

The `CS` global proxy uses reflection to access C# types:

```
CS.UnityEngine.Debug.Log("Hello")
```

This resolves to:

1.  Look up the `UnityEngine.Debug` type
2.  Find the `Log` method
3.  Convert arguments to C# types
4.  Invoke the method
5.  Convert the return value back to JavaScript

Objects returned from C# are wrapped in proxies with integer handles. Property access and method calls go through the interop layer.

## Event System#

UI Toolkit events are captured and routed to JavaScript:

1.  C# registers a TrickleDown callback on the root element
2.  When an event fires, it's serialized to JSON
3.  `__dispatchEvent(handle, eventType, data)` is called
4.  JavaScript looks up handlers and invokes them

```
UI Event → C# Capture → JSON Serialize → JS Dispatch → Your Handler
```

## Scheduling#

OneJS provides familiar timing APIs:

-   `requestAnimationFrame()`: Called every Unity Update
-   `setTimeout()` / `setInterval()`: Timer queue processed each frame
-   `queueMicrotask()`: For Promise resolution

The bridge's `Tick()` method drives all scheduling from Unity's Update loop.

## Memory Management#

C# objects referenced from JavaScript are tracked with integer handles:

1.  C# object → Handle registered in table
2.  JavaScript proxy wraps the handle
3.  When JS object is garbage collected → FinalizationRegistry releases handle
4.  C# object becomes eligible for GC

Manual cleanup is available via `releaseObject()` but rarely needed.

## Application Lifecycle#

OneJS runs your code in two contexts: **edit-mode preview** (renders UI without Play mode) and **play mode**. Export lifecycle hooks to control when game logic runs:

```
Edit-mode preview:
  InitializeBridge → RunScript (module-level code runs) → EditModeTick at 30Hz

Play mode:
  InitializeBridge → RunScript → onPlay()
  → Update every frame
  → File change → onStop() → teardown → rebuild → onPlay()
  → Exit Play mode → onStop()
```

-   **Module-level code** (including `render()`) runs in both modes, safe for UI
-   **`onPlay()`** fires only in play mode, after the bundle loads
-   **`onStop()`** fires before teardown (play mode exit or live reload)
-   **`__isPlaying`** global: `true` in play mode, `false` in edit-mode preview

## WebGL Differences#

On WebGL, JavaScript runs in the browser's V8/SpiderMonkey engine:

-   JIT compilation (faster execution)
-   Browser's native RAF loop
-   No QuickJS overhead

The same code runs on both platforms. Only the underlying engine changes.

---

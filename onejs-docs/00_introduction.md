**Title:** Introduction — OneJS Docs

**Source:** [https://v3.onejs.com/docs/introduction](https://v3.onejs.com/docs/introduction)

---

# Page Structure Map
```text
Introduction — OneJS Docs
├── Two Worlds, One Runtime#
├── For App Developers#
├── For Game Developers#
├── Platform Support#
├── How It Works#
├── Developer Experience#
├── Requirements#
└── Next Steps#
```

---

OneJS lets you build UIs with React and TypeScript that render through Unity's GPU-accelerated graphics pipeline. Write familiar web code, deploy everywhere Unity runs.

## Two Worlds, One Runtime#

OneJS bridges **web development** and **game engines**:

-   Use **React 19**, hooks, JSX, and the npm ecosystem
-   Render through **Unity's UI Toolkit** - GPU-accelerated, not a webview
-   Access **any C# API** directly from JavaScript
-   Deploy to **desktop, mobile, web, consoles, and VR/AR** from one codebase

## For App Developers#

If you're building cross-platform applications, your current options have painful tradeoffs:

| Solution | Problem |
| --- | --- |
| Electron | Ships Chromium (~150MB), high RAM usage, CPU-bound rendering |
| Tauri/WebView | Still browser rendering under the hood |
| Flutter | Different language (Dart), different ecosystem |
| React Native | Mobile-focused, limited desktop support |

**OneJS offers something different:**

-   **True GPU rendering** - UI Toolkit renders on the GPU, not a browser engine
-   **No webview overhead** - No DOM, no layout engine, no Chromium
-   **React you already know** - Hooks, components, JSX, TypeScript
-   **Unity's capabilities** - Mix 2D UI with 3D scenes, use shaders, physics, audio
-   **Smaller builds** - No bundled browser engine

## For Game Developers#

Game UI development has been stuck in the past - verbose C# code, slow iteration, limited modding support.

OneJS brings modern practices to game UI:

-   **Declarative components** - Write UI as React components, not imperative C# code
-   **Hot reload** - See changes instantly without recompiling
-   **Separation of concerns** - UI logic in JavaScript, game logic in C#
-   **First-class modding** - Players can modify UI without touching game code
-   **Web dev accessibility** - Expand your team with JavaScript developers

## Platform Support#

From a single React codebase:

| Platform | Notes |
| --- | --- |
| Windows, macOS, Linux | Desktop builds |
| iOS, Android | Mobile builds |
| WebGL | Browser builds (uses V8/SpiderMonkey with JIT) |
| PlayStation, Xbox, Switch | Console builds |
| VR/AR | XR device builds |

## How It Works#

```
React Components → onejs-react Reconciler → UI Toolkit Elements → GPU Rendering
```

When you write:

```
<Button text="Click me" onClick={() => console.log("clicked")} />
```

OneJS creates a real `UnityEngine.UIElements.Button` and wires up the event handler. React's diffing algorithm ensures only changed properties update.

## Developer Experience#

-   **TypeScript** - Full type definitions for Unity APIs
-   **Hot Reload** - Changes appear instantly during development
-   **CSS Modules** - Scoped styles with `.module.uss` files
-   **Tailwind CSS** - Utility-first styling, automatically escaped for USS
-   **npm Packages** - Use libraries from the JavaScript ecosystem
-   **C# Interop** - Call any Unity API with `CS.UnityEngine.*`
-   **AI-friendly** - Point Cursor/Claude/ChatGPT at /llms-full.txt for full-docs context

## Requirements#

-   Unity 6.3 or later
-   Node.js 18+ (for TypeScript compilation)

## Next Steps#

-   Quick Start - Get OneJS running and build your first UI
-   Comparison - Deep dive into the value proposition

---
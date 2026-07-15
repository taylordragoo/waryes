**Title:** Tailwind CSS — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/tailwind](https://v3.onejs.com/docs/guides/tailwind)

---

# Page Structure Map
```text
Tailwind CSS — OneJS Docs
├── Setup#
├── Basic Usage#
├── JIT-Style Generation#
├── Supported Utilities#
│   ├── Spacing#
│   ├── Colors#
│   ├── Layout#
│   ├── Typography#
│   ├── Borders#
│   ├── Transforms#
│   ├── Transitions#
│   └── Other#
├── Arbitrary Values#
├── Character Escaping#
├── Responsive Design#
│   ├── Breakpoints#
│   └── How It Works#
├── Responsive Hooks#
├── Hover and States#
├── Limitations#
├── Full Example#
├── Hot Reload#
└── esbuild Configuration#
```

---

OneJS includes a built-in Tailwind-like utility class generator. No external dependencies required - just import and use.

**Related:** Styling Basics | CSS Modules

## Setup#

Tailwind is enabled by default in the esbuild config. Just add the import to your code:

```
import "onejs:tailwind"
```

That's it! The plugin scans your source files for class names and generates USS automatically.

## Basic Usage#

Use Tailwind classes directly:

```
import { render, View, Label, Button } from "onejs-react"
import "onejs:tailwind"

function App() {
    return (
        <View className="p-4 bg-gray-900 rounded-lg">
            <Label className="text-white text-lg" text="Hello" />
            <Button className="mt-2 py-2 px-4 bg-blue-500 hover:bg-blue-600" text="Click" />
        </View>
    )
}

render(<App />, __root)
```

## JIT-Style Generation#

Only classes actually used in your source files are included in the bundle. The plugin scans files matching the `content` patterns in your esbuild config:

```
// esbuild.config.mjs
tailwindPlugin({
    content: ["./**/*.{tsx,ts,jsx,js}"]
})
```

Dynamic and conditional classes are fully supported. The scanner extracts string literals from ternaries, logical expressions, and helper functions:

```
// All of these work
<View className={active ? "bg-blue-500" : "bg-gray-500"} />
<View className={clsx("p-4", isActive && "bg-blue-500")} />
<View className={`flex ${size === "lg" ? "px-6" : "px-3"}`} />
```

## Supported Utilities#

### Spacing#

```
// Padding
<View className="p-4 px-6 py-2 pt-4 pr-6 pb-4 pl-6" />

// Margin
<View className="m-4 mx-auto my-2 mt-4 mr-6 mb-4 ml-6" />
```

> **Note**: USS does not support the `gap` property. Use margins on child elements instead.

### Colors#

Full Tailwind color palette: slate, gray, zinc, neutral, stone, red, orange, amber, yellow, lime, green, emerald, teal, cyan, sky, blue, indigo, violet, purple, fuchsia, pink, rose.

```
// Background
<View className="bg-gray-900 bg-blue-500 bg-transparent" />

// Text
<Label className="text-white text-gray-300 text-blue-500" />

// Border
<View className="border border-gray-700 border-red-500" />
```

### Layout#

```
// Flexbox
<View className="flex flex-row flex-col flex-wrap" />
<View className="justify-center justify-between items-center items-start" />
<View className="flex-1 grow shrink-0" />

// Sizing
<View className="w-full w-1/2 w-64 h-full h-screen" />
<View className="min-w-0 max-w-full min-h-0 max-h-full" />

// Position
<View className="relative absolute top-0 right-0 bottom-0 left-0" />
```

### Typography#

```
// Size
<Label className="text-xs text-sm text-base text-lg text-xl text-2xl text-3xl" />

// Weight (maps to -unity-font-style: normal or bold)
<Label className="font-normal font-bold" />

// Alignment
<Label className="text-left text-center text-right" />

// Letter spacing
<Label className="tracking-tight tracking-normal tracking-wide" />
```

> **Note**: USS only supports `-unity-font-style` with `normal`, `bold`, `italic`, and `bold-and-italic`. Classes like `font-thin`, `font-light`, `font-medium` map to `normal`; `font-semibold`, `font-bold`, `font-extrabold`, `font-black` map to `bold`.

### Borders#

```
// Width
<View className="border border-2 border-4 border-t border-r border-b border-l" />

// Colors (all sides or individual)
<View className="border border-gray-500" />
<View className="border-t-2 border-t-blue-500" />
<View className="border-b border-b-red-500" />

// Radius
<View className="rounded rounded-sm rounded-md rounded-lg rounded-xl rounded-full" />
<View className="rounded-t-lg rounded-b-none" />
```

### Transforms#

```
// Rotate
<View className="rotate-45 rotate-90 -rotate-45" />

// Scale
<View className="scale-50 scale-100 scale-125 hover:scale-105" />
<View className="scale-x-75 scale-y-150" />

// Translate
<View className="translate-x-4 translate-y-2 -translate-x-4" />

// Transform origin
<View className="origin-center origin-top-left origin-bottom-right" />
```

### Transitions#

```
// Basic transition
<View className="transition hover:bg-blue-500" />

// With duration and easing
<View className="transition duration-300 ease-in-out hover:scale-105" />

// Specific properties
<View className="transition-colors duration-200" />
<View className="transition-opacity duration-500" />
<View className="transition-transform duration-300" />

// With delay
<View className="transition delay-100 hover:opacity-50" />
```

Duration values: `0`, `75`, `100`, `150`, `200`, `300`, `500`, `700`, `1000` (ms)

### Other#

```
// Opacity
<View className="opacity-0 opacity-50 opacity-100" />

// Display
<View className="hidden flex" />

// Overflow
<View className="overflow-hidden overflow-scroll" />

// Flex basis
<View className="basis-1/2 basis-auto basis-0" />

// Aspect ratio
<View className="aspect-square aspect-video" />
```

## Arbitrary Values#

Use square brackets for one-off values outside the design system:

```
// Custom sizes
<View className="w-[200] h-[150]" />    // 200px, 150px
<View className="w-[50%] h-[25%]" />    // percentages

// Custom colors
<View className="bg-[#ff5733] text-[#333]" />

// Custom spacing
<View className="p-[15] m-[7]" />       // 15px, 7px

// Custom font size
<Label className="text-[22]" text="Custom size" />

// Custom border radius
<View className="rounded-[10]" />       // 10px
```

Rules:

-   Numbers without units default to `px`
-   Use `%` for percentages
-   Use `#` prefix for hex colors
-   No spaces inside brackets

## Character Escaping#

Tailwind classes with special characters are automatically escaped for USS:

| Character | Escaped | Example |
| --- | --- | --- |
| `:` | `_c_` | `hover:bg-red` → `hover_c_bg-red` |
| `/` | `_s_` | `w-1/2` → `w-1_s_2` |
| `.` | `_d_` | `opacity-0.5` → `opacity-0_d_5` |
| `[` `]` | `_lb_` `_rb_` | `w-[100px]` → `w-_lb_100px_rb_` |

Write standard Tailwind in your JSX. The escaping happens automatically at build time.

## Responsive Design#

Wrap your app with `ScreenProvider`:

```
import { render, ScreenProvider } from "onejs-react"
import "onejs:tailwind"

render(
    <ScreenProvider>
        <App />
    </ScreenProvider>,
    __root
)
```

Use responsive prefixes:

```
<View className="p-2 sm:p-4 md:p-6 lg:p-8">
    <Label className="text-sm md:text-base lg:text-lg" text="Responsive" />
</View>
```

### Breakpoints#

| Prefix | Min Width |
| --- | --- |
| `sm:` | 640px |
| `md:` | 768px |
| `lg:` | 1024px |
| `xl:` | 1280px |
| `2xl:` | 1536px |

### How It Works#

USS doesn't support media queries. Instead:

1.  Responsive classes become ancestor-scoped selectors (`.sm .sm_c_p-4`)
2.  ScreenProvider adds breakpoint classes to the root element
3.  At 1400px width, root has classes: `sm md lg xl`

This enables mobile-first CSS cascading.

## Responsive Hooks#

```
import { useBreakpoint, useScreenSize, useResponsive, useMediaQuery } from "onejs-react"

function Component() {
    // Current breakpoint name
    const bp = useBreakpoint() // "base" | "sm" | "md" | "lg" | "xl" | "2xl"

    // Viewport dimensions
    const { width, height } = useScreenSize()

    // All responsive state
    const { isSm, isMd, isLg, isXl, is2xl } = useResponsive()

    // Check specific breakpoint
    const isDesktop = useMediaQuery("lg")

    return (
        <View>
            <Label text={`Breakpoint: ${bp}`} />
            <Label text={`Size: ${width}x${height}`} />
            {isLg && <Label text="Large screen content" />}
        </View>
    )
}
```

## Hover and States#

Pseudo-classes work as expected:

```
<View className="bg-gray-800 hover:bg-gray-700">
    <Button className="bg-blue-500 hover:bg-blue-600 active:bg-blue-700" />
</View>
```

## Limitations#

USS doesn't support everything in Tailwind:

| Not Supported | Alternative |
| --- | --- |
| CSS Grid | Use Flexbox (`flex-row`, `flex-col`) |
| `gap` property | Use margins on children (`mb-4`, `mr-4`) |
| `z-index` | Reorder elements in hierarchy |
| `font-weight` (100-900) | Use `font-normal` or `font-bold` (`-unity-font-style`) |
| `text-transform` | Use rich text tags or C# string methods |
| Shadows | Use background colors or borders |
| Filters | Not available |
| Keyframe animations | Use transitions for simple effects |
| CSS custom properties in utilities | Use USS Variables in a separate `.uss` file |
| `line-height` | Not available in USS |
| `currentColor` | Use explicit colors |
| `:first-child`, `:nth-child` | Not available in USS |

## Full Example#

```
import { useState } from "react"
import { render, View, Label, Button, ScreenProvider, useBreakpoint } from "onejs-react"
import "onejs:tailwind"

function Card({ title, children }) {
    return (
        <View className="p-4 md:p-6 bg-gray-800 rounded-lg">
            <Label className="text-lg text-white mb-2" text={title} />
            <View className="text-gray-400">{children}</View>
        </View>
    )
}

function App() {
    const bp = useBreakpoint()

    return (
        <View className="p-4 md:p-8 lg:p-12 bg-gray-900 flex-1">
            <Label className="text-xs text-gray-500 mb-4" text={`Breakpoint: ${bp}`} />

            <View className="flex-col md:flex-row">
                <View className="mb-4 md:mb-0 md:mr-4">
                    <Card title="Feature 1">
                        <Label text="Description here" />
                    </Card>
                </View>
                <Card title="Feature 2">
                    <Label text="Another feature" />
                </Card>
            </View>

            <Button
                className="mt-4 py-2 px-4 bg-blue-500 hover:bg-blue-600 rounded-lg"
                text="Get Started"
            />
        </View>
    )
}

render(
    <ScreenProvider>
        <App />
    </ScreenProvider>,
    __root
)
```

## Hot Reload#

Tailwind styles work seamlessly with hot reload. The stylesheet is automatically deduplicated - when you modify your code and the app reloads, the existing Tailwind stylesheet is replaced rather than duplicated.

## esbuild Configuration#

Tailwind is enabled by default in the scaffolded esbuild config:

```
import { tailwindPlugin } from "onejs-unity/esbuild"

await esbuild.build({
    plugins: [
        tailwindPlugin({
            content: ["./**/*.{tsx,ts,jsx,js}"],
            safelist: ["bg-blue-500", "bg-red-500"],  // optional
        }),
    ],
})
```

| Option | Description |
| --- | --- |
| `content` | File patterns to scan for class names |
| `safelist` | Classes to always include, even if not found in source files (useful for classes stored in variables or external data) |

---
**Title:** Styling — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/styling](https://v3.onejs.com/docs/core-concepts/styling)

---

# Page Structure Map
```text
Styling — OneJS Docs
├── Inline Styles#
├── Value Types#
├── Layout Properties#
│   ├── Dimensions#
│   ├── Flexbox#
│   ├── Spacing#
│   └── Positioning#
├── Visual Properties#
│   ├── Backgrounds#
│   ├── Borders#
│   └── Display#
├── Typography#
│   ├── Custom Fonts#
│   ├── Text Overflow#
│   ├── Text Outline#
│   └── All Typography Properties#
├── Background Image Tint#
├── 9-Slice#
├── Transform#
│   ├── translate#
│   ├── rotate#
│   ├── scale#
│   ├── transformOrigin#
│   └── C# Struct Pass-through#
├── Transitions#
├── Other Properties#
├── USS Stylesheets#
├── StyleSheet API#
│   ├── loadStyleSheet#
│   ├── compileStyleSheet#
│   ├── removeStyleSheet#
│   ├── clearStyleSheets#
│   └── Hot Reload Behavior#
├── USS Variables#
├── Pseudo-classes#
├── Combining Styles#
├── Dynamic Styles#
├── Style Objects#
├── Common Patterns#
│   ├── Centering#
│   ├── Full Screen#
│   └── Responsive Width#
└── Debugging Styles#
    ├── Dump Visual Tree#
    ├── Find Elements by Class#
    ├── Find Elements by Type#
    └── Common Debugging Scenarios#
```

---

OneJS supports inline styles, USS (Unity Style Sheets), CSS Modules, and Tailwind (built-in, no external dependencies).

## Inline Styles#

Apply styles directly to elements:

```
<View
    style={{
        width: 200,
        height: 100,
        backgroundColor: "#1a1a1a",
        padding: 20,
        borderRadius: 8,
    }}
>
    <Label text="Styled content" style={{ color: "#ffffff" }} />
</View>
```

## Value Types#

**Numbers**: Interpreted as pixels:

```
style={{ width: 100, padding: 20 }}
```

**Strings**: For percentages, keywords, or complex values:

```
style={{ width: "50%", height: "auto" }}
```

**Colors**: Hex, rgb, rgba, or named colors:

```
style={{
    backgroundColor: "#ff0000",
    color: "rgb(255, 255, 255)",
    borderColor: "rgba(0, 0, 0, 0.5)",
}}
```

## Layout Properties#

### Dimensions#

```
style={{
    width: 200,
    height: 100,
    minWidth: 50,
    maxWidth: 400,
    minHeight: 50,
    maxHeight: 300,
}}
```

### Flexbox#

```
style={{
    flexDirection: "row",      // "row" | "column" | "row-reverse" | "column-reverse"
    flexWrap: "wrap",          // "nowrap" | "wrap" | "wrap-reverse"
    justifyContent: "center",  // "flex-start" | "center" | "flex-end" | "space-between" | "space-around"
    alignItems: "center",      // "flex-start" | "center" | "flex-end" | "stretch"
    alignContent: "center",
    flexGrow: 1,
    flexShrink: 0,
    flexBasis: "auto",
}}
```

### Spacing#

```
style={{
    // Shorthand (applies to all sides)
    margin: 10,
    padding: 20,

    // Or individual sides
    marginTop: 10,
    marginRight: 10,
    marginBottom: 10,
    marginLeft: 10,
    paddingTop: 20,
    paddingRight: 20,
    paddingBottom: 20,
    paddingLeft: 20,
}}
```

Shorthands like `margin`, `padding`, `borderWidth`, `borderColor`, and `borderRadius` are automatically expanded to individual properties.

### Positioning#

```
style={{
    position: "absolute",  // "relative" | "absolute"
    top: 10,
    right: 10,
    bottom: 10,
    left: 10,
}}
```

## Visual Properties#

### Backgrounds#

```
style={{
    backgroundColor: "#2a2a2a",
    backgroundImage: texture,  // Texture2D or RenderTexture
}}
```

Background images accept Unity `Texture2D`, `RenderTexture`, or GPU compute RenderTextures:

```
import { loadImage } from "onejs-unity/assets"

function Card() {
    const [texture, setTexture] = useState(null)

    useEffect(() => {
        const tex = loadImage("images/card-bg.png")
        setTexture(tex)
    }, [])

    return (
        <View style={{ backgroundImage: texture, width: 200, height: 150 }}>
            <Label text="Card Content" />
        </View>
    )
}
```

### Borders#

```
style={{
    borderWidth: 1,
    borderColor: "#333333",
    borderRadius: 8,
    // Or individual sides
    borderTopWidth: 2,
    borderLeftColor: "#ff0000",
    borderTopLeftRadius: 4,
}}
```

### Display#

```
style={{
    display: "flex",     // "flex" | "none"
    visibility: "visible", // "visible" | "hidden"
    overflow: "hidden",  // "visible" | "hidden"
    opacity: 0.8,
}}
```

## Typography#

```
<Label
    text="Styled text"
    style={{
        fontSize: 16,
        color: "#ffffff",
        unityFontStyleAndWeight: "bold", // "normal" | "bold" | "italic" | "bold-and-italic"
        unityTextAlign: "middle-center", // Text alignment
        whiteSpace: "normal",            // "normal" | "nowrap"
        letterSpacing: 2,
        wordSpacing: 4,
    }}
/>
```

### Custom Fonts#

Use `loadFontDefinition` from `onejs-unity/assets` to load a font and apply it via `unityFontDefinition`:

```
import { loadFontDefinition } from "onejs-unity/assets"

function StyledText() {
    const [fontDef, setFontDef] = useState(null)

    useEffect(() => {
        const def = loadFontDefinition("fonts/Consolas-Regular.ttf")
        setFontDef(def)
    }, [])

    return (
        <Label style={{ unityFontDefinition: fontDef, fontSize: 18 }}>
            Custom font text
        </Label>
    )
}
```

### Text Overflow#

```
style={{
    textOverflow: "ellipsis",                // "clip" | "ellipsis"
    unityTextOverflowPosition: "end",        // "end" | "start" | "middle"
    whiteSpace: "nowrap",                    // Required for ellipsis to work
}}
```

### Text Outline#

```
style={{
    unityTextOutlineColor: "#000000",
    unityTextOutlineWidth: 1,
}}
```

### All Typography Properties#

| Property | Type | Values |
| --- | --- | --- |
| `fontSize` | Length | `16`, `"16px"` |
| `color` | Color | `"#fff"`, `"rgb(255,0,0)"` |
| `unityFont` | object | C# `Font` object |
| `unityFontDefinition` | object | C# `FontDefinition` from `loadFontDefinition()` |
| `unityFontStyleAndWeight` | enum | `"normal"`, `"bold"`, `"italic"`, `"bold-and-italic"` |
| `unityTextAlign` | enum | `"upper-left"`, `"middle-center"`, etc. |
| `whiteSpace` | enum | `"normal"`, `"nowrap"` |
| `letterSpacing` | Length | `2`, `"2px"` |
| `wordSpacing` | Length | `4`, `"4px"` |
| `unityParagraphSpacing` | Length | `10`, `"10px"` |
| `unityTextOutlineColor` | Color | `"#000"` |
| `unityTextOutlineWidth` | Length | `1`, `"1px"` |
| `textOverflow` | enum | `"clip"`, `"ellipsis"` |
| `unityTextOverflowPosition` | enum | `"end"`, `"start"`, `"middle"` |

## Background Image Tint#

Tint the background image with a color:

```
style={{
    backgroundImage: texture,
    unityBackgroundImageTintColor: "rgba(255, 0, 0, 0.5)",
}}
```

## 9-Slice#

Control how background images are sliced for scalable UI:

```
style={{
    backgroundImage: texture,
    unitySliceTop: 12,
    unitySliceRight: 12,
    unitySliceBottom: 12,
    unitySliceLeft: 12,
    unitySliceScale: 1,
}}
```

## Transform#

Apply rotation, scale, and translation transforms using friendly shorthand values:

```
style={{
    translate: [30, 0],
    rotate: 45,
    scale: 1.5,
    transformOrigin: ["50%", "50%"],
}}
```

### translate#

Accepts `[x, y]` where values are numbers (px) or strings (`"50%"`, `"10px"`):

```
style={{ translate: [30, 0] }}       // 30px right
style={{ translate: [0, 20] }}       // 20px down
style={{ translate: ["50%", 0] }}    // 50% of own width
style={{ translate: ["50%", "50%"] }}
```

### rotate#

Accepts a number (degrees) or string with unit:

```
style={{ rotate: 45 }}           // 45 degrees
style={{ rotate: "0.5turn" }}    // half turn (180deg)
style={{ rotate: "45deg" }}
style={{ rotate: "1.57rad" }}
```

### scale#

Accepts a number (uniform) or `[x, y]`:

```
style={{ scale: 1.5 }}           // 150% on both axes
style={{ scale: 0.5 }}           // 50% on both axes
style={{ scale: [2, 0.5] }}      // stretch horizontal, squash vertical
```

### transformOrigin#

Accepts `[x, y]` (same value types as translate):

```
style={{ transformOrigin: ["0%", "0%"], rotate: 30 }}     // top-left pivot
style={{ transformOrigin: ["50%", "50%"], rotate: 30 }}    // center pivot
style={{ transformOrigin: ["100%", "100%"], rotate: 30 }}  // bottom-right pivot
```

### C# Struct Pass-through#

You can also pass C# structs directly. They are auto-wrapped in `Style*` types:

```
const UIE = CS.UnityEngine.UIElements

style={{
    translate: new UIE.Translate(
        new UIE.Length(30, UIE.LengthUnit.Pixel),
        new UIE.Length(0, UIE.LengthUnit.Pixel)
    ),
    rotate: new UIE.Rotate(UIE.Angle.Degrees(45)),
}}
```

## Transitions#

Animate style changes with transitions. These accept C# `StyleList` values:

```
style={{
    transitionProperty: /* StyleList of property names */,
    transitionDuration: /* StyleList of TimeValue */,
    transitionDelay: /* StyleList of TimeValue */,
    transitionTimingFunction: /* StyleList of EasingFunction */,
}}
```

For most use cases, transitions are easier to define in USS:

```
.animated {
    transition-property: background-color, opacity;
    transition-duration: 0.3s;
    transition-timing-function: ease-in-out;
}
```

## Other Properties#

| Property | Type | Values |
| --- | --- | --- |
| `unityOverflowClipBox` | enum | `"padding-box"`, `"content-box"` |
| `cursor` | object | C# `Cursor` struct |

## USS Stylesheets#

For reusable styles, use USS files:

```
/* styles/main.uss */
.card {
    background-color: #2a2a2a;
    padding: 20px;
    border-radius: 8px;
    margin-bottom: 10px;
}

.card-title {
    font-size: 18px;
    color: #ffffff;
    margin-bottom: 10px;
}

.button-primary {
    background-color: #0066cc;
    color: #ffffff;
    padding: 10px 20px;
    border-radius: 4px;
}

.button-primary:hover {
    background-color: #0077ee;
}
```

Load and apply the stylesheet:

```
// Load at startup
loadStyleSheet("styles/main.uss")

// Use classes
<View className="card">
    <Label text="Card Title" className="card-title" />
    <Button text="Action" className="button-primary" />
</View>
```

**Note:** CSS Modules and Tailwind embed styles directly in the JS bundle, so they work in builds automatically. USS files loaded via `loadStyleSheet()` are loaded from the filesystem and need to be placed in your `assets/` folder to be included in builds.

## StyleSheet API#

OneJS provides functions for loading and managing stylesheets at runtime.

### loadStyleSheet#

Load a USS file from the working directory:

```
loadStyleSheet("styles/main.uss")  // Returns true if successful
```

### compileStyleSheet#

Compile a USS string and apply it to the root element. If a stylesheet with the same name already exists, it will be replaced automatically (deduplication):

```
const uss = `
.my-class {
    background-color: #333;
    padding: 10px;
}
`
compileStyleSheet(uss, "my-styles")  // Name used for deduplication
```

This is how CSS Modules and Tailwind work internally - they embed USS in the JavaScript bundle and call `compileStyleSheet()` at runtime.

### removeStyleSheet#

Remove a specific stylesheet by name:

```
removeStyleSheet("my-styles")  // Returns true if found and removed
```

### clearStyleSheets#

Remove all JS-loaded stylesheets. Does not affect Unity asset-based stylesheets:

```
const count = clearStyleSheets()  // Returns number of stylesheets removed
console.log(`Removed ${count} stylesheets`)
```

### Hot Reload Behavior#

Stylesheets are automatically deduplicated by name. When you hot reload your app, re-importing CSS Modules or Tailwind won't accumulate duplicate styles - the existing stylesheet is replaced.

## USS Variables#

Define reusable values with custom properties:

```
:root {
    --accent: #FF6600;
    --spacing: 20px;
    --bg-dark: #333333;
}

.card {
    background-color: var(--bg-dark);
    padding: var(--spacing);
}

.card-title {
    color: var(--accent);
}
```

Use `var()` with a fallback value for when the variable is undefined:

```
.text {
    color: var(--undefined-var, #00CCFF);
}
```

Variables work in all styling methods: plain USS files, CSS Modules, and inline `compileStyleSheet()` calls.

**Limitations:** USS variables are simpler than CSS variables. `var()` cannot be nested inside other functions like `rgb()`, and mathematical operations are not supported.

## Pseudo-classes#

USS supports pseudo-classes for interactive states:

```
.button:hover {
    background-color: #333333;
}

.button:active {
    background-color: #444444;
}

.button:focus {
    border-color: #0066cc;
}

.input:focus {
    border-color: #0066cc;
    border-width: 2px;
}
```

## Combining Styles#

Use both className and inline styles:

```
<View
    className="card"
    style={{ width: 300 }}  // Inline overrides USS
>
    <Label text="Content" />
</View>
```

Inline styles take precedence over USS classes.

## Dynamic Styles#

Compute styles based on state:

```
function Button({ active, children }) {
    return (
        <View
            style={{
                backgroundColor: active ? "#0066cc" : "#333333",
                padding: 10,
                borderRadius: 4,
            }}
        >
            {children}
        </View>
    )
}
```

## Style Objects#

Extract and reuse style objects:

```
const styles = {
    container: {
        padding: 20,
        backgroundColor: "#1a1a1a",
    },
    title: {
        fontSize: 24,
        color: "#ffffff",
        marginBottom: 20,
    },
    button: {
        padding: 10,
        backgroundColor: "#0066cc",
        borderRadius: 4,
    },
}

function Screen() {
    return (
        <View style={styles.container}>
            <Label text="Title" style={styles.title} />
            <Button text="Click" style={styles.button} />
        </View>
    )
}
```

## Common Patterns#

### Centering#

```
// Center children
<View style={{
    justifyContent: "center",
    alignItems: "center",
    height: "100%",
}}>
    <Label text="Centered" />
</View>
```

### Full Screen#

```
<View style={{
    position: "absolute",
    top: 0,
    right: 0,
    bottom: 0,
    left: 0,
}}>
    {/* Content fills screen */}
</View>
```

### Responsive Width#

```
<View style={{
    width: "100%",
    maxWidth: 800,
    marginLeft: "auto",
    marginRight: "auto",
}}>
    {/* Centered, max 800px */}
</View>
```

## Debugging Styles#

When USS selectors aren't applying as expected, use the built-in debugging utilities to inspect the visual tree.

### Dump Visual Tree#

Use `__dumpUI()` to output the element hierarchy with USS classes:

```
// Dump entire UI from root
console.log(__dumpUI())

// Dump specific element with depth limit
console.log(__dumpUI(myElement, 5))

// Include computed styles
console.log(__dumpUI(myElement, 5, true))
```

Output shows element types, names, and USS classes:

```
<VisualElement name="root" class="unity-ui-document">
  <VisualElement class="container">
    <Button class="unity-button btn-primary">
    </Button>
  </VisualElement>
</VisualElement>
```

### Find Elements by Class#

Use `__findByClass()` to locate elements with a specific USS class:

```
// Find all elements with class "btn-primary"
console.log(__findByClass("btn-primary"))
```

Returns element info including pseudo-states and computed styles:

```
Found 2 elements with class 'btn-primary':
  Type: Button
  Name: submit-btn
  Classes: [unity-button, btn-primary]
  Pseudo States: [:hover]
  Styles:
    backgroundColor: rgba(0.24, 0.47, 0.99, 1.00)
```

### Find Elements by Type#

Use `__findByType()` to find all elements of a specific type:

```
// Find all TextField elements
console.log(__findByType("TextField"))

// Find all Buttons
console.log(__findByType("Button"))
```

### Common Debugging Scenarios#

**Selector not matching**: Dump the element to see its actual USS classes and verify your selector matches.

**Style not applying**: Check if a higher-specificity rule is overriding yours. Inline styles always win.

**Pseudo-class issues**: Use `__findByClass()` to see which pseudo-states (`:hover`, `:focus`, etc.) are active.

**Inherited styles**: Remember that UI Toolkit uses visual tree inheritance, not DOM inheritance. Dump the parent chain to trace style sources.

---
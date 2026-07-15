**Title:** CSS Modules — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/css-modules](https://v3.onejs.com/docs/guides/css-modules)

---

# Page Structure Map
```text
CSS Modules — OneJS Docs
├── Setup#
├── Basic Usage#
├── How It Works#
├── Dynamic Classes#
├── TypeScript Support#
├── Combining Classes#
├── Pseudo-classes#
├── USS Variables#
├── Global Styles#
├── File Structure#
├── Full Example#
├── Hot Reload#
└── Manual Cleanup#
```

---

CSS Modules provide scoped styling for your components. Class names are automatically hashed to prevent conflicts. Like Tailwind, the styles are embedded in your JavaScript bundle and work in standalone builds.

**Related:** Styling Basics | Tailwind CSS

## Setup#

Add the esbuild plugin to your config:

```
// esbuild.config.mjs
import { ussModulesPlugin } from "onejs-unity/esbuild"

await esbuild.build({
    // ...
    plugins: [
        ussModulesPlugin({ generateTypes: true }),
    ],
})
```

## Basic Usage#

Create a `.module.uss` file:

```
/* Button.module.uss */
.container {
    padding: 12px 24px;
    border-radius: 8px;
}

.primary {
    background-color: #e94560;
    color: #ffffff;
}

.primary:hover {
    background-color: #ff6b6b;
}
```

Import and use in your component:

```
import { Button } from "onejs-react"
import styles from "./Button.module.uss"

function PrimaryButton({ children, onClick }) {
    return (
        <Button
            className={`${styles.container} ${styles.primary}`}
            onClick={onClick}
        >
            {children}
        </Button>
    )
}
```

## How It Works#

The plugin transforms `.module.uss` files at build time:

1.  Generates a unique hash from the file path
2.  Appends the hash to all class names: `.container` becomes `.container__a1b2c3`
3.  Exports a mapping object: `{ container: "container__a1b2c3" }`
4.  Calls `compileStyleSheet()` to inject the CSS at runtime

Your component uses the scoped class names, avoiding conflicts with other styles.

## Dynamic Classes#

Use bracket notation for dynamic class selection:

```
import styles from "./Button.module.uss"

type Variant = "primary" | "secondary" | "ghost"

function Button({ variant = "primary", children }) {
    const className = `${styles.container} ${styles[variant]}`
    return <Button className={className}>{children}</Button>
}
```

## TypeScript Support#

With `generateTypes: true`, the plugin creates `.d.ts` files automatically:

```
// Button.module.uss.d.ts (auto-generated)
declare const styles: {
    readonly "container": string
    readonly "primary": string
    readonly "secondary": string
    readonly "ghost": string
}
export default styles
```

This gives you full autocomplete and type checking for class names.

## Combining Classes#

Multiple classes work as expected:

```
<View className={`${styles.card} ${styles.elevated} ${styles.rounded}`}>
    {children}
</View>
```

Combine with conditional classes:

```
<View className={`${styles.button} ${active ? styles.active : ""}`}>
    {children}
</View>
```

## Pseudo-classes#

USS pseudo-classes work normally:

```
/* Input.module.uss */
.input {
    border-width: 1px;
    border-color: #333;
}

.input:hover {
    border-color: #666;
}

.input:focus {
    border-color: #0066cc;
    border-width: 2px;
}
```

The plugin correctly scopes the base class while preserving pseudo-class selectors.

## USS Variables#

Custom properties work in `.module.uss` files:

```
/* Theme.module.uss */
.container {
    --accent: #e94560;
    --text: #ffffff;
    padding: 20px;
}

.title {
    color: var(--accent);
}

.body {
    color: var(--text);
}
```

See USS Variables for full details.

## Global Styles#

For global styles that shouldn't be scoped, use regular `.uss` files:

```
/* global.uss - not scoped */
.unity-button {
    /* Targets Unity's built-in button class */
}
```

Load global styles with `compileStyleSheet()` directly.

## File Structure#

Recommended organization:

```
components/
├── Button.tsx
├── Button.module.uss
├── Card.tsx
├── Card.module.uss
└── Input.tsx
└── Input.module.uss
```

## Full Example#

```
/* Card.module.uss */
.card {
    background-color: #1a1a1a;
    border-radius: 8px;
    padding: 20px;
}

.header {
    flex-direction: row;
    justify-content: space-between;
    margin-bottom: 16px;
}

.title {
    font-size: 18px;
    color: #ffffff;
}

.content {
    color: #999999;
}
```

```
import { View, Label } from "onejs-react"
import styles from "./Card.module.uss"

function Card({ title, children }) {
    return (
        <View className={styles.card}>
            <View className={styles.header}>
                <Label text={title} className={styles.title} />
            </View>
            <View className={styles.content}>
                {children}
            </View>
        </View>
    )
}
```

## Hot Reload#

CSS Modules work seamlessly with hot reload. Stylesheets are automatically deduplicated by name - when you modify a `.module.uss` file and the app reloads, the existing stylesheet is replaced rather than duplicated.

This happens because each CSS Module uses its file path as the stylesheet name when calling `compileStyleSheet()`. The runtime tracks stylesheets by name and removes any existing stylesheet before adding the updated one.

## Manual Cleanup#

If you need to programmatically manage stylesheets:

```
// Remove a specific CSS Module's styles
removeStyleSheet("components/Button.module.uss")

// Remove all JS-loaded stylesheets
clearStyleSheets()
```

See StyleSheet API for more details.

--- 
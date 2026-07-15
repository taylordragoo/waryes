**Title:** Refs & Direct Access — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/refs](https://v3.onejs.com/docs/core-concepts/refs)

---

# Page Structure Map
```text
Refs & Direct Access — OneJS Docs
├── Basic Usage#
├── VisualElement API#
│   ├── Properties#
│   ├── Focus Methods#
│   ├── Class List Methods#
│   ├── Hierarchy Methods#
│   └── Layout#
├── Typed Refs#
│   └── Available Element Types#
├── TextField Methods#
├── Measuring Elements#
├── Direct Style Manipulation#
├── Callback Refs#
└── Best Practices#
```

---

Use React refs to access the underlying UI Toolkit `VisualElement` directly. This is useful for imperative operations, measurements, or accessing features not exposed through props.

## Basic Usage#

```
import { useRef, useEffect } from "react"
import { View, Label } from "onejs-react"

function FocusableInput() {
    const inputRef = useRef(null)

    useEffect(() => {
        // Focus the element on mount
        inputRef.current?.Focus()
    }, [])

    return <TextField ref={inputRef} placeholder="Auto-focused" />
}
```

## VisualElement API#

When you access `ref.current`, you get a proxy to the C# `VisualElement`. Available properties and methods:

### Properties#

| Property | Type | Description |
| --- | --- | --- |
| `__csHandle` | `number` | Internal C# object handle |
| `__csType` | `string` | C# type name |
| `name` | `string` | Element name |
| `visible` | `boolean` | Visibility state |
| `enabledSelf` | `boolean` | Whether element is enabled |
| `enabledInHierarchy` | `boolean` | Enabled state including parents |
| `focusable` | `boolean` | Whether element can receive focus |
| `style` | `object` | Inline style access |
| `childCount` | `number` | Number of children |
| `parent` | `VisualElement` | Parent element |

### Focus Methods#

```
const ref = useRef(null)

// Focus the element
ref.current?.Focus()

// Remove focus
ref.current?.Blur()
```

### Class List Methods#

```
const ref = useRef(null)

// Add USS class
ref.current?.AddToClassList("highlighted")

// Remove USS class
ref.current?.RemoveFromClassList("highlighted")

// Check if class exists
const hasClass = ref.current?.ClassListContains("highlighted")

// Clear all classes
ref.current?.ClearClassList()
```

### Hierarchy Methods#

```
const containerRef = useRef(null)

// Add a child element
const label = new CS.UnityEngine.UIElements.Label()
containerRef.current?.Add(label)

// Insert at index
containerRef.current?.Insert(0, label)

// Remove a child
containerRef.current?.Remove(label)

// Remove by index
containerRef.current?.RemoveAt(0)

// Clear all children
containerRef.current?.Clear()

// Find child index
const index = containerRef.current?.IndexOf(label)
```

### Layout#

```
// Force repaint
ref.current?.MarkDirtyRepaint()
```

## Typed Refs#

Components export typed element interfaces for better IntelliSense:

```
import type { TextFieldElement, ButtonElement, ScrollViewElement } from "onejs-react"

function Form() {
    const textFieldRef = useRef<TextFieldElement>(null)
    const buttonRef = useRef<ButtonElement>(null)

    const handleSubmit = () => {
        const value = textFieldRef.current?.value
        console.log("Submitted:", value)
    }

    return (
        <View>
            <TextField ref={textFieldRef} placeholder="Enter text" />
            <Button ref={buttonRef} text="Submit" onClick={handleSubmit} />
        </View>
    )
}
```

### Available Element Types#

| Type | Specific Properties |
| --- | --- |
| `VisualElement` | Base type for all elements |
| `TextElement` | `text: string` |
| `LabelElement` | `text: string` |
| `ButtonElement` | `text: string` |
| `TextFieldElement` | `value`, `text`, `isReadOnly`, `maxLength`, `SelectAll()` |
| `ToggleElement` | `value: boolean`, `text: string` |
| `SliderElement` | `value`, `lowValue`, `highValue` |
| `ScrollViewElement` | `scrollOffset`, `ScrollTo(child)` |

## TextField Methods#

```
import type { TextFieldElement } from "onejs-react"

function SearchInput() {
    const ref = useRef<TextFieldElement>(null)

    return (
        <View>
            <TextField
                ref={ref}
                placeholder="Search..."
            />
            <Button
                text="Clear"
                onClick={() => {
                    if (ref.current) {
                        ref.current.value = ""
                    }
                }}
            />
            <Button
                text="Select All"
                onClick={() => ref.current?.SelectAll()}
            />
        </View>
    )
}
```

```
import type { ScrollViewElement, VisualElement } from "onejs-react"

function AutoScroll() {
    const scrollRef = useRef<ScrollViewElement>(null)
    const lastItemRef = useRef<VisualElement>(null)

    const scrollToBottom = () => {
        if (scrollRef.current && lastItemRef.current) {
            scrollRef.current.ScrollTo(lastItemRef.current)
        }
    }

    return (
        <View>
            <ScrollView ref={scrollRef} style={{ height: 300 }}>
                {items.map((item, i) => (
                    <View
                        key={i}
                        ref={i === items.length - 1 ? lastItemRef : null}
                    >
                        <Label text={item} />
                    </View>
                ))}
            </ScrollView>
            <Button text="Scroll to Bottom" onClick={scrollToBottom} />
        </View>
    )
}
```

## Measuring Elements#

Access resolved layout values:

```
function MeasuredElement() {
    const ref = useRef(null)
    const [size, setSize] = useState({ width: 0, height: 0 })

    return (
        <View
            ref={ref}
            onGeometryChanged={(e) => {
                setSize({
                    width: e.newRect.width,
                    height: e.newRect.height,
                })
            }}
        >
            <Label text={`Size: ${size.width}x${size.height}`} />
        </View>
    )
}
```

## Direct Style Manipulation#

While inline styles are preferred, you can manipulate styles directly:

```
function AnimatedElement() {
    const ref = useRef(null)

    const animate = () => {
        if (ref.current) {
            ref.current.style.backgroundColor = new CS.UnityEngine.Color(1, 0, 0, 1)
        }
    }

    return (
        <View ref={ref} style={{ width: 100, height: 100, backgroundColor: "#333" }}>
            <Button text="Turn Red" onClick={animate} />
        </View>
    )
}
```

## Callback Refs#

For more control, use callback refs:

```
function CallbackRefExample() {
    const handleRef = (element) => {
        if (element) {
            console.log("Element mounted:", element.__csType)
            element.AddToClassList("initialized")
        }
    }

    return <View ref={handleRef}>Content</View>
}
```

## Best Practices#

1.  **Prefer props over refs** - Use refs only when declarative props aren't sufficient
2.  **Check for null** - Always check `ref.current` before accessing
3.  **Use typed refs** - Import element types for better autocomplete
4.  **Avoid frequent direct manipulation** - Let React handle updates when possible

---
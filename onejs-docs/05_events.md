**Title:** Events — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/events](https://v3.onejs.com/docs/core-concepts/events)

---

# Page Structure Map
```text
Events — OneJS Docs
├── Click Events#
├── Pointer Events#
├── Focus Events#
├── Change Events#
├── Keyboard Events#
├── Event Propagation#
│   └── Suppressing native behavior#
├── Pointer Capture#
├── Other Events#
├── Event Object Properties#
│   ├── Pointer / Mouse Event#
│   ├── Wheel Event#
│   └── Change Event#
├── Event Mapping Reference#
│   ├── Click & Pointer#
│   ├── Mouse#
│   ├── Focus & Input#
│   ├── Drag & Drop#
│   ├── Layout & Transitions#
│   └── Navigation#
└── Custom Rendering#
```

---

Handle user interactions with React-style event props.

## Click Events#

```
<Button
    text="Click me"
    onClick={(e) => {
        console.log("Clicked at", e.x, e.y)
    }}
/>
```

## Pointer Events#

Track pointer (mouse/touch) interactions:

```
<View
    style={{ width: 200, height: 200, backgroundColor: "#333" }}
    onPointerDown={(e) => console.log("Down", e.x, e.y)}
    onPointerUp={(e) => console.log("Up")}
    onPointerMove={(e) => console.log("Move", e.x, e.y)}
    onPointerEnter={(e) => console.log("Enter")}
    onPointerLeave={(e) => console.log("Leave")}
>
    <Label text="Hover and click me" />
</View>
```

## Focus Events#

```
<TextField
    placeholder="Type here..."
    onFocus={(e) => console.log("Focused")}
    onBlur={(e) => console.log("Lost focus")}
/>
```

## Change Events#

```
<TextField value={text} onChange={(e) => setText(e.value)} />
<Toggle value={on} onChange={(e) => setOn(e.value)} label="Enable" />
<Slider value={vol} lowValue={0} highValue={100} onChange={(e) => setVol(e.value)} />
```

## Keyboard Events#

```
<TextField
    onKeyDown={(e) => {
        if (e.keyCode === CS.UnityEngine.KeyCode.Return) {
            console.log("Enter pressed!")
        }
    }}
/>
```

## Event Propagation#

Events bubble up through the component tree. Use `stopPropagation()` to prevent this:

```
<View onClick={() => console.log("Parent")}>
    <Button
        text="Click"
        onClick={(e) => {
            e.stopPropagation()
            console.log("Only this handler runs")
        }}
    />
</View>
```

### Suppressing native behavior#

`stopPropagation()` only stops OneJS's own (JavaScript-side) bubbling. To stop the **native** UI Toolkit behavior of a control underneath your handler — for example a `ScrollView`'s scroll — call `preventDefault()`:

```
// Keep the ScrollView from scrolling while the pointer is over this element
<ScrollView>
    <View onWheel={(e) => e.preventDefault()}>
        ...
    </View>
</ScrollView>
```

`preventDefault()` works on pointer (`onPointerDown`/`onPointerMove`/`onPointerUp`), `onClick`, and `onWheel` handlers. Both methods are lowercase (`e.preventDefault()`, `e.stopPropagation()`) — the PascalCase C# names are not available on the event object.

## Pointer Capture#

Lock pointer events to a specific element. Essential for reliable dragging. Without capture, fast pointer movement can leave the element's bounds and drop events.

```
useExtensions(CS.UnityEngine.UIElements.PointerCaptureHelper)

function Draggable({ children }) {
    const ref = useRef(null)
    const [pos, setPos] = useState({ x: 0, y: 0 })
    const startPos = useRef({ x: 0, y: 0 })

    return (
        <View
            ref={ref}
            style={{ position: "absolute", left: pos.x, top: pos.y }}
            onPointerDown={(e) => {
                ref.current.CapturePointer(e.pointerId)
                startPos.current = { x: e.x - pos.x, y: e.y - pos.y }
            }}
            onPointerMove={(e) => {
                if (ref.current.HasPointerCapture(e.pointerId)) {
                    setPos({
                        x: e.x - startPos.current.x,
                        y: e.y - startPos.current.y,
                    })
                }
            }}
            onPointerUp={(e) => ref.current.ReleasePointer(e.pointerId)}
        >
            {children}
        </View>
    )
}
```

`useExtensions` registers C# extension methods so they can be called on elements. See C# Interop: Extension Methods for details.

## Other Events#

```
// Geometry: detect size/position changes
<View onGeometryChanged={(e) => console.log(e.newRect)} />

// Transitions: monitor USS animations
<View
    onTransitionEnd={(e) => console.log("Done:", e.styleProperty)}
/>

// Navigation: gamepad/keyboard UI navigation
<View
    onNavigationMove={(e) => console.log("Direction:", e.direction)}
    onNavigationSubmit={(e) => console.log("Submit")}
/>

// Mouse-specific (prefer pointer events for cross-device support)
<View
    onMouseDown={(e) => console.log("Mouse", e.button)}
    onWheel={(e) => console.log("Scroll", e.deltaY)}
/>
```

## Event Object Properties#

### Pointer / Mouse Event#

| Property | Type | Description |
| --- | --- | --- |
| `type` | `string` | Event type name |
| `x` | `number` | X position |
| `y` | `number` | Y position |
| `button` | `number` | Mouse button (0=left, 1=middle, 2=right) |
| `pointerId` | `number` | Unique pointer identifier |

### Wheel Event#

| Property | Type | Description |
| --- | --- | --- |
| `type` | `string` | Event type name |
| `deltaX` | `number` | Horizontal scroll amount |
| `deltaY` | `number` | Vertical scroll amount (positive = down) |

### Change Event#

| Property | Type | Description |
| --- | --- | --- |
| `type` | `string` | Event type name |
| `value` | `T` | New value (type depends on control) |

## Event Mapping Reference#

### Click & Pointer#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onClick` | `ClickEvent` |
| `onPointerDown` | `PointerDownEvent` |
| `onPointerUp` | `PointerUpEvent` |
| `onPointerMove` | `PointerMoveEvent` |
| `onPointerEnter` | `PointerEnterEvent` |
| `onPointerLeave` | `PointerLeaveEvent` |
| `onPointerCancel` | `PointerCancelEvent` |
| `onPointerCapture` | `PointerCaptureEvent` |
| `onPointerCaptureOut` | `PointerCaptureOutEvent` |
| `onPointerStationary` | `PointerStationaryEvent` |

### Mouse#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onMouseDown` | `MouseDownEvent` |
| `onMouseUp` | `MouseUpEvent` |
| `onMouseMove` | `MouseMoveEvent` |
| `onMouseEnter` | `MouseEnterEvent` |
| `onMouseLeave` | `MouseLeaveEvent` |
| `onMouseOver` | `MouseOverEvent` |
| `onMouseOut` | `MouseOutEvent` |
| `onWheel` | `WheelEvent` |
| `onContextClick` | `ContextClickEvent` |

### Focus & Input#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onFocus` | `FocusEvent` |
| `onBlur` | `BlurEvent` |
| `onFocusIn` | `FocusInEvent` |
| `onFocusOut` | `FocusOutEvent` |
| `onChange` | `ChangeEvent<T>` |
| `onInput` | `InputEvent` |
| `onKeyDown` | `KeyDownEvent` |
| `onKeyUp` | `KeyUpEvent` |

### Drag & Drop#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onDragEnter` | `DragEnterEvent` |
| `onDragLeave` | `DragLeaveEvent` |
| `onDragUpdated` | `DragUpdatedEvent` |
| `onDragPerform` | `DragPerformEvent` |
| `onDragExited` | `DragExitedEvent` |

### Layout & Transitions#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onGeometryChanged` | `GeometryChangedEvent` |
| `onTransitionRun` | `TransitionRunEvent` |
| `onTransitionStart` | `TransitionStartEvent` |
| `onTransitionEnd` | `TransitionEndEvent` |
| `onTransitionCancel` | `TransitionCancelEvent` |

### Navigation#

| React Prop | UI Toolkit Event |
| --- | --- |
| `onNavigationMove` | `NavigationMoveEvent` |
| `onNavigationSubmit` | `NavigationSubmitEvent` |
| `onNavigationCancel` | `NavigationCancelEvent` |
| `onTooltip` | `TooltipEvent` |

## Custom Rendering#

For custom drawing with `onGenerateVisualContent`, see the Vector Drawing guide.

---
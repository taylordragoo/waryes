**Title:** Error Handling — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/error-handling](https://v3.onejs.com/docs/core-concepts/error-handling)

---

# Page Structure Map
```text
Error Handling — OneJS Docs
├── Import#
├── Basic Usage#
├── Custom Fallback#
├── Fallback with Error Details#
├── Props#
├── Error Logging#
├── Resetting the Boundary#
├── Nested Boundaries#
├── Format Error Utility#
├── What Errors Are Caught#
├── Event Handler Errors#
├── Async Error Handling#
└── Full App Example#
```

---

React errors can crash your entire UI. Use `ErrorBoundary` to catch errors and display fallback UI instead.

## Import#

```
import { ErrorBoundary } from "onejs-react"
```

## Basic Usage#

Wrap components that might throw errors:

```
<ErrorBoundary>
    <MyComponent />
</ErrorBoundary>
```

If `MyComponent` throws an error, ErrorBoundary displays a default error UI instead of crashing.

## Custom Fallback#

Provide your own fallback UI:

```
<ErrorBoundary fallback={<Label text="Something went wrong" />}>
    <MyComponent />
</ErrorBoundary>
```

## Fallback with Error Details#

Use a function to access error information:

```
<ErrorBoundary
    fallback={(error, errorInfo) => (
        <View style={{ padding: 20, backgroundColor: "#2d1b1b" }}>
            <Label
                text={`Error: ${error.message}`}
                style={{ color: "#ff6666", fontSize: 16 }}
            />
            <Label
                text={errorInfo.componentStack}
                style={{ color: "#ffaaaa", fontSize: 12, marginTop: 10 }}
            />
        </View>
    )}
>
    <MyComponent />
</ErrorBoundary>
```

## Props#

| Prop | Type | Description |
| --- | --- | --- |
| `children` | `ReactNode` | Components to wrap |
| `fallback` | `ReactNode` or `(error, errorInfo) => ReactNode` | Fallback UI |
| `onError` | `(error, errorInfo) => void` | Error callback |
| `onReset` | `() => void` | Reset callback |

## Error Logging#

Log errors for debugging or analytics:

```
<ErrorBoundary
    onError={(error, errorInfo) => {
        console.error("React Error:", error.message)
        console.error("Component Stack:", errorInfo.componentStack)

        // Send to error tracking service
        trackError(error, errorInfo)
    }}
>
    <App />
</ErrorBoundary>
```

## Resetting the Boundary#

ErrorBoundary can be reset to try rendering again:

```
function App() {
    const boundaryRef = useRef(null)

    return (
        <View>
            <ErrorBoundary
                ref={boundaryRef}
                fallback={
                    <View>
                        <Label text="Error occurred" />
                        <Button
                            text="Try Again"
                            onClick={() => boundaryRef.current?.reset()}
                        />
                    </View>
                }
                onReset={() => {
                    // Clean up any state that caused the error
                    resetAppState()
                }}
            >
                <MainContent />
            </ErrorBoundary>
        </View>
    )
}
```

## Nested Boundaries#

Use multiple boundaries for different sections:

```
function App() {
    return (
        <View>
            <ErrorBoundary fallback={<Label text="Header error" />}>
                <Header />
            </ErrorBoundary>

            <ErrorBoundary fallback={<Label text="Content error" />}>
                <MainContent />
            </ErrorBoundary>

            <ErrorBoundary fallback={<Label text="Footer error" />}>
                <Footer />
            </ErrorBoundary>
        </View>
    )
}
```

This way, an error in the header doesn't affect the rest of the app.

## Format Error Utility#

The `formatError` helper creates readable error strings:

```
import { ErrorBoundary, formatError } from "onejs-react"

<ErrorBoundary
    fallback={(error, errorInfo) => {
        const formatted = formatError(error, errorInfo.componentStack)
        console.log(formatted)

        return <Label text="Error logged" />
    }}
>
    <MyComponent />
</ErrorBoundary>
```

Output format:

```
Error: Something went wrong

Stack:
  at MyComponent
  at View
  at App

Component:
  at MyComponent
  at ErrorBoundary
  at App
```

## What Errors Are Caught#

ErrorBoundary catches:

-   Errors in render methods
-   Errors in lifecycle methods
-   Errors in constructors

ErrorBoundary does **NOT** catch:

-   Errors in event handlers (use try/catch)
-   Async errors (use try/catch in async functions)
-   Errors thrown outside React (global errors)

## Event Handler Errors#

Handle errors in event handlers manually:

```
function SafeButton() {
    const handleClick = () => {
        try {
            doSomethingRisky()
        } catch (error) {
            console.error("Click error:", error)
            // Show error to user
        }
    }

    return <Button text="Click" onClick={handleClick} />
}
```

## Async Error Handling#

```
function AsyncComponent() {
    const [error, setError] = useState(null)

    useEffect(() => {
        async function loadData() {
            try {
                const data = await fetchData()
                // Use data
            } catch (e) {
                setError(e.message)
            }
        }
        loadData()
    }, [])

    if (error) {
        return <Label text={`Error: ${error}`} style={{ color: "#ff6666" }} />
    }

    return <Label text="Content" />
}
```

## Full App Example#

```
import { render, ErrorBoundary, View, Label, Button } from "onejs-react"

function App() {
    return (
        <ErrorBoundary
            fallback={(error) => (
                <View style={{ padding: 40, alignItems: "center" }}>
                    <Label
                        text="Oops! Something went wrong."
                        style={{ fontSize: 20, color: "#ff6666", marginBottom: 20 }}
                    />
                    <Label
                        text={error.message}
                        style={{ color: "#ffaaaa", marginBottom: 20 }}
                    />
                    <Button
                        text="Reload App"
                        onClick={() => location.reload()}
                    />
                </View>
            )}
            onError={(error, info) => {
                // Log to console
                console.error("[App Error]", error)

                // Could send to external service
                // analytics.trackError(error, info)
            }}
        >
            <MainApp />
        </ErrorBoundary>
    )
}

render(<App />, __root)
```

---
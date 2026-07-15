**Title:** Async C# Methods — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/async-csharp](https://v3.onejs.com/docs/guides/async-csharp)

---

# Page Structure Map
```text
Async C# Methods — OneJS Docs
├── Basic Usage#
├── Return Types#
├── Error Handling#
├── With React#
├── How It Works#
└── Notes#
```

---

C# `async Task` methods automatically become JavaScript Promises. You can `await` them just like any other async operation.

## Basic Usage#

```
// C#
public static class DataLoader {
    public static async Task<string> LoadDataAsync(string url) {
        using var request = UnityWebRequest.Get(url);
        await request.SendWebRequest();
        return request.downloadHandler.text;
    }
}
```

```
// JavaScript
const data = await CS.DataLoader.LoadDataAsync("/api/data")
console.log(data)
```

Non-generic `Task` methods work the same way:

```
// C#
public static async Task SaveAsync(string path, string content) {
    await File.WriteAllTextAsync(path, content);
}
```

```
// JavaScript
await CS.DataLoader.SaveAsync("save.json", JSON.stringify(state))
// resolves to null (no return value)
```

## Return Types#

| C# Return Type | JavaScript Result |
| --- | --- |
| `Task` | `null` |
| `Task<int>`, `Task<float>`, `Task<bool>` | `number`, `number`, `boolean` |
| `Task<string>` | `string` |
| `Task<T>` (reference type) | C# proxy object |
| Already-completed `Task<T>` | Returns result directly (not a Promise) |

Already-completed tasks (e.g., `Task.FromResult(42)`) skip the Promise path entirely and return the result synchronously.

## Error Handling#

Faulted and canceled tasks reject the Promise, so use standard `try/catch`:

```
// C#
public static class Api {
    public static async Task<string> FetchUserAsync(int id) {
        var user = await db.FindAsync(id);
        if (user == null) throw new Exception("User not found");
        return user.Name;
    }

    public static async Task<string> SlowQueryAsync(CancellationToken ct) {
        await Task.Delay(5000, ct);
        return "done";
    }
}
```

```
// JavaScript
try {
    const name = await CS.Api.FetchUserAsync(999)
} catch (e) {
    console.log(e.message) // "User not found"
}
```

Canceled tasks reject with `"Task was canceled"`:

```
try {
    const result = await CS.Api.SlowQueryAsync(token)
} catch (e) {
    console.log(e.message) // "Task was canceled"
}
```

## With React#

Use the standard `useEffect` + async pattern for loading data from C# async methods:

```
import { View, Label, render } from "onejs-react"
import { useState, useEffect } from "react"

function PlayerStats({ playerId }) {
    const [stats, setStats] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    useEffect(() => {
        let cancelled = false

        async function load() {
            try {
                const result = await CS.MyGame.StatsManager.GetStatsAsync(playerId)
                if (!cancelled) setStats(result)
            } catch (e) {
                if (!cancelled) setError(e.message)
            } finally {
                if (!cancelled) setLoading(false)
            }
        }

        load()
        return () => { cancelled = true }
    }, [playerId])

    if (loading) return <Label text="Loading..." />
    if (error) return <Label text={`Error: ${error}`} />

    return (
        <View>
            <Label text={`Level: ${stats.Level}`} />
            <Label text={`Score: ${stats.Score}`} />
        </View>
    )
}
```

## How It Works#

When you call a C# async method from JavaScript:

1.  If the task is **already completed**, the result is returned directly (no Promise)
2.  If the task is **still pending**, a Promise is created and linked to the task via an internal ID
3.  When the task completes on the C# side, the result is queued for delivery
4.  On the **next frame tick**, the Promise is resolved (or rejected) and your `await` or `.then()` handler runs

This means async results arrive at least one frame after the task completes.

## Notes#

-   Only `Task` and `Task<T>` are supported (`ValueTask` is not supported)
-   Pending tasks are resolved during the next `Tick()` call (up to 50 per frame to avoid stalling)
-   If more than 100 tasks are pending, a warning is logged automatically
-   For async asset loading from Unity's `Resources/` folder, see `loadResourceAsync`
-   See C# Interop for general C# usage from JavaScript

---
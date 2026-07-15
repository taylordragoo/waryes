**Title:** C# Interop — OneJS Docs

**Source:** [https://v3.onejs.com/docs/core-concepts/csharp-interop](https://v3.onejs.com/docs/core-concepts/csharp-interop)

---

# Page Structure Map
```text
C# Interop — OneJS Docs
├── Accessing Types#
│   └── ES6 Imports#
├── Working with Objects#
├── Enums#
├── Generic Types#
│   └── Generic Methods#
├── Arrays and Collections#
├── Async Methods#
├── Events and Delegates#
│   ├── Delegate Fields#
│   ├── C# Events#
│   └── React Cleanup#
├── Extension Methods#
├── Common Patterns#
│   ├── Async Alternatives to Coroutines#
│   └── Per-Frame Logic#
└── Performance Tips#
```

---

The `CS` global gives you access to any C# type.

## Accessing Types#

Access C# types through their full namespace:

```
// Static methods and properties
CS.UnityEngine.Debug.Log("Hello from JS!")
const dt = CS.UnityEngine.Time.deltaTime

// Create instances
const vec = new CS.UnityEngine.Vector3(1, 2, 3)
console.log(vec.x, vec.y, vec.z)
```

Your own code works the same way:

```
// C#
namespace MyGame {
    public class GameManager {
        public static int Score { get; set; }
        public static void AddScore(int points) { Score += points; }
    }
}
```

```
CS.MyGame.GameManager.AddScore(100)
const score = CS.MyGame.GameManager.Score
```

### ES6 Imports#

With esbuild's import transform plugin, you can use ES6 import syntax instead of `CS.*`:

```
import { GameObject, Rigidbody } from "UnityEngine"
import { GameManager } from "MyGame"

const go = new GameObject("Player")
go.AddComponent(Rigidbody)
GameManager.AddScore(100)
```

This is the preferred style for app code. The examples below use it where applicable.

## Working with Objects#

Use `new` to create C# objects. Properties and fields are accessed with dot notation:

```
import { GameObject, Rigidbody, MeshRenderer } from "UnityEngine"

const go = new GameObject("MyObject")

const rb = go.AddComponent(Rigidbody)
rb.mass = 2.0
rb.useGravity = true

const pos = go.transform.position
console.log(pos.x, pos.y, pos.z)
```

This works for any C# type: structs, classes, and your own types all use dot notation.

> **Note:** `console.log(obj)` on a C# object shows its proxy handle (e.g. `[CSObject ?#27]`), not the field values. To see actual data, log the fields directly: `console.log(obj.x, obj.y)`.

## Enums#

Access enum values directly:

```
import { Space, Vector3, KeyCode } from "UnityEngine"

transform.Translate(new Vector3(1, 0, 0), Space.World)

if (keyCode === KeyCode.Space) {
    console.log("Space pressed!")
}
```

Enum values are plain numbers in JS. An enum member like `KeyCode.Space` resolves to its underlying integer, and an enum field read from a C# object returns that same number, so you can compare them with `===` and use bitwise math for `[Flags]` enums:

```
const dir = someComponent.Direction      // number, e.g. 1
if (dir === Direction.Bar) { /* ... */ } // compares correctly

const mask = Flags.A | Flags.B           // bitwise combine
```

## Generic Types#

Create bound generic types with function-call syntax:

```
import { List, Dictionary } from "System.Collections.Generic"
import { Int32, String } from "System"

const numbers = new (List(Int32))()
numbers.Add(1)
numbers.Add(2)

const scores = new (Dictionary(String, Int32))()
scores.set_Item("player1", 100)
```

### Generic Methods#

Generic _methods_ (e.g., `Create<T>()`, `GetValue<T>()`) are not supported by the interop layer. Only generic _types_ like `List<T>` and `Dictionary<TKey, TValue>` work, using the function-call syntax shown above.

The workaround is to add a non-generic wrapper method in C#:

```
// Instead of calling Create<MyThing>() from JS,
// add a non-generic overload in C#:
public Task<MyThing> CreateMyThing(CancellationToken ct) => Create<MyThing>(ct);
```

```
// Then call the non-generic version from JS
const result = await factory.CreateMyThing(cancellationToken)
```

## Arrays and Collections#

C# collections use `.Length`/`.Count` and indexers:

```
import { Renderer } from "UnityEngine"

const renderers = go.GetComponentsInChildren(Renderer)
for (let i = 0; i < renderers.Length; i++) {
    renderers[i].enabled = false
}
```

Use `toArray` from `onejs-react` to convert to JS arrays for `.map()`, `.filter()`, etc.:

```
import { toArray } from "onejs-react"

const items = toArray(inventory.Items)
items.map(item => console.log(item.Name))

// Safe with null, returns []
const npcs = toArray(currentPlace?.NPCs)
```

Both `List<T>` and `T[]` work with `toArray`.

## Async Methods#

C# `async Task` methods return Promises:

```
// C#
public class DataLoader {
    public static async Task<string> LoadDataAsync(string url) { ... }
}
```

```
import { DataLoader } from "MyGame"

const data = await DataLoader.LoadDataAsync("/api/data")
```

See Async C# Methods for error handling and React patterns.

## Events and Delegates#

### Delegate Fields#

For C# **delegate fields** (`Action`, `Action<T>`, etc.), assign a JS function directly. This replaces any existing handler:

```
// C#
public struct PlayerData {
    public string name;
    public int score;
}

public class NetworkManager {
    public static Action OnConnected;
    public static Action<PlayerData> OnPlayerJoined;
}
```

```
import { NetworkManager } from "MyGame"

// No parameters
NetworkManager.OnConnected = () => console.log("Connected!")

// Typed parameter, access fields directly
NetworkManager.OnPlayerJoined = (player) => {
    console.log(player.name, player.score)
}

// Clear with null
NetworkManager.OnConnected = null
```

### C# Events#

For C# **events**, use `add_`/`remove_` (equivalent to `+=`/`-=`). Events support multiple subscribers:

```
const handler = () => console.log("Players changed!")
GameManager.add_OnPlayersChanged(handler)
GameManager.remove_OnPlayersChanged(handler)  // must pass the same reference
```

### React Cleanup#

Clean up subscriptions in `useEffect` to avoid leaks during hot reload:

```
// Delegate field: assign and clear
useEffect(() => {
    NetworkManager.OnPlayerJoined = (player) => {
        setPlayers(prev => [...prev, { name: player.name, score: player.score }])
    }
    return () => { NetworkManager.OnPlayerJoined = null }
}, [])

// C# event: add and remove
useEffect(() => {
    const handler = () => setCount(GameManager.PlayerCount)
    GameManager.add_OnPlayersChanged(handler)
    return () => GameManager.remove_OnPlayersChanged(handler)
}, [])
```

`useEffect` cleanup works for hot reload, but **won't run during scene transitions** because Unity destroys the JSRunner before React can run cleanup functions. If your app uses multiple scenes, null out static delegates in `onStop` instead:

```
export function onStop() {
    NetworkManager.OnPlayerJoined = null
}
```

See Lifecycle Hooks for details on `onStop`.

## Extension Methods#

C# extension methods aren't discoverable via reflection on the target type. Use `useExtensions` to register them (like a C# `using` statement):

```
useExtensions(CS.UnityEngine.UIElements.PointerCaptureHelper)

const el = ref.current
el.CapturePointer(0)       // PointerCaptureHelper.CapturePointer(el, 0)
el.ReleasePointer(0)       // PointerCaptureHelper.ReleasePointer(el, 0)
```

```
useExtensions(CS.UnityEngine.ImageConversion)

const tex = new CS.UnityEngine.Texture2D(2, 2)
tex.LoadImage(bytes)  // ImageConversion.LoadImage(tex, bytes)
```

Call `useExtensions` once per static class at module level.

## Common Patterns#

### Async Alternatives to Coroutines#

JS has `async`/`await` and `requestAnimationFrame`, so you often don't need C# coroutines:

```
import { Time, Color } from "UnityEngine"

async function fadeOut(renderer, duration) {
    const start = Time.time
    while (true) {
        await new Promise(r => requestAnimationFrame(r))
        const t = Math.min((Time.time - start) / duration, 1)
        renderer.material.color = new Color(1, 1, 1, 1 - t)
        if (t >= 1) break
    }
}
```

### Per-Frame Logic#

```
import { Input, KeyCode } from "UnityEngine"

function update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
        console.log("Jump!")
    }
    requestAnimationFrame(update)
}
requestAnimationFrame(update)
```

## Performance Tips#

1.  **Cache type references**: avoid repeated lookups in hot loops
2.  **Batch property access**: read multiple values at once when possible
3.  **Use fast paths**: common operations like `Time.deltaTime` are optimized

For performance-critical code running every frame, see the Zero-Allocation Interop guide.

---
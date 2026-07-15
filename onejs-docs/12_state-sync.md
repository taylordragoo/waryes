**Title:** C# State Sync — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/state-sync](https://v3.onejs.com/docs/guides/state-sync)

---

# Page Structure Map
```text
C# State Sync — OneJS Docs
├── useFrameSync#
│   ├── Simple Mode#
│   ├── Selector Mode#
│   ├── Collections and the Parent/Child Pattern#
│   └── Full Example#
├── useEventSync#
│   ├── Convention Form#
│   └── Explicit Form#
├── Choosing Between Them#
├── Performance#
│   └── FastPath Registration#
└── Reference#
    ├── useThrottledSync#
    ├── Dependencies#
    └── TypeScript Declarations#
```

---

React doesn't know when C# values change. OneJS provides two hooks to sync C# state into React components.

|  | `useFrameSync` | `useEventSync` |
| --- | --- | --- |
| **How it works** | Polls every frame, re-renders on change | Subscribes to C# events, reads on fire |
| **C# requirements** | Just a property | Property + `event Action` + `Invoke()` |
| **Per-frame cost** | 1 proxy read (zero with FastPath) | Zero |
| **Best for** | Simple primitives, Unity built-ins | Collections, complex/derived state |

Start with `useFrameSync` for most properties. It's more ergonomic (no events to declare or fire), less error-prone (can't forget to invoke an event), and zero-alloc with FastPath.

Switch to `useEventSync` when you have collections (avoids per-frame `toArray()`), many properties that rarely change, or C# code that already fires events.

```
import { useFrameSync, useEventSync, toArray } from "onejs-react"
```

## useFrameSync#

Polls a C# value every frame and re-renders when it changes. Simplest way to read C# state. No C# events needed.

### Simple Mode#

For primitives (numbers, strings, booleans), pass a getter. Re-renders when the value changes (`Object.is` comparison):

```
const health = useFrameSync(() => player.Health)
const gold = useFrameSync(() => player.Gold)
const name = useFrameSync(() => target?.name ?? "Unknown")
```

With FastPath registered, simple mode is **zero-allocation** per frame. No C# boilerplate required, just a property.

### Selector Mode#

C# objects always return the **same proxy reference** (cached by handle), so `Object.is` would always return `true`. Pass a **selector** as the second argument to extract comparable primitives:

```
const place = useFrameSync(
    () => gameState.currentPlace,
    (p) => [p?.Name, p?.NPCs?.Count]
)
```

Each frame, the selector's output is compared element-by-element. If any value changed, the component re-renders. The returned object is always fresh from the getter.

This works with any C# object (game state, Unity structs, quest data):

```
// Structs: extract the fields you care about
const pos = useFrameSync(
    () => player.transform.position,
    (p) => [p?.x, p?.y, p?.z]
)

// Version stamp: catch any change with a single selector
const quest = useFrameSync(
    () => questManager.activeQuest,
    (q) => [q?.Version]
)
```

> **Note:** Selector mode allocates a new array every frame for comparison, even when nothing changed. For complex state with many selectors, consider `useEventSync` instead.

### Collections and the Parent/Child Pattern#

C# collections (`List<T>`, arrays) have `.Count` and indexers but no `.map()` or `.filter()`. Use `toArray` to convert them to JS arrays:

```
const items = toArray(inventory) // reads .Count, loops indexer, returns JS array
```

For lists with mutable items (inventories, NPC lists, quest logs), split into a **parent** that watches list structure and **children** that each watch their own item:

-   **Parent** selects `[collection.Count]`: re-renders only on add/remove
-   **Child** selects `[item.Name, item.Durability, ...]`: re-renders only when that item changes

When one item's durability drops, only that child re-renders. The parent and all other children are untouched. When an item is added or removed, the parent re-renders and mounts/unmounts the affected child.

### Full Example#

A complete example showing all three patterns together: primitives, selectors, and the parent/child collection pattern.

**C# component:**

```
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {
    public class Item {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Durability { get; set; }
        public int StackCount { get; set; }
        public int Version { get; set; }
    }

    public class PlayerController : MonoBehaviour {
        public int Health { get; set; } = 100;
        public int Gold { get; set; } = 50;
        public List<Item> Inventory { get; set; } = new() {
            new Item { Id = 1, Name = "Sword", Durability = 100, StackCount = 1, Version = 1 },
            new Item { Id = 2, Name = "Shield", Durability = 80, StackCount = 1, Version = 1 },
            new Item { Id = 3, Name = "Potion", Durability = 1, StackCount = 5, Version = 1 },
        };
    }
}
```

> Both C# properties (`{ get; set; }`) and plain fields work. The proxy tries properties first, then falls back to fields.

**TypeScript UI:**

```
import React from "react"
import { View, Text, ScrollView, render } from "onejs-react"
import { useFrameSync, toArray } from "onejs-react"

interface Item {
    Id: number
    Name: string
    Durability: number
    StackCount: number
    Version: number
}

interface PlayerController {
    Health: number
    Gold: number
    Inventory: { Count: number; [index: number]: Item }
}

// Cache reference at module scope, don't call Find() inside a getter
const player = CS.UnityEngine.GameObject.Find("Player")
    ?.GetComponent("MyGame.PlayerController") as unknown as PlayerController | null

// --- Child: only re-renders when THIS item's properties change ---
const ItemSlot = React.memo(function ItemSlot({ item }: { item: Item }) {
    const data = useFrameSync(
        () => item,
        (i) => [i.Name, i.Durability, i.StackCount]
    )

    return (
        <View style={{ flexDirection: "row", padding: 4 }}>
            <Text style={{ color: "#fff" }}>
                {data.Name} x{data.StackCount} ({data.Durability} dur)
            </Text>
        </View>
    )
})

// --- Parent: only re-renders when items are added/removed ---
function InventoryPanel() {
    const inv = useFrameSync(
        () => player?.Inventory ?? null,
        (i) => [i?.Count]
    )

    if (!inv) return <Text style={{ color: "#666" }}>No inventory</Text>

    return (
        <ScrollView>
            {toArray<Item>(inv).map(item => (
                <ItemSlot key={item.Id} item={item} />
            ))}
        </ScrollView>
    )
}

// --- App: primitives use simple mode ---
function App() {
    const health = useFrameSync(() => player?.Health ?? 0)
    const gold = useFrameSync(() => player?.Gold ?? 0)

    return (
        <View style={{ padding: 16, backgroundColor: "#1a1a1a" }}>
            <Text style={{ color: "#4ade80", fontSize: 18 }}>HP: {health}</Text>
            <Text style={{ color: "#fbbf24", fontSize: 18 }}>Gold: {gold}</Text>
            <Text style={{ color: "#888", marginTop: 16, marginBottom: 8 }}>
                Inventory:
            </Text>
            <InventoryPanel />
        </View>
    )
}

render(<App />, __root)
```

If your C# items use a version stamp (e.g., via Fody), the child selector can be simplified to `(i) => [i.Version]` to catch any property change.

This pattern scales to any nested structure: places with NPCs, quest logs with objectives, skill trees with nodes.

## useEventSync#

Subscribes to C# events instead of polling. Zero work when nothing changes. Use this when:

-   Your C# code already fires events on state change
-   You have collections that are expensive to poll (avoids per-frame `toArray`)
-   You have many properties that rarely change

### Convention Form#

Pass the source object and property name. Subscribes to `On{Prop}Changed` and reads the property automatically:

```
const health = useEventSync(player, "Health")
const score = useEventSync(player, "Score")
```

C# side needs standard events:

```
public class Character : MonoBehaviour {
    float _health = 100f;
    public float Health => _health;
    public event Action OnHealthChanged;

    public void TakeDamage(float amount) {
        _health -= amount;
        OnHealthChanged?.Invoke();
    }
}
```

No source generators, no special base classes.

### Explicit Form#

For derived state or multiple event sources, pass a getter and an array of `[source, eventName]` pairs:

```
const itemCount = useEventSync(
    () => inventory.Items.Count,
    [[inventory, "OnItemAdded"], [inventory, "OnItemRemoved"]]
)
```

Works with static events too:

```
const score = useEventSync(
    () => CS.MyGame.GameManager.Score,
    [[CS.MyGame.GameManager, "OnScoreChanged"]]
)
```

## Choosing Between Them#

|  | `useFrameSync` | `useEventSync` |
| --- | --- | --- |
| C# boilerplate | Just a property | Property + event + Invoke() |
| JS boilerplate | One line | One line |
| Per-frame cost | 1 proxy read (zero with FastPath) | Zero |
| Best for | Simple primitives, Unity built-ins, third-party types | Collections, complex state, event-driven architectures |

For a typical game UI:

-   **Player stats** (health, gold, score): `useFrameSync`. Simplest, zero-alloc with FastPath.
-   **Inventory list**: `useEventSync`. Avoids per-frame collection traversal.
-   **Transform position**: `useFrameSync`. No events on Transform, already FastPath-registered.
-   **Quest state**: `useEventSync`. Complex state, fires events on quest progress.

## Performance#

`useFrameSync` simple mode with FastPath is **zero-allocation** per frame. For most simple properties, it's the right default.

Selector mode allocates a new array every frame for comparison. With many selectors at 60fps, this adds up. Switch to `useEventSync` if you see GC pressure in the Unity Profiler.

### FastPath Registration#

Common Unity types (`Transform`, `Time`, `GameObject`, etc.) are pre-registered. For your own types:

```
// Register once (e.g., in Awake or RuntimeInitializeOnLoadMethod)
QuickJSNative.FastPath.Property<PlayerController, int>("Health", p => p.Health);
QuickJSNative.FastPath.Property<PlayerController, int>("Gold", p => p.Gold);
QuickJSNative.FastPath.Property<PlayerController, float>("Speed", p => p.Speed, (p, v) => p.Speed = v);
```

No JavaScript changes needed. The same `useFrameSync(() => player.Health)` call now hits the fast path automatically.

See Zero-Allocation Interop for the full details.

## Reference#

### useThrottledSync#

Polls at a custom interval instead of every frame:

```
import { useThrottledSync } from "onejs-react"

const gameTime = useThrottledSync(() => gameManager.GameTime, 1000)  // every 1s
const score = useThrottledSync(() => gameManager.Score, 250)         // every 250ms
```

### Dependencies#

All sync hooks accept an optional `deps` array as the last argument, for when the source reference can change:

```
const health = useFrameSync(() => currentPlayer.Health, [currentPlayer])
const score = useEventSync(currentPlayer, "Score", [currentPlayer])

const place = useFrameSync(
    () => currentPlayer.Location,
    (loc) => [loc?.Name],
    [currentPlayer]
)
```

### TypeScript Declarations#

Define interfaces for your C# types in a `.d.ts` file:

```
// types/game.d.ts
declare namespace CS.MyGame {
    interface Item {
        readonly Id: number
        readonly Name: string
        readonly Durability: number
        readonly StackCount: number
        readonly Version: number
    }

    interface PlayerController {
        readonly Health: number
        readonly Gold: number
        readonly Inventory: { Count: number; [index: number]: Item }
    }
}
```

---
**Title:** Zero-Allocation Interop — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/zero-alloc](https://v3.onejs.com/docs/guides/zero-alloc)

---

# Page Structure Map
```text
Zero-Allocation Interop — OneJS Docs
├── FastPath#
│   ├── Built-in Registrations#
│   ├── Registering Your Own Types#
│   ├── Registration API#
│   ├── Checking Registrations#
│   └── Supported Types#
├── `za` API#
│   ├── Pre-registered Bindings (zero-alloc)#
│   ├── Dynamic Bindings (reduced alloc)#
│   └── Debugging#
├── When to Use What#
└── Limitations#
```

---

The standard `CS` proxy uses reflection and allocates memory on every call. Fine for occasional use, but in per-frame loops this creates GC pressure and frame hitches. OneJS provides two systems to eliminate these allocations:

-   **FastPath**: intercepts regular `CS` proxy calls and handles them with typed delegates. No JS changes needed.
-   **`za` API**: explicit zero-alloc function bindings from JavaScript. For complex static method calls.

## FastPath#

### Built-in Registrations#

Common Unity types are pre-registered. These are already zero-alloc with no setup:

-   **Time**: `deltaTime`, `unscaledDeltaTime`, `time`, `frameCount`, `timeScale`, etc.
-   **Transform**: `position`, `localPosition`, `rotation`, `localScale`, `forward`, `eulerAngles`, etc.
-   **GameObject**: `activeSelf`, `activeInHierarchy`, `name`, `tag`, `layer`, `transform`
-   **Input**: `mousePosition`, `GetKey`, `GetAxis`, etc. (legacy input)
-   **Screen**: `width`, `height`, `dpi`
-   **Mathf**: `Sin`, `Cos`, `Abs`, `Sqrt`, `Floor`, `Ceil`, `Round`

### Registering Your Own Types#

Register properties you read per-frame in C#:

```
public class PlayerController : MonoBehaviour
{
    public int Health { get; set; } = 100;
    public int Gold { get; set; } = 50;
    public float Speed { get; set; } = 5f;

    void Awake()
    {
        // Register once. All instances of this type benefit.
        QuickJSNative.FastPath.Property<PlayerController, int>("Health", p => p.Health);
        QuickJSNative.FastPath.Property<PlayerController, int>("Gold", p => p.Gold);
        QuickJSNative.FastPath.Property<PlayerController, float>(
            "Speed", p => p.Speed, (p, v) => p.Speed = v  // getter + setter
        );
    }
}
```

JavaScript stays the same. `useFrameSync` and direct property access both benefit:

```
// These are now zero-alloc on the C# side
const health = useFrameSync(() => player.Health)
const gold = useFrameSync(() => player.Gold)
```

Only register properties you actually read per-frame. Most apps only need a handful. The typed lambdas compile to direct method calls, making them AOT-safe on all platforms including iOS and consoles. A source generator approach could automate this in the future, but manual registration keeps things explicit and avoids adding build infrastructure.

### Registration API#

```
// Instance properties
FastPath.Property<TTarget, TValue>(name, getter)
FastPath.Property<TTarget, TValue>(name, getter, setter)

// Static properties
FastPath.StaticProperty<TOwner, TValue>(name, getter)
FastPath.StaticProperty<TOwner, TValue>(name, getter, setter)

// Instance methods (0-1 args, with or without return)
FastPath.Method<TTarget>(name, action)
FastPath.Method<TTarget, TResult>(name, func)
FastPath.Method<TTarget, TArg0, TResult>(name, func)
FastPath.Method<TTarget, TArg0>(name, action)

// Static methods (0-6 args)
FastPath.StaticMethod<TOwner, TResult>(name, func)
FastPath.StaticMethod<TOwner, TArg0, TResult>(name, func)
// ... up to 6 arguments
```

### Checking Registrations#

```
FastPath.IsTypeRegistered<PlayerController>()          // any registrations for this type?
FastPath.IsRegistered<PlayerController>("Health")      // specific member?
FastPath.GetRegisteredMembers<PlayerController>()      // ["Health", "Gold", "Speed"]
```

### Supported Types#

Primitives and Unity structs are fully zero-alloc. Strings and reference types still allocate on the C# side.

| Type | Zero-alloc |
| --- | --- |
| `int`, `float`, `double`, `bool`, `long` | Yes |
| `Vector2`, `Vector3`, `Vector4`, `Quaternion`, `Color` | Yes |
| `string` | No (UTF8 marshaling) |
| Reference types | No (handle + type hint) |

## `za` API#

For complex static method calls (GPU compute, physics batching, etc.) where you need explicit control over the binding.

```
import { za } from "onejs-unity/interop"
```

### Pre-registered Bindings (zero-alloc)#

Register typed delegates in C# and use them from JS by binding ID:

```
// C#: register and expose binding IDs
public static int RaycastId = QuickJSNative.Bind<float, float, float, float, float, float, float, bool>(
    (ox, oy, oz, dx, dy, dz, dist) => Physics.Raycast(
        new Vector3(ox, oy, oz), new Vector3(dx, dy, dz), dist
    )
);
```

```
// JS: use binding ID for zero-alloc calls
const raycast = za.fromId(MyBridge.RaycastId, 7)

function update() {
    if (raycast(ox, oy, oz, dx, dy, dz, maxDist)) {
        // hit
    }
    requestAnimationFrame(update)
}
```

### Dynamic Bindings (reduced alloc)#

For convenience when pre-registration isn't worth the setup. Faster than `CS` proxy but still allocates per call:

```
const Physics = za.static("UnityEngine.Physics", {
    Raycast: 4,
    SphereCast: 5,
})

Physics.Raycast(origin, direction, maxDistance, layerMask)
```

Single method variant:

```
const getTime = za.method("UnityEngine.Time", "get_time", 0)
```

### Debugging#

```
console.log("Binding count:", za.getBindingCount())
const info = za.getBindingInfo(bindingId)
// { typeName, methodName, argCount }
```

## When to Use What#

| Approach | GC Alloc | JS Changes | Best for |
| --- | --- | --- | --- |
| Standard `CS` proxy | High | None | Prototyping, occasional calls |
| FastPath | Zero | None | Per-frame property reads, `useFrameSync` |
| `za.static()` / `za.method()` | Reduced | Yes | Development, non-critical paths |
| `za.fromId()` | Zero | Yes | Maximum performance static methods |

Use FastPath for property reads and simple methods. Use `za.fromId()` for complex static method dispatch like GPU compute. For most apps, FastPath alone is sufficient.

## Limitations#

| Limitation | Workaround |
| --- | --- |
| `za` max 8 arguments | Split into multiple calls |
| `za` static methods only | Create static C# wrappers |
| No generic methods | Use concrete overloads |
| Strings valid during call only | Copy if needed beyond call |

---
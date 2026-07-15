**Title:** Quick Start — OneJS Docs

**Source:** [https://v3.onejs.com/docs/quickstart](https://v3.onejs.com/docs/quickstart)

---

# Page Structure Map
```text
Quick Start — OneJS Docs
├── Install#
├── Set Up a Project#
├── Write a Component#
├── Calling C##
├── Quick Prototyping with JSPad#
├── Coming from V2?#
└── Next Steps#
```

---

Install OneJS and build a working UI component in your Unity project.

## Install#

Open **Window > Package Manager**, click **+** > **Add package from git URL**, and enter:

```
https://github.com/Singtaa/OneJS.git#onejs-v3
```

Or clone directly into your Assets folder:

```
cd YourProject/Assets
git clone -b onejs-v3 https://github.com/Singtaa/OneJS.git
```

## Set Up a Project#

1.  Create a new GameObject and add the **JSRunner** component
2.  Click **Initialize Project** in the inspector
3.  Enter Play Mode

JSRunner handles everything: installs dependencies, builds the bundle, and starts a file watcher. The first run takes a moment while packages install; once it's done, the starter UI appears in the Game view.

Your project folder lives next to your scene, named after the GameObject:

```
Assets/Scenes/YourScene/
└── App/                      # Named after your GameObject
    ├── ~/                    # Working directory (~ = Unity ignores)
    │   ├── index.tsx         # Entry point
    │   ├── package.json
    │   ├── tsconfig.json
    │   └── esbuild.config.mjs
    ├── app.js.txt            # Built bundle
    └── PanelSettings.asset   # Project marker
```

## Write a Component#

Open `~/index.tsx` and replace its contents:

```
import { render, View, Label, Button } from "onejs-react"
import { useState } from "react"

function Counter() {
    const [count, setCount] = useState(0)

    return (
        <View style={{ padding: 20, alignItems: "flex-start" }}>
            <Label text={`Count: ${count}`} style={{ fontSize: 24, marginBottom: 10 }} />
            <Button text="Increment" onClick={() => setCount(c => c + 1)} />
        </View>
    )
}

render(<Counter />, __root)
```

Save the file. JSRunner automatically watches for changes, rebuilds, and hot-reloads. The UI updates in the Game view without leaving the editor. This is standard React: `useState`, JSX, component composition. The difference is that `<Button>` creates a real `UnityEngine.UIElements.Button` rendered on the GPU.

## Calling C##

Access any C# API through the `CS` global:

```
function DebugButton() {
    const handleClick = () => {
        CS.UnityEngine.Debug.Log("Hello from JS!")
        console.log(`Time: ${CS.UnityEngine.Time.time}`)
    }

    return <Button text="Debug" onClick={handleClick} />
}
```

This works with all Unity APIs, your own game code, and third-party libraries.

## Quick Prototyping with JSPad#

For quick experiments without file setup, use **JSPad** instead:

1.  Add the **JSPad** component to a GameObject
2.  Write TSX directly in the inspector
3.  Click **Build**, then enter Play Mode

JSPad is self-contained. The bundle lives in the scene file. Great for learning and quick tests. Use JSRunner for real projects.

## Coming from V2?#

| V2 | V3 |
| --- | --- |
| ScriptEngine + Runner + Bundler | Single JSRunner component |
| Project-root `App/` folder | Scene Folder convention |
| Shared PanelSettings | Per-instance (auto-created) |
| `npm run setup` to initialize | "Initialize Project" button |
| Preact | React 19 |
| Separate Tailwind watcher | Built-in (`import "onejs:tailwind"`) |

---
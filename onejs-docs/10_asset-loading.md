
**Title:** Asset Loading — OneJS Docs

**Source:** [https://v3.onejs.com/docs/guides/assets](https://v3.onejs.com/docs/guides/assets)

---

# Page Structure Map
```text
Asset Loading — OneJS Docs
├── Quick Start#
├── Functions#
├── Using with Components#
├── Absolute Paths and URLs#
│   ├── Absolute Paths#
│   └── URLs#
├── Path Resolution#
├── Builds#
├── Unity Resources#
│   ├── `assets/` vs `Resources/`#
│   └── Async Resource Loading#
└── Package Assets#
```

---

Load images, fonts, and data files from your project. Path resolution between Editor and builds is handled automatically.

## Quick Start#

Place files in `assets/` inside your working directory, then load by relative path:

```
~/                             <- Working directory
├── assets/
│   ├── logo.png
│   ├── fonts/
│   │   └── Inter.ttf
│   └── data/
│       └── config.json
├── index.tsx
└── esbuild.config.mjs
```

```
import { loadImage, loadFont, loadJson } from "onejs-unity/assets"

const logo = loadImage("logo.png")
const font = loadFont("fonts/Inter.ttf")
const config = loadJson("data/config.json")
```

## Functions#

| Function | Returns | Description |
| --- | --- | --- |
| `loadImage(path)` | `Texture2D | VectorImage` | Load PNG, JPG, or SVG. Accepts relative or absolute paths |
| `loadImageFromUrl(url)` | `Promise<Texture2D>` | Load an image from a URL |
| `loadFont(path)` | `Font` | Load a font file (TTF, OTF) |
| `loadFontDefinition(path)` | `FontDefinition` | Load a font for UI Toolkit styling |
| `loadText(path)` | `string` | Load a text file |
| `loadJson<T>(path)` | `T` | Load and parse JSON |
| `loadBytes(path)` | `Uint8Array` | Load raw bytes |
| `assetExists(path)` | `boolean` | Check if an asset exists |
| `getAssetPath(path)` | `string` | Get the resolved full path |

All file-based functions are **synchronous** and throw if not found. `loadImageFromUrl` is async. Paths are case-sensitive on most platforms.

## Using with Components#

The `<Image>` component's `src` prop loads from the assets folder:

```
import { Image } from "onejs-react"

<Image src="hero.png" style={{ width: 200, height: 200 }} />
```

For background images and custom fonts, use the load functions:

```
import { loadImage, loadFontDefinition } from "onejs-unity/assets"

<View style={{ backgroundImage: loadImage("bg.png"), width: 300, height: 200 }} />

<Label style={{ unityFontDefinition: loadFontDefinition("fonts/Inter.ttf") }}>
    Custom font text
</Label>
```

## Absolute Paths and URLs#

For files outside the `assets/` folder (user save directories, Steam Workshop cache, remote servers), use absolute paths or URLs.

### Absolute Paths#

All load functions accept absolute file paths. Asset resolution is bypassed entirely:

```
import { loadImage } from "onejs-unity/assets"

const thumb = loadImage("/path/to/steam/workshop/thumbnail.png")

// Works with <Image> src prop too
<Image src="/path/to/user/saves/preview.png" style={{ width: 128, height: 128 }} />

// Works with backgroundImage
<View style={{ backgroundImage: loadImage("/path/to/bg.png"), width: 300, height: 200 }} />
```

### URLs#

`loadImageFromUrl` loads remote images. Returns a `Promise<Texture2D>`:

```
import { loadImageFromUrl } from "onejs-unity/assets"

const tex = await loadImageFromUrl("https://example.com/thumbnail.jpg")
<Image image={tex} style={{ width: 128, height: 128 }} />
```

> **Note:** Textures from absolute paths or URLs are not cached. Cache the `Texture2D` yourself if loading repeatedly, and call `Object.Destroy()` when done to avoid memory leaks.

## Path Resolution#

In the **Editor**, assets load from the filesystem. In **builds**, from `StreamingAssets`.

| Context | Resolved path for `logo.png` |
| --- | --- |
| Editor | `{WorkingDir}/assets/logo.png` |
| Build | `{StreamingAssets}/onejs/assets/logo.png` |

This is automatic. Absolute paths bypass resolution entirely.

## Builds#

Assets are copied to `StreamingAssets` automatically during Unity builds via `JSRunnerBuildProcessor`:

1.  Finds each JSRunner's working directory
2.  Copies `{WorkingDir}/assets/` to `Assets/StreamingAssets/onejs/assets/`
3.  Skips `.meta` files
4.  Flushes the destination first to remove stale files

No esbuild plugin or manual setup needed.

## Unity Resources#

For assets that need Unity's import pipeline, use the `Resources/` folder instead. Place files in `Assets/Resources/` and Unity imports them as native types.

```
import { Image } from "onejs-react"
import { VectorImage } from "UnityEngine/UIElements"

const icon = CS.UnityEngine.Resources.Load("icon", VectorImage)
<Image image={icon} style={{ width: 64, height: 64 }} />
```

Path is relative to any `Resources/` folder, without file extension.

### `assets/` vs `Resources/`#

|  | `assets/` folder | `Resources/` folder |
| --- | --- | --- |
| **Location** | `{WorkingDir}/assets/` | `Assets/Resources/` |
| **Loading** | `loadImage()`, `loadFont()`, etc. | `Resources.Load()` |
| **Processing** | Raw files, read at runtime | Unity-imported at build time |
| **Best for** | Images, fonts, JSON, text data | Assets needing Unity import processing |

Most of the time `assets/` is all you need.

### Async Resource Loading#

Load Unity resources without blocking:

```
import { loadResourceAsync } from "onejs-unity/assets"

const textAsset = await loadResourceAsync("MyData/config", CS.UnityEngine.TextAsset)
console.log(textAsset.text)

// Returns null if not found
const missing = await loadResourceAsync("DoesNotExist")
```

With React:

```
function ConfigPanel() {
    const [config, setConfig] = useState(null)

    useEffect(() => {
        let cancelled = false
        async function load() {
            const asset = await loadResourceAsync("GameConfig", CS.UnityEngine.TextAsset)
            if (!cancelled && asset) {
                setConfig(JSON.parse(asset.text))
            }
        }
        load()
        return () => { cancelled = true }
    }, [])

    if (!config) return <Label text="Loading..." />
    return <Label text={`Difficulty: ${config.difficulty}`} />
}
```

|  | `loadImage` / `loadText` | `loadResourceAsync` |
| --- | --- | --- |
| **Source** | `assets/` folder (raw files) | Unity `Resources/` folder |
| **Sync/Async** | Synchronous | Asynchronous (Promise) |
| **Asset processing** | Raw bytes/text at runtime | Unity-imported at build time |
| **Not found** | Throws error | Returns `null` |

## Package Assets#

npm packages can distribute assets using the `@namespace/` convention:

```
my-ui-kit/
├── package.json
├── assets/
│   └── @my-ui-kit/
│       ├── icons/
│       │   └── check.png
│       └── backgrounds/
│           └── gradient.png
└── src/
    └── index.ts
```

Load with the `@` prefix:

```
const icon = loadImage("@my-ui-kit/icons/check.png")
```

In the Editor, assets resolve through `node_modules`. In builds, they're copied to `StreamingAssets/onejs/assets/@my-ui-kit/`.

To generate the manifest for Editor resolution, add `copyAssetsPlugin` to your esbuild config:

```
import { copyAssetsPlugin } from "onejs-unity/esbuild"

const config = {
    plugins: [
        copyAssetsPlugin(),
    ],
}
```

---
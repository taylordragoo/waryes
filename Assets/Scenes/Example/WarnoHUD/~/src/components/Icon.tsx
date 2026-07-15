import { Image } from "onejs-react"

export type IconName =
    | "activity"
    | "chevron-right"
    | "chevron-up"
    | "circle"
    | "clipboard-list"
    | "crosshair"
    | "eye"
    | "fast-forward"
    | "map-pin"
    | "monitor"
    | "pause"
    | "play"
    | "settings"
    | "star"
    | "target"

export default function Icon({
    name,
    className = "w-4 h-4",
    tint = "#94A3B8",
}: {
    name: IconName
    className?: string
    tint?: string
}) {
    return (
        <Image
            src={`icons/${name}.svg`}
            className={className}
            tintColor={tint}
            scaleMode="ScaleToFit"
            pickingMode="Ignore"
        />
    )
}

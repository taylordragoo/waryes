import { type ReactNode } from "react"
import { Button, Label, View, useFrameSync } from "onejs-react"
import { getHudBridge } from "../bridge"
import Icon, { type IconName } from "./Icon"

function Panel({ children, className = "" }: { children: ReactNode; className?: string }) {
    return (
        <View
            className={`bg-[#183545]/85 border-2 border-cyan-700 rounded-sm text-slate-200 ${className}`}
            pickingMode="Ignore"
        >
            {children}
        </View>
    )
}

function formatClock(totalSeconds: number): string {
    const minutes = Math.floor(totalSeconds / 60)
    const seconds = totalSeconds % 60
    return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`
}

function speedLabel(scale: number): string {
    if (scale === 0) return "PAUSED"
    if (scale === 1) return "STANDARD SPEED"
    return `${scale}X SPEED`
}

function weaponHeading(weapon: string): string {
    if (weapon.includes("M2HB")) return "HEAVY MACHINE GUN"
    if (weapon.includes("MACHINE GUN")) return "MACHINE GUN"
    return "PRIMARY ARMAMENT"
}

const wrappedTextStyle = {
    whiteSpace: "normal" as const,
    flexShrink: 1,
}

export function TopBar() {
    const credits = useFrameSync(() => getHudBridge()?.Credits ?? 0)
    const side = useFrameSync(() => getHudBridge()?.PlayerSide ?? "BLUE")
    const units = useFrameSync(() => getHudBridge()?.PlayerUnitCount ?? 0)
    const selected = useFrameSync(() => getHudBridge()?.SelectedUnitCount ?? 0)

    return (
        <Panel className="flex flex-row items-center bg-[#183545]/90 rounded-b-md border-t-0 px-6 py-1.5">
            <View className="flex flex-row items-center mr-8">
                <Icon name="star" className="w-4 h-4 mr-2" tint="#EAB308" />
                <Label className="text-yellow-500 text-sm font-bold">${credits}</Label>
            </View>
            <View className="flex flex-row items-center mr-8">
                <Icon name="circle" className="w-4 h-4 mr-2" tint="#4AC6D7" />
                <Label className="text-[#4AC6D7] text-sm font-bold">{side}</Label>
            </View>
            <View className="flex flex-row items-center">
                <Icon name="map-pin" className="w-4 h-4 mr-2" tint="#94A3B8" />
                <Label className="bg-[#2A4570] text-white text-xs font-bold px-2 py-0.5 rounded-sm mr-1">{units}</Label>
                <Label className="bg-[#4A555A] text-white text-xs font-bold px-2 py-0.5 rounded-sm mr-1">{selected}</Label>
                <Label className="bg-[#7A2525] text-white text-xs font-bold px-2 py-0.5 rounded-sm">0</Label>
            </View>
        </Panel>
    )
}

function TimeButton({
    icon,
    scale,
    active,
    doubled = false,
}: {
    icon: IconName
    scale: number
    active: boolean
    doubled?: boolean
}) {
    return (
        <View
            onClick={() => getHudBridge()?.SetTimeScale(scale)}
            className={`w-8 h-7 mr-1 flex flex-row items-center justify-center border-2 rounded-sm ${
                active
                    ? "bg-[#2A6B84] border-cyan-300"
                    : "bg-[#2A6B84]/50 border-cyan-900"
            }`}
        >
            <Icon name={icon} className="w-4 h-4" tint="#4AC6D7" />
            {doubled && <Icon name={icon} className="w-4 h-4 -ml-2" tint="#4AC6D7" />}
        </View>
    )
}

export function TimeControls() {
    const elapsed = useFrameSync(() => getHudBridge()?.ElapsedSeconds ?? 0)
    const scale = useFrameSync(() => getHudBridge()?.TimeScale ?? 1)

    return (
        <View className="flex flex-col items-end">
            <Label className="text-white text-lg font-medium">{formatClock(elapsed)}</Label>
            <Label className="text-[#4AC6D7] text-[10px] font-bold tracking-wider">{speedLabel(scale)}</Label>
            <View className="flex flex-row mt-1">
                <TimeButton icon="pause" scale={0} active={scale === 0} />
                <TimeButton icon="play" scale={1} active={scale === 1} />
                <TimeButton icon="chevron-right" scale={1} active={false} />
                <TimeButton icon="fast-forward" scale={2} active={scale === 2} />
                <TimeButton icon="chevron-right" scale={4} active={scale === 4} doubled />
            </View>
        </View>
    )
}

export function InfoPanel() {
    return (
        <Panel className="w-80 p-4">
            <Label className="text-yellow-500 font-semibold text-lg mb-4">Information</Label>

            <View className="mb-6">
                <Label className="text-[#4AC6D7] font-medium mb-2 text-sm max-w-full" style={wrappedTextStyle}>Reminder: Line of Sight Tool</Label>
                <View className="flex flex-row flex-wrap items-center mb-1">
                    <Label className="text-xs text-slate-300 mr-1" style={wrappedTextStyle}>Toggle the</Label>
                    <Icon name="eye" className="w-3 h-3 mr-1" tint="#4AC6D7" />
                    <Label className="text-xs text-[#4AC6D7]" style={wrappedTextStyle}>LoS Tool button on!</Label>
                </View>
                <Label className="text-xs text-slate-300 leading-relaxed max-w-full" style={wrappedTextStyle}>
                    It displays the current field of view of the selected friendly unit. You can also use it freely by
                    hovering over the battlefield while maintaining the "C" key.
                </Label>
            </View>

            <View>
                <Label className="text-[#4AC6D7] font-medium mb-2 text-sm max-w-full" style={wrappedTextStyle}>Reminder: Stealth Display Tool</Label>
                <Label className="text-xs text-slate-300 leading-relaxed max-w-full" style={wrappedTextStyle}>
                    Compare unit detection ranges against enemy stealth and terrain to make better positioning
                    decisions.
                </Label>
            </View>
        </Panel>
    )
}

export function ObjectivesPanel() {
    return (
        <Panel className="w-64 p-3 bg-[#183545]/70 border-0">
            <Label className="text-white text-sm font-medium tracking-wide mb-2">OBJECTIVES</Label>
            <View className="flex flex-row items-start">
                <View className="w-3 h-3 border-2 border-slate-400 bg-slate-900/50 mt-1 mr-2" />
                <View className="flex flex-col flex-1 min-w-0">
                    <Label className="text-white text-sm font-medium max-w-full" style={wrappedTextStyle}>Find a way</Label>
                    <Label className="text-slate-400 text-xs italic max-w-full" style={wrappedTextStyle}>Road to Friedewald</Label>
                </View>
            </View>
        </Panel>
    )
}

function RuleButton({
    icon,
    label,
    active,
    wide = false,
}: {
    icon: IconName
    label: string
    active: boolean
    wide?: boolean
}) {
    return (
        <View
            onClick={() => getHudBridge()?.SetEngagementRule(label)}
            className={`${wide ? "w-full" : "w-[113px]"} h-5 px-2 mb-1 flex flex-row items-center border-2 rounded-sm ${
                active
                    ? "bg-[#2A6B84] border-cyan-300 text-white"
                    : "bg-[#122834] border-cyan-700 text-[#4AC6D7]"
            }`}
        >
            <Icon name={icon} className="w-3.5 h-3.5 mr-1.5" tint={active ? "#FFFFFF" : "#4AC6D7"} />
            <Label className="text-[10px] font-bold max-w-full" style={wrappedTextStyle}>{label}</Label>
        </View>
    )
}

export function UnitPanel() {
    const name = useFrameSync(() => getHudBridge()?.SelectedUnitName ?? "NO UNIT SELECTED")
    const status = useFrameSync(() => getHudBridge()?.SelectedUnitStatus ?? "STANDBY")
    const weapon = useFrameSync(() => getHudBridge()?.WeaponName ?? "NO WEAPON DATA")
    const selected = useFrameSync(() => getHudBridge()?.SelectedUnitCount ?? 0)
    const playerUnits = useFrameSync(() => getHudBridge()?.PlayerUnitCount ?? 0)
    const activeRule = useFrameSync(() => getHudBridge()?.ActiveRule ?? "FIRE AT WILL")

    return (
        <Panel className="flex flex-row h-[104px] bg-[#183545]/95 rounded-md rounded-b-none border-b-0 overflow-hidden">
            <View className="w-56 bg-[#183545] flex flex-col p-2 border-r-2 border-cyan-700">
                <View className="flex flex-row items-center mb-1">
                    <Icon name="activity" className="w-4 h-4 mr-1.5" tint="#CBD5E1" />
                    <Label className="text-white font-bold text-sm max-w-full" style={wrappedTextStyle}>{name}</Label>
                </View>
                <View className="flex flex-row justify-between items-center bg-[#10222E] px-2 py-1 mb-1 rounded-sm border-2 border-cyan-800">
                    <Label className="text-slate-200 text-xs font-medium max-w-full" style={wrappedTextStyle}>
                        {selected > 1 ? `${selected} UNITS SELECTED` : selected === 1 ? "PRIMARY SELECTION" : "NO SELECTION"}
                    </Label>
                    <Icon name="chevron-up" className="w-3 h-3" tint="#94A3B8" />
                </View>
                <View className="flex flex-row justify-between items-center mt-auto px-1">
                    <Label className="text-[10px] text-slate-400 font-medium">READINESS</Label>
                    <Label className="bg-yellow-600 text-white text-[10px] font-bold px-2 rounded-sm">{status}</Label>
                </View>
                <Label
                    text={`Selected: <color=#FFFFFF>${selected}</color> / ${playerUnits}`}
                    className="text-[10px] text-slate-400 text-center border-t-2 border-cyan-800 pt-1 mt-1"
                />
            </View>

            <View className="w-64 px-3 py-2 border-r-2 border-cyan-700 bg-[#183545]">
                <Label className="text-[10px] text-white font-bold mb-1 text-center tracking-wider">ENGAGEMENT RULES</Label>
                <View className="flex flex-row justify-between">
                    <RuleButton icon="crosshair" label="HUNT" active={activeRule === "HUNT"} />
                    <RuleButton icon="activity" label="FAST" active={activeRule === "FAST"} />
                </View>
                <View className="flex flex-row justify-between">
                    <RuleButton icon="target" label="COVER" active={activeRule === "COVER"} />
                    <RuleButton icon="target" label="DEFENSIVE" active={activeRule === "DEFENSIVE"} />
                </View>
                <RuleButton icon="crosshair" label="FIRE AT WILL" active={activeRule === "FIRE AT WILL"} wide />
            </View>

            <View className="w-56 px-3 py-2 flex flex-col items-center bg-[#183545]">
                <Label className="text-[10px] text-white font-bold mb-2 tracking-wider text-center max-w-full" style={wrappedTextStyle}>{weaponHeading(weapon)}</Label>
                <View className="w-32 h-6 bg-[#0F1F2A] mb-1 flex flex-row items-center justify-center border-2 border-cyan-800 rounded-sm">
                    <View className="w-20 h-1.5 bg-[#4AC6D7] rounded-sm relative">
                        <View className="absolute left-4 top-[-2px] w-4 h-2.5 bg-[#4AC6D7] rounded-sm" />
                        <View className="absolute right-2 top-[1px] w-2 h-1 bg-slate-800" />
                    </View>
                </View>
                <Label className="text-sm text-[#4AC6D7] font-bold text-center max-w-full" style={wrappedTextStyle}>{weapon}</Label>
                <Label className="text-green-500 font-bold text-[10px] tracking-wider">{status}</Label>
                <Label
                    text="WEAPON SYSTEM <color=#FFFFFF>ONLINE</color>"
                    className="text-[10px] text-slate-400 mt-auto"
                />
            </View>
        </Panel>
    )
}

function OrderButton({ label, disabled = false, className = "" }: { label: string; disabled?: boolean; className?: string }) {
    return (
        <Button
            text={label.toUpperCase()}
            disabled={disabled}
            onClick={() => getHudBridge()?.IssueOrder(label)}
            style={{ whiteSpace: "normal", unityTextAlign: "middle-center" }}
            className={`h-7 py-0 border-2 rounded-sm text-[11px] font-bold ${
                disabled
                    ? "bg-[#152733]/80 border-slate-800 text-[#3A5D72]"
                    : "bg-[#122834] border-cyan-700 text-[#4AC6D7]"
            } ${className}`}
        />
    )
}

function OrderColumn({ title, children, className = "" }: { title: string; children: ReactNode; className?: string }) {
    return (
        <View className={`w-[104px] flex flex-col ${className}`}>
            <Label className="text-[10px] text-white/80 font-bold tracking-wider text-center bg-[#0F1F2A] py-0.5 mb-1 border-2 border-cyan-800 rounded-sm">
                {title}
            </Label>
            {children}
        </View>
    )
}

export function OrdersPanel() {
    const hasSelection = useFrameSync(() => (getHudBridge()?.SelectedUnitCount ?? 0) > 0)

    return (
        <Panel className="p-2 w-[340px] bg-[#183545]/95">
            <Label className="text-[10px] text-white font-bold mb-2 tracking-wider px-1">SMART ORDERS FOR GROUPS</Label>
            <View className="flex flex-row justify-between mb-2">
                <OrderButton label="Seize" className="w-[158px]" />
                <OrderButton label="Hold Position" className="w-[158px]" />
            </View>
            <View className="flex flex-row justify-between">
                <OrderColumn title="MOVEMENT">
                    <OrderButton label="Stop" disabled={!hasSelection} className="mb-1.5" />
                    <OrderButton label="Move" className="mb-1.5" />
                    <OrderButton label="Move Fast" className="mb-1.5" />
                    <OrderButton label="Reverse" />
                </OrderColumn>
                <OrderColumn title="COMBAT">
                    <OrderButton label="Fire Pos" className="mb-1.5" />
                    <OrderButton label="Hunt" className="mb-1.5" />
                    <OrderButton label="Quick Hunt" className="mb-1.5" />
                    <OrderButton label="Return Fire" />
                </OrderColumn>
                <OrderColumn title="SPECIAL">
                    <OrderButton label="Unload At Position" disabled className="mb-1.5" />
                    <OrderButton label="Unload" disabled />
                </OrderColumn>
            </View>
        </Panel>
    )
}

function UtilityButton({ icon, onClick, last = false }: { icon: IconName; onClick?: () => void; last?: boolean }) {
    return (
        <View
            onClick={onClick}
            className={`w-9 h-9 flex flex-row items-center justify-center bg-[#183545]/95 ${
                last ? "" : "border-r-2 border-cyan-700"
            }`}
        >
            <Icon name={icon} className="w-5 h-5" tint="#4AC6D7" />
        </View>
    )
}

export function MiniMapControls() {
    return (
        <View className="flex flex-row bg-[#183545]/95 border-2 border-cyan-700 rounded-sm overflow-hidden">
            <UtilityButton icon="clipboard-list" />
            <UtilityButton icon="target" />
            <UtilityButton icon="eye" />
            <UtilityButton icon="monitor" />
            <UtilityButton icon="settings" onClick={() => getHudBridge()?.ClearSelection()} last />
        </View>
    )
}

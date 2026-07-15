export interface HudBridge {
    Credits: number
    PlayerUnitCount: number
    SelectedUnitCount: number
    PlayerSide: string
    SelectedUnitName: string
    SelectedUnitStatus: string
    WeaponName: string
    ElapsedSeconds: number
    TimeScale: number
    IsPaused: boolean
    ActiveRule: string
    LastOrder: string

    SetTimeScale(value: number): void
    SetEngagementRule(rule: string): void
    IssueOrder(order: string): void
    StopSelectedUnits(): void
    ClearSelection(): void
}

declare global {
    // Registered by the JSRunner on the WarnoHUD GameObject.
    // eslint-disable-next-line no-var
    var hudBridge: HudBridge | undefined
}

export function getHudBridge(): HudBridge | undefined {
    return (globalThis as { hudBridge?: HudBridge }).hudBridge
}

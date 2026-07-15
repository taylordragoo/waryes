import { View } from "onejs-react"
import {
    InfoPanel,
    MiniMapControls,
    ObjectivesPanel,
    OrdersPanel,
    TimeControls,
    TopBar,
    UnitPanel,
} from "./components/Panels"

export default function App() {
    return (
        <View
            className="relative w-screen h-screen overflow-hidden bg-transparent select-none text-slate-200"
            pickingMode="Ignore"
        >
            <View className="absolute top-0 left-0 right-0 flex flex-row justify-center" pickingMode="Ignore">
                <TopBar />
            </View>

            <View className="absolute top-2 right-4" pickingMode="Ignore">
                <TimeControls />
            </View>

            <View className="absolute top-12 left-4" pickingMode="Ignore">
                <InfoPanel />
            </View>

            <View className="absolute top-20 right-4" pickingMode="Ignore">
                <ObjectivesPanel />
            </View>

            <View className="absolute bottom-0 left-0 right-0 flex flex-row justify-center" pickingMode="Ignore">
                <UnitPanel />
            </View>

            <View className="absolute right-4 bottom-4 flex flex-col items-end" pickingMode="Ignore">
                <OrdersPanel />
                <View className="mt-1.5">
                    <MiniMapControls />
                </View>
            </View>
        </View>
    )
}

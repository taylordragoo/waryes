/**
 * @license
 * SPDX-License-Identifier: Apache-2.0
 */

import { TopBar, TimeControls, InfoPanel, ObjectivesPanel, UnitPanel, OrdersPanel, MiniMapControls } from './components/Panels';

export default function App() {
  return (
    <div className="relative w-screen h-screen overflow-hidden bg-slate-900 select-none font-sans">
      {/* Background Simulation */}
      <div className="absolute inset-0 z-0">
        <div className="absolute inset-0 bg-[#2a4536] opacity-70" />
        <div 
          className="w-full h-full"
          style={{
            backgroundImage: 'radial-gradient(#4a7558 1px, transparent 1px)',
            backgroundSize: '40px 40px'
          }}
        />
        {/* Subtle vignette/gradient to make UI pop */}
        <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-transparent via-slate-900/40 to-slate-900/80 pointer-events-none" />
      </div>

      {/* Top Bar - Centered */}
      <div className="absolute top-0 left-1/2 -translate-x-1/2 z-10">
        <TopBar />
      </div>

      {/* Top Right - Time & Controls */}
      <div className="absolute top-2 right-4 z-10">
        <TimeControls />
      </div>

      {/* Top Left - Info Panel */}
      <div className="absolute top-12 left-4 z-10">
        <InfoPanel />
      </div>

      {/* Top Right below Time - Objectives */}
      <div className="absolute top-20 right-4 z-10">
        <ObjectivesPanel />
      </div>

      {/* Bottom Layout Container */}
      <div className="absolute bottom-0 inset-x-0 z-10 flex items-end pointer-events-none">
        
        {/* Center - Unit Info */}
        <div className="absolute left-1/2 -translate-x-1/2 bottom-0 pointer-events-auto">
          <UnitPanel />
        </div>

        {/* Right - Orders & Controls */}
        <div className="absolute right-0 bottom-0 pointer-events-auto flex flex-col items-end gap-1.5 p-4 pt-0">
          <OrdersPanel />
          <MiniMapControls />
        </div>
      </div>
    </div>
  );
}

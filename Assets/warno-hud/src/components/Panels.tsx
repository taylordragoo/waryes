import { ReactNode } from 'react';
import {
  Star,
  Circle,
  MapPin,
  Pause,
  Play,
  FastForward,
  ChevronRight,
  Eye,
  Settings,
  Monitor,
  ClipboardList,
  Target,
  ChevronUp,
  Activity,
  Crosshair
} from 'lucide-react';

const Panel = ({ children, className = '' }: { children: ReactNode; className?: string }) => (
  <div className={`bg-[#183545]/85 backdrop-blur-md border border-[#2a6b84] rounded-sm text-slate-200 ${className}`}>
    {children}
  </div>
);

export const TopBar = () => (
  <div className="flex bg-[#183545]/90 border-b border-x border-[#2a6b84] rounded-b-md px-6 py-1 gap-8 text-sm font-bold shadow-lg">
    <div className="flex items-center gap-2 text-yellow-500">
      <Star className="w-4 h-4 fill-current" />
      <span>---</span>
    </div>
    <div className="flex items-center gap-2 text-[#4ac6d7]">
      <Circle className="w-4 h-4" />
      <span>---</span>
    </div>
    <div className="flex items-center gap-2">
      <MapPin className="w-4 h-4 text-slate-400" />
      <div className="flex gap-1">
        <span className="bg-[#2a4570] px-2 rounded-sm text-white">0</span>
        <span className="bg-[#4a555a] px-2 rounded-sm text-white">0</span>
        <span className="bg-[#7a2525] px-2 rounded-sm text-white">0</span>
      </div>
    </div>
  </div>
);

export const TimeControls = () => (
  <div className="flex flex-col items-end gap-1">
    <div className="text-right text-white">
      <div className="text-lg leading-none font-medium text-shadow">05:42</div>
      <div className="text-[10px] text-[#4ac6d7] tracking-wider font-bold">STANDARD SPEED</div>
    </div>
    <div className="flex gap-1 mt-1">
      {[Pause, Play, ChevronRight, FastForward].map((Icon, i) => (
        <button key={i} className="bg-[#2a6b84]/50 hover:bg-[#2a6b84] border border-[#4ac6d7]/30 p-1 rounded-sm transition-colors">
          <Icon className="w-4 h-4 text-[#4ac6d7]" />
        </button>
      ))}
      <button className="bg-[#2a6b84]/50 hover:bg-[#2a6b84] border border-[#4ac6d7]/30 p-1 rounded-sm transition-colors flex items-center justify-center">
        <ChevronRight className="w-4 h-4 text-[#4ac6d7]" />
        <ChevronRight className="w-4 h-4 text-[#4ac6d7] -ml-2" />
      </button>
    </div>
  </div>
);

export const InfoPanel = () => (
  <Panel className="w-80 p-4 shadow-xl">
    <h2 className="text-yellow-500 font-semibold text-lg mb-4">Information</h2>
    
    <div className="mb-6">
      <h3 className="text-[#4ac6d7] font-medium mb-2 text-sm">Reminder: Line of Sight Tool</h3>
      <p className="text-xs text-slate-300 leading-relaxed">
        Toggle the <Eye className="inline w-3 h-3" /> <span className="text-[#4ac6d7]">LoS Tool button</span> on! It will display the <span className="text-[#4ac6d7]">current field of view</span> of the currently selected <span className="text-green-400">friendly unit</span>.<br/>
        You can also <span className="text-[#4ac6d7]">use it freely</span> by hovering your mouse over the battlefield while <span className="text-[#4ac6d7]">maintaining the "C" key</span>.
      </p>
    </div>

    <div>
      <h3 className="text-[#4ac6d7] font-medium mb-2 text-sm">Reminder : Stealth Display Tool</h3>
      <p className="text-xs text-slate-300 leading-relaxed">
        This tool helps you take better strategic decisions by showing you the <span className="text-[#4ac6d7]">detection ranges of your units</span> in function of the enemy's <span className="text-[#4ac6d7]">stealth</span> level and <span className="text-green-400">terrain</span>.
      </p>
    </div>
  </Panel>
);

export const ObjectivesPanel = () => (
  <Panel className="w-64 p-3 bg-[#183545]/70 border-none shadow-lg">
    <h3 className="text-white text-sm font-medium tracking-wide mb-2 uppercase">Objectives</h3>
    <div className="flex items-start gap-2">
      <div className="w-3 h-3 border border-slate-400 bg-slate-900/50 mt-1 flex-shrink-0" />
      <div>
        <div className="text-white text-sm font-medium">Find a way</div>
        <div className="text-slate-400 text-xs italic">Road to Friedewald</div>
      </div>
    </div>
  </Panel>
);

const UnitButton = ({ icon: Icon, label, active = false }: { icon: any, label: string, active?: boolean }) => (
  <button className={`flex items-center gap-1.5 px-2 py-1.5 text-[10px] font-bold border rounded-sm transition-colors ${
    active ? 'bg-[#2a6b84] border-[#4ac6d7] text-white' : 'bg-[#122834] border-[#2a6b84] hover:bg-[#1a3848] text-[#4ac6d7]'
  }`}>
    <Icon className="w-3.5 h-3.5" />
    <span className="uppercase">{label}</span>
  </button>
);

export const UnitPanel = () => (
  <Panel className="flex p-0 shadow-2xl overflow-hidden rounded-md border-b-0 rounded-b-none border-[#2a6b84] h-[104px]">
    {/* Left Column - Unit Status */}
    <div className="w-56 bg-[#183545] flex flex-col p-2 border-r border-[#2a6b84]">
      <div className="flex items-center gap-1.5 text-white font-bold text-sm mb-1">
        <Activity className="w-4 h-4 text-slate-300" />
        <span>M1025 HUMVEE M2HB</span>
      </div>
      <div className="flex justify-between items-center bg-[#10222e] px-2 py-1 mb-2 rounded-sm border border-[#2a6b84]/50 cursor-pointer hover:bg-[#152733]">
        <span className="text-slate-200 text-xs font-medium">Grimes</span>
        <ChevronUp className="w-3 h-3 text-slate-400" />
      </div>
      <div className="flex items-center justify-between mt-auto mb-1 px-1">
        <span className="text-[10px] text-slate-400 font-medium">COHESION</span>
        <span className="bg-yellow-600 text-white text-[10px] font-bold px-2 rounded-sm">HIGH</span>
      </div>
      <div className="text-[10px] text-slate-400 text-center border-t border-[#2a6b84]/50 pt-1 mt-1">
        Fuel: <span className="text-white font-medium">92</span> / 100
      </div>
    </div>

    {/* Middle Column - Rules */}
    <div className="w-64 px-3 py-2 border-r border-[#2a6b84] bg-[#183545]">
      <div className="text-[10px] text-white font-bold mb-2 text-center tracking-wider">ENGAGEMENT RULES</div>
      <div className="grid grid-cols-2 gap-1">
        <UnitButton icon={Crosshair} label="Hunt" />
        <UnitButton icon={Activity} label="Fast" />
        <UnitButton icon={Target} label="Cover" />
        <UnitButton icon={Target} label="Defensive" />
        <div className="col-span-2">
          <UnitButton icon={Crosshair} label="Fire at will" active />
        </div>
      </div>
    </div>

    {/* Right Column - Weapon */}
    <div className="w-56 px-3 py-2 flex flex-col items-center bg-[#183545]">
      <div className="text-[10px] text-white font-bold mb-2 tracking-wider">HEAVY MACHINE GUN</div>
      <div className="w-32 h-6 bg-[#0f1f2a] mb-2 flex items-center justify-center border border-[#2a6b84]/50 rounded-sm">
        {/* Mock Weapon Graphic - Just a shape to represent a gun silhouette */}
        <div className="w-20 h-1.5 bg-[#4ac6d7] rounded-sm relative">
           <div className="absolute top-1/2 -translate-y-1/2 left-4 w-4 h-2.5 bg-[#4ac6d7] rounded-sm"></div>
           <div className="absolute top-1/2 -translate-y-1/2 right-2 w-2 h-1 bg-slate-800"></div>
        </div>
      </div>
      <div className="text-sm text-[#4ac6d7] font-bold mb-0.5">M2HB</div>
      <div className="text-green-500 font-bold text-[10px] tracking-wider mb-1">READY</div>
      <div className="text-[10px] text-slate-400 mt-auto">
        12.7mm <span className="text-white font-medium">(2000 / 2000)</span>
      </div>
    </div>
  </Panel>
);

const OrderBtn = ({ label, disabled = false }: { label: string, disabled?: boolean }) => (
  <button 
    disabled={disabled}
    className={`w-full py-1 text-[11px] font-bold border rounded-sm transition-colors uppercase h-7 ${
      disabled 
        ? 'bg-[#152733]/80 border-[#1e3b4d] text-[#3a5d72] cursor-not-allowed' 
        : 'bg-[#122834] hover:bg-[#1d4154] border-[#2a6b84] text-[#4ac6d7] hover:text-white'
    }`}
  >
    {label}
  </button>
);

export const OrdersPanel = () => (
  <Panel className="p-2 w-[340px] shadow-2xl bg-[#183545]/95">
    <div className="text-[10px] text-white font-bold mb-2 tracking-wider uppercase px-1">Smart Orders for Groups</div>
    <div className="grid grid-cols-2 gap-1.5 mb-2">
      <OrderBtn label="Seize" />
      <OrderBtn label="Hold Position" />
    </div>
    <div className="grid grid-cols-3 gap-1.5">
      {/* MOVEMENT */}
      <div className="flex flex-col gap-1.5">
        <div className="text-[10px] text-white/80 font-bold tracking-wider text-center mb-0.5 bg-[#0f1f2a] py-0.5 border border-[#2a6b84]/50 rounded-sm">MOVEMENT</div>
        <OrderBtn label="Stop" disabled />
        <OrderBtn label="Move" />
        <OrderBtn label="Move Fast" />
        <OrderBtn label="Reverse" />
      </div>
      {/* COMBAT */}
      <div className="flex flex-col gap-1.5">
        <div className="text-[10px] text-white/80 font-bold tracking-wider text-center mb-0.5 bg-[#0f1f2a] py-0.5 border border-[#2a6b84]/50 rounded-sm">COMBAT</div>
        <OrderBtn label="Fire Pos" />
        <OrderBtn label="Hunt" />
        <OrderBtn label="Quick Hunt" />
        <OrderBtn label="Return Fire" />
      </div>
      {/* SPECIAL */}
      <div className="flex flex-col gap-1.5">
        <div className="text-[10px] text-white/80 font-bold tracking-wider text-center mb-0.5 bg-[#0f1f2a] py-0.5 border border-[#2a6b84]/50 rounded-sm">SPECIAL</div>
        <OrderBtn label="Unload At Position" disabled />
        <OrderBtn label="Unload" disabled />
      </div>
    </div>
  </Panel>
);

export const MiniMapControls = () => (
  <div className="flex bg-[#183545]/95 border border-[#2a6b84] rounded-sm shadow-xl w-fit">
    {[ClipboardList, Target, Eye, Monitor, Settings].map((Icon, i) => (
      <button key={i} className="p-2 hover:bg-[#1d4154] text-[#4ac6d7] hover:text-white border-r border-[#2a6b84] last:border-r-0 transition-colors">
        <Icon className="w-5 h-5" />
      </button>
    ))}
  </div>
);

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        MyCommandLine commandLine = new MyCommandLine();
        Dictionary<string, Action> commands = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase);

        public Program()
        {
            commands["cycle"] = Cycle;
            commands["enable"] = Enable;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (commandLine.TryParse(argument))
            {
                Action action;
                string command = commandLine.Argument(0);

                if (command == null)
                {
                    Echo("No command specified");
                }
                else if (commands.TryGetValue(command, out action))
                {
                    action();
                }

            }
        }

        public void Cycle()
        {
            string command = commandLine.Argument(1);

            if (string.Equals(command, "landing gear", StringComparison.OrdinalIgnoreCase))
            {
                
            }
        }

        public void Enable()
        {
            string command = commandLine.Argument(1);
            IMyCockpit cockpit = GridTerminalSystem.GetBlockWithName("Fighter Cockpit") as IMyCockpit;

            if (string.Equals(command, "proximity sensor", StringComparison.OrdinalIgnoreCase))
            {
                EnableProximityWarning(cockpit);
            }
            else if (string.Equals(command, "altimeter", StringComparison.OrdinalIgnoreCase))
            {
                EnableAltitude(cockpit);
            }
        }

        public void EnableAltitude(IMyCockpit cockpit)
        {
            IMyTextSurface lcd = cockpit.GetSurface(0);
        }

        public void EnableProximityWarning(IMyCockpit cockpit)
        {
            IMyTextSurface lcd = cockpit.GetSurface(0);
        }
    }
}

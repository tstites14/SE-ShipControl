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
                CycleLandingGear();
            }
        }

        public void CycleLandingGear()
        {
            List<IMyPistonBase> pistons = new List<IMyPistonBase>();
            List<IMyMotorStator> hinges = new List<IMyMotorStator>();

            IMyBlockGroup pistonGroup = GridTerminalSystem.GetBlockGroupWithName("Landing Gear Pistons");
            if (pistonGroup == null)
            {
                Echo("Could not find the piston group");
            }
            IMyBlockGroup hingeGroup = GridTerminalSystem.GetBlockGroupWithName("Landing Gear Hinges");
            if (hingeGroup == null)
            {
                Echo("Could not find the hinge group");
            }

            pistonGroup.GetBlocksOfType(pistons);
            hingeGroup.GetBlocksOfType(hinges);


        }

        public void Enable()
        {
            string command = commandLine.Argument(1);
            IMyCockpit cockpit = GridTerminalSystem.GetBlockWithName("Fighter Cockpit") as IMyCockpit;
            IMyCameraBlock camera = GridTerminalSystem.GetBlockWithName("Proximity Camera") as IMyCameraBlock;

            if (string.Equals(command, "proximity sensor", StringComparison.OrdinalIgnoreCase))
            {
                EnableProximityWarning(cockpit, camera);
            }
            else if (string.Equals(command, "altimeter", StringComparison.OrdinalIgnoreCase))
            {
                EnableAltitude(cockpit, camera);
            }
        }

        public void EnableAltitude(IMyCockpit cockpit, IMyCameraBlock camera)
        {
            IMyTextSurface lcd = cockpit.GetSurface(0);

            double altitude = GetAltitude(camera);
        }

        private double GetAltitude(IMyCameraBlock camera)
        {
            MyDetectedEntityInfo detection = camera.Raycast(500);
            Vector3 cameraPosition = camera.GetPosition();
            Vector3D? raycastPosition;

            if (detection.HitPosition != null)
            {
                raycastPosition = detection.HitPosition;
            }

            return double.MaxValue;
        }

        public void EnableProximityWarning(IMyCockpit cockpit, IMyCameraBlock camera)
        {
            IMyTextSurface lcd = cockpit.GetSurface(0);


        }
    }
}

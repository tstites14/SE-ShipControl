﻿using Sandbox.Game.EntityComponents;
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

        bool enableAltitude = false;
        bool enableProximity = false;

        IMyCockpit cockpit;
        IMyCameraBlock camera;

        IMyTextSurface proximityLCD;
        IMyTextSurface altitudeLCD;

        public Program()
        {
            commands["cycle"] = Cycle;
            commands["enable"] = Enable;

            cockpit = GridTerminalSystem.GetBlockWithName("Fighter Cockpit") as IMyCockpit;
            camera = GridTerminalSystem.GetBlockWithName("Proximity Camera") as IMyCameraBlock;

            proximityLCD = cockpit.GetSurface(2);
            altitudeLCD = cockpit.GetSurface(3);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
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

            if (enableProximity)
            {
                SetUpProximity();
            }
            if (enableAltitude)
            {
                SetUpAltitude();
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

            if (string.Equals(command, "proximity sensor", StringComparison.OrdinalIgnoreCase))
            {
                EnableProximityWarning();
            }
            else if (string.Equals(command, "altimeter", StringComparison.OrdinalIgnoreCase))
            {
                EnableAltitude();
            }
        }

        public void EnableAltitude()
        {
            enableAltitude = true;
        }

        private void SetUpAltitude()
        {
            double altitude = GetAltitude(camera);
        }

        private double GetAltitude(IMyCameraBlock camera)
        {
            MyDetectedEntityInfo detection = camera.Raycast(500);
            Vector3D cameraPosition = camera.GetPosition();
            Vector3D raycastPosition = new Vector3D();

            if (detection.HitPosition != null)
            {
                raycastPosition = (Vector3D)detection.HitPosition;

                Vector3D diff = cameraPosition - raycastPosition;
            }

            return double.MaxValue;
        }

        public void EnableProximityWarning()
        {
            enableProximity = true;
        }

        private void SetUpProximity()
        {

        }
    }
}

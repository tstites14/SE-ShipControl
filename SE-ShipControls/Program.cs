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

        bool firstRun = true;
        bool enableAltitude = false;
        bool enableProximity = false;

        GearState gearState;

        IMyCockpit cockpit;
        IMyCameraBlock camera;

        IMyTextSurface proximityLCD;
        IMyTextSurface altitudeLCD;

        List<IMyMotorStator> hinges;
        List<IMyPistonBase> pistons;

        public Program()
        {
            commands["cycle"] = Cycle;
            commands["enable"] = Enable;

            cockpit = GridTerminalSystem.GetBlockWithName("Fighter Cockpit") as IMyCockpit;
            camera = GridTerminalSystem.GetBlockWithName("Proximity Camera") as IMyCameraBlock;

            proximityLCD = cockpit.GetSurface(2);
            altitudeLCD = cockpit.GetSurface(3);

            hinges = new List<IMyMotorStator>();
            pistons = new List<IMyPistonBase>();
            GridTerminalSystem.GetBlockGroupWithName("Landing Gear Hinges").GetBlocksOfType(hinges);
            GridTerminalSystem.GetBlockGroupWithName("Landing Gear Pistons").GetBlocksOfType(pistons);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
            Storage = string.Format("{0};{1};{2};{3}", enableAltitude.ToString(), enableProximity.ToString(), firstRun.ToString(), gearState);
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

            if (Storage != "")
            {
                string[] entries = Storage.Split(';');
                bool.TryParse(entries[0], out enableAltitude);
                bool.TryParse(entries[1], out enableProximity);
                bool.TryParse(entries[2], out firstRun);

                Enum.TryParse(entries[3], out gearState);
            }

            Echo("Altitude: " + enableAltitude.ToString());
            Echo("Proximity: " + enableProximity.ToString());

            if (enableProximity)
            {
                SetUpProximity();
            }
            if (enableAltitude)
            {
                SetUpAltitude();
            }

            if (firstRun)
            {
                //Calibrate gearState in case gear has never been deployed before
                List<IMyLandingGear> landingGear = new List<IMyLandingGear>();
                GridTerminalSystem.GetBlocksOfType(landingGear);

                CalibrateGearState(landingGear);
                firstRun = false;
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
            //TODO: Set gearState
        }

        private void CalibrateGearState(List<IMyLandingGear> gear)
        {
            bool gearLocked = gear.Any(item =>
            {
                return item.LockMode == LandingGearMode.Locked;
            });
            bool pistonsExtended = pistons.Any(item =>
            {
                return item.CurrentPosition == item.HighestPosition;
            });
            bool isTransitioning = pistons.Any(item =>
            {
                return item.CurrentPosition != item.LowestPosition &&
                    item.CurrentPosition != item.HighestPosition;
            });

            if (gearLocked)
            {
                gearState = GearState.Open;
            }
            else if (!pistonsExtended)
            {
                gearState = GearState.Closed;
            }
            else if (isTransitioning)
            {
                gearState = GearState.Transition;
            }
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

            SetUpAltitude();
        }

        private void SetUpAltitude()
        {
            float altitude = Convert.ToSingle(GetAltitude(camera));

            RectangleF view = new RectangleF((altitudeLCD.TextureSize - altitudeLCD.SurfaceSize) / 2F, altitudeLCD.SurfaceSize);
            var frame = altitudeLCD.DrawFrame();

            var altitudeSprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = string.Format("{0:F1}", altitude),
                RotationOrScale = 1,
                Color = Color.White,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            };

            frame.Add(altitudeSprite);

            frame.Dispose();
        }

        private double GetAltitude(IMyCameraBlock camera)
        {
            camera.EnableRaycast = true;
            MyDetectedEntityInfo detection = camera.Raycast(500);
            Vector3D cameraPosition = camera.GetPosition();
            Vector3D raycastPosition;

            if (detection.HitPosition != null)
            {
                raycastPosition = (Vector3D)detection.HitPosition;

                Vector3D diff = cameraPosition - raycastPosition;

                return diff.Length() - 0.3;
            }

            return double.MaxValue;
        }

        public void EnableProximityWarning()
        {
            enableProximity = true;

            SetUpProximity();
        }

        private void SetUpProximity()
        {
            float altitude = Convert.ToSingle(GetAltitude(camera));

            if (altitude <= 15 && gearState != GearState.Open)
            {
                proximityLCD.BackgroundColor = Color.Red;
            }
            else
            {
                //proximityLCD.BackgroundColor = 
            }
        }

        private enum GearState
        {
            Open,
            Transition,
            Closed
        }
    }
}

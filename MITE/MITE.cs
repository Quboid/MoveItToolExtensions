using ColossalFramework.IO;
using Harmony;
using ICities;
using MoveIt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MITE
{
    public class MITE : LoadingExtensionBase, IUserMod
    {
        public string Name => "Move It Tool Extensions";
        public string Description => "Extends Move It with extra tools and filters";

        public const MoveItTool.ToolState TOOL_KEY = (MoveItTool.ToolState)6;
        public const int TOOL_ACTION_DO = 1;
        public const int UI_FILTER_CB_HEIGHT = 25;

        public static bool filterSurfaces = true;
        public static bool filterNetworks = false;
        public static Dictionary<string, NetworkFilter> NetworkFilters = new Dictionary<string, NetworkFilter>
        {   
            { "NF-Roads", new NetworkFilter(true, new List<Type> { typeof(RoadBaseAI) } ) },
            { "NF-Tracks", new NetworkFilter(true, new List<Type> { typeof(TrainTrackBaseAI) } ) },
            { "NF-Paths", new NetworkFilter(true, new List<Type> { typeof(PedestrianPathAI), typeof(PedestrianTunnelAI), typeof(PedestrianBridgeAI), typeof(PedestrianWayAI) } ) },
            { "NF-Fences", new NetworkFilter(true, new List<Type> { typeof(DecorationWallAI) } ) },
            { "NF-Power", new NetworkFilter(true, new List<Type> { typeof(PowerLineAI) } ) },
            { "NF-Other", new NetworkFilter(true,  null ) }
        };

        public enum AlignModes { Off, Height, Individual, Group, Random };
        public static AlignModes AlignMode = AlignModes.Off;

        private static bool debugInitialised = false;
        public static readonly string debugPath = Path.Combine(DataLocation.localApplicationData, "MITE.log");

        private static readonly string harmonyId = "quboid.csl_mods.csl_mite";
        private static HarmonyInstance harmonyInstance;
        private static readonly object padlock = new object();


        public override void OnLevelLoaded(LoadMode loadMode)
        {
            HarmonyInstance harmony = GetHarmonyInstance();
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }


        public static void DebugLine(string line, bool newLine = true)
        {
            if (!debugInitialised)
            {
                File.WriteAllText(debugPath, $"MITE debug log:\n");
                debugInitialised = true;
            }

            Debug.Log(line);
            File.AppendAllText(debugPath, line);
            if (newLine)
            {
                File.AppendAllText(debugPath, "\n");
            }
        }


        public static bool DeactivateAlignTool(bool switchMode = true)
        {
            if (switchMode)
            {
                AlignMode = AlignModes.Off;
            }

            MoveItTool.instance.toolState = MoveItTool.ToolState.Default;
            UI.UpdateAlignTools();
            MoveIt.Action.UpdateArea(MoveIt.Action.GetTotalBounds(false));
            return false;
        }


        public static HarmonyInstance GetHarmonyInstance()
        {
            lock (padlock)
            {
                if (harmonyInstance == null)
                {
                    harmonyInstance = HarmonyInstance.Create(harmonyId);
                }

                return harmonyInstance;
            }
        }
    }

    /*
    public class MIAlignThreading : ThreadingExtensionBase
    {
        private bool _processed = false;
        private HashSet<InstanceState> m_states = new HashSet<InstanceState>();

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (ColossalFramework.Singleton<ToolController>.instance.CurrentTool is MoveItTool tool)
            {
                if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKey(KeyCode.A))
                {
                    if (_processed) return;
                    _processed = true;

                    // Action
                    if (MITE.AlignMode != MITE.AlignModes.Off)
                    { // Switch Off
                        MITE.DeactivateAlignTool();
                    }
                    else
                    {
                        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                        { // Both Control and Shift are being held
                            ;
                        }
                        else if (MoveIt.Action.selection.Count > 0)
                        {
                            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                            {
                                MITE.AlignMode = MITE.AlignModes.Group;
                            }
                            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            {
                                MITE.AlignMode = MITE.AlignModes.Random;

                                // Perform action immediately
                                AlignRotationAction action = new AlignRandomAction();
                                action.followTerrain = MoveItTool.followTerrain;
                                ActionQueue.instance.Push(action);
                                ActionQueue.instance.Do();
                                MITE.DeactivateAlignTool();
                            }
                            else
                            {
                                MITE.AlignMode = MITE.AlignModes.Individual;
                            }
                            if (tool.toolState != MoveItTool.ToolState.AligningHeights)
                            {
                                tool.StartAligningHeights();
                            }
                        }
                    }

                    //Debug.Log($"Active:{Mod.active} toolState:{tool.toolState}");
                }
                else
                {
                    _processed = false;
                }
            }
        }
    }*/
}

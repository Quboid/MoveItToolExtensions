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
        public const MoveItTool.ToolState Tool_Key = (MoveItTool.ToolState)6;
        public const int Tool_Action_Do = 1;
        public const int UI_Filter_CB_Height = 25;
        public const bool Enable_ModTools = true;

        public string Name => "Move It Tool Extensions";
        public string Description => "Extra tools and filters for Move It!";
        internal static readonly string settingsFilePath = Path.Combine(DataLocation.localApplicationData, "MITE.xml");

        public static bool filterSurfaces = true;
        public static bool filterNetworks = false;

        public enum AlignModes { Off, Height, Individual, Group, Random };
        public static AlignModes AlignMode = AlignModes.Off;

        private static bool debugInitialised = false;
        public static readonly string debugPath = Path.Combine(DataLocation.localApplicationData, "MITE.log");

        private static readonly string harmonyId = "quboid.csl_mods.mite";
        private static HarmonyInstance harmonyInstance;
        private static readonly object padlock = new object();

        public static StepOver StepOver;

        private static Settings m_settings;
        public static Settings Settings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = Settings.LoadConfiguration();

                    if (m_settings == null)
                    {
                        m_settings = new Settings();
                        m_settings.SaveConfiguration();
                    }
                }
                return m_settings;
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            Settings.OnSettingsUI(helper);
        }


        public override void OnLevelLoaded(LoadMode loadMode)
        {
            HarmonyInstance harmony = GetHarmonyInstance();
            StepOver = new StepOver();

            //BindingFlags flags = new BindingFlags();

            //var orig = typeof(MoveItTool).GetMethod("RaycastHoverInstance");
            //var prefix = typeof(MIT_RaycastHoverInstance).GetMethod("Prefix");
            //harmony.Patch(orig, new HarmonyMethod(prefix));

            //orig = typeof(MoveItTool).GetMethod("GetMarqueeList");
            //var postfix = typeof(MIT_GetMarqueeList).GetMethod("Postfix");
            //harmony.Patch(orig, null, new HarmonyMethod(postfix));

            //orig = typeof(MoveItTool).GetMethod("StopAligningHeights");
            //prefix = typeof(MIT_StopAligningHeights).GetMethod("Prefix");
            //harmony.Patch(orig, new HarmonyMethod(prefix));

            //orig = typeof(MoveItTool).GetMethod("StartAligningHeights");
            //postfix = typeof(MIT_StatAligningHeights).GetMethod("Postfix");
            //harmony.Patch(orig, null, new HarmonyMethod(postfix));

            //orig = typeof(MoveItTool).GetMethod("OnLeftClick");
            //prefix = typeof(MIT_OnLeftClick).GetMethod("Prefix");
            //harmony.Patch(orig, new HarmonyMethod(prefix));

            //orig = typeof(UIToolOptionPanel).GetMethod("Start");
            //postfix = typeof(UITOP_Start).GetMethod("Postfix");
            //harmony.Patch(orig, null, new HarmonyMethod(postfix));

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
    public class GridBeGoneThreading : ThreadingExtensionBase
    {
        private bool _processed = false;
        
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.N))
            {
                if (_processed) return;
                _processed = true;

                MITE.StepOver.Update();
                }
            else
            {
                _processed = false;
            }
        }
    }*/
}

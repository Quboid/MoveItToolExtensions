using ColossalFramework;
using Harmony;
using MoveIt;
using ColossalFramework.Math;
using ColossalFramework.UI;
using SamsamTS;
using System;
using System.Collections.Generic;
using UnityEngine;

// More detours in Moveable*.cs and Filter_Detours.cs

namespace MoveMore
{
    
    [HarmonyPatch(typeof(UIToolOptionPanel))]
    [HarmonyPatch("Start")]
    class UITOP_Start
    {
        private static UIButton marquee;
        private static Traverse _panel = null;


        private static Traverse _getPanel()
        {
            if (_panel == null)
            {
                _panel = Traverse.Create(marquee);
            }
            return _panel;
        }


        public static void Postfix(UIToolOptionPanel __instance, UIButton ___m_marquee)
        {
            UIPanel filtersPanel;
            marquee = ___m_marquee;
            __instance.RemoveUIComponent(__instance.filtersPanel);
            
            filtersPanel = __instance.filtersPanel = __instance.AddUIComponent(typeof(UIPanel)) as UIPanel;
            filtersPanel.atlas = SamsamTS.UIUtils.GetAtlas("Ingame");
            filtersPanel.backgroundSprite = "SubcategoriesPanel";
            filtersPanel.clipChildren = true;
            UI.FilterPanel = filtersPanel;

            filtersPanel.size = new Vector2(150, 140);
            filtersPanel.isVisible = false;

            void OnDoubleClick(UIComponent c, UIMouseEventParameter p)
            {
                foreach (UIComponent comp in filtersPanel.components)
                {
                    UICheckBox box = comp as UICheckBox;
                    if (box != null)
                    {
                        if (NetworkFilter.GetNames().Exists(n => n.Equals(box.name))) continue;
                        if (box != c) box.isChecked = false;
                    }
                }

                ((UICheckBox)c).isChecked = true;
            }

            void OnDoubleClickNetwork(UIComponent c, UIMouseEventParameter p)
            {
                foreach (UIComponent comp in filtersPanel.components)
                {
                    UICheckBox box = comp as UICheckBox;
                    if (box != null)
                    {
                        if (!NetworkFilter.GetNames().Exists(n => n.Equals(box.name))) continue;
                        if (box != c) box.isChecked = false;
                    }
                }

                ((UICheckBox)c).isChecked = true;
            }

            UICheckBox checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Buildings";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterBuildings = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Props";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterProps = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Decals";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterDecals = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Surfaces";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveMore.filterSurfaces = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Trees";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterTrees = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Nodes";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterNodes = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Segments";
            checkBox.isChecked = true;
            checkBox.eventCheckChanged += (c, p) =>
            {
                MoveItTool.filterSegments = p;
            };
            checkBox.eventDoubleClick += OnDoubleClick;

            UIButton btnNetworks = UI.CreateToggleNF();

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Roads";
            checkBox.name = "NF-Roads";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Roads");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Tracks";
            checkBox.name = "NF-Tracks";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Tracks");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Paths";
            checkBox.name = "NF-Paths";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Paths");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Fences";
            checkBox.name = "NF-Fences";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Fences");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Powerlines";
            checkBox.name = "NF-Power";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Power");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            checkBox = SamsamTS.UIUtils.CreateCheckBox(filtersPanel);
            checkBox.label.text = "Other";
            checkBox.name = "NF-Other";
            checkBox.isChecked = true;
            checkBox.isVisible = false;
            UI.NetworkCheckboxes.Add(checkBox);
            checkBox.eventCheckChanged += (c, p) =>
            {
                NetworkFilter.ToggleFilter("NF-Other");
            };
            checkBox.eventDoubleClick += OnDoubleClickNetwork;

            filtersPanel.padding = new RectOffset(10, 10, 10, 10);
            filtersPanel.autoLayoutDirection = LayoutDirection.Vertical;
            filtersPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
            filtersPanel.autoLayout = true;
            filtersPanel.height = 210;
            filtersPanel.absolutePosition = marquee.absolutePosition - new Vector3(0, filtersPanel.height + 5);

            marquee.eventDoubleClick += (UIComponent c, UIMouseEventParameter p) =>
            {
                int u = 0;
                bool v = false;
                foreach (UIComponent comp in filtersPanel.components)
                {
                    UICheckBox box = comp as UICheckBox;
                    if (box != null)
                    {
                        if (box.isChecked == false)
                            u++;
                    }
                }
                if (u > 0) v = true;
                foreach (UIComponent comp in filtersPanel.components)
                {
                    UICheckBox box = comp as UICheckBox;
                    if (box != null)
                    {
                        box.isChecked = v;
                    }
                }
            };

            Component[] component = filtersPanel.GetComponentsInChildren(typeof(UICheckBox));
            Component[] label = filtersPanel.GetComponentsInChildren(typeof(UILabel));
            //MoveMore.DebugLine($"{filtersPanel.childCount},{filtersPanel.height} component-length:{component.Length}, label-length:{label.Length}");
        }
    }


    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("StopAligningHeights")]
    class MIT_StopAligningHeights
    {
        public static void Prefix(MoveItTool __instance)
        {
            if (__instance.toolState == MoveItTool.ToolState.AligningHeights && MoveMore.mode != MoveMore.AlignMode.Off)
            {
                // User switched tool
                MoveMore.mode = MoveMore.AlignMode.Off;
            }
        }
    }


    // Completely replaces MoveIt's OnLeftClick
    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("OnLeftClick")]
    class MIT_OnLeftClick
    {
        public static bool Prefix(MoveItTool __instance, ref Instance ___m_hoverInstance, ref int ___m_nextAction)
        {
            if (__instance == null)
            {
                //Debug.Log("Null instance!");
                return true;
            }

            if (MoveMore.mode != MoveMore.AlignMode.Off)
            {
                float angle;

                if (___m_hoverInstance is MoveableBuilding mb)
                {
                    Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[mb.id.Building];
                    angle = building.m_angle;
                }
                else if (___m_hoverInstance is MoveableProp mp)
                {
                    PropInstance prop = Singleton<PropManager>.instance.m_props.m_buffer[mp.id.Prop];
                    angle = prop.Angle;
                }
                else if (___m_hoverInstance is MoveableSegment ms)
                {
                    PropInstance segment = Singleton<PropManager>.instance.m_props.m_buffer[ms.id.Prop];
                    NetSegment[] segmentBuffer = NetManager.instance.m_segments.m_buffer;

                    Vector3 startPos = NetManager.instance.m_nodes.m_buffer[segmentBuffer[ms.id.NetSegment].m_startNode].m_position;
                    Vector3 endPos = NetManager.instance.m_nodes.m_buffer[segmentBuffer[ms.id.NetSegment].m_endNode].m_position;

                    //Debug.Log($"Vector:{endNode.x - startNode.x},{endNode.z - startNode.z} Start:{startNode.x},{startNode.z} End:{endNode.x},{endNode.z}");
                    angle = (float)Math.Atan2(endPos.z - startPos.z, endPos.x - startPos.x);
                }
                else
                {
                    //Debug.Log($"Wrong hover asset type <{___m_hoverInstance.GetType()}>");
                    return MoveMore.Deactivate();
                }

                // Add action to queue, also enables Undo/Redo
                AlignRotationAction action;
                switch (MoveMore.mode)
                {
                    case MoveMore.AlignMode.All:
                        action = new AlignGroupRotationAction();
                        break;

                    default:
                        action = new AlignEachRotationAction();
                        break;
                }
                action.newAngle = angle;
                action.followTerrain = MoveItTool.followTerrain;
                ActionQueue.instance.Push(action);
                ___m_nextAction = MoveMore.TOOL_ACTION_DO;

                //Debug.Log($"Angle:{angle}, from {___m_hoverInstance}");
                return MoveMore.Deactivate(false);
            }

            return true;
        }
    }


    static class Detour_Utils
    {
        public static bool IsBuildingValid(ref Building building, ItemClass.Layer itemLayers)
        {
            if ((building.m_flags & Building.Flags.Created) == Building.Flags.Created)
            {
                return ((building.Info.m_class.m_layer & itemLayers) != ItemClass.Layer.None);
            }

            return false;
        }

        public static bool IsNodeValid(ref NetNode node, ItemClass.Layer itemLayers)
        {
            if ((node.m_flags & NetNode.Flags.Created) == NetNode.Flags.Created)
            {
                if ((node.Info.GetConnectionClass().m_layer & itemLayers) == ItemClass.Layer.None)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool IsSegmentValid(ref NetSegment segment, ItemClass.Layer itemLayers)
        {
            if ((segment.m_flags & NetSegment.Flags.Created) == NetSegment.Flags.Created)
            {
                if ((segment.Info.GetConnectionClass().m_layer & itemLayers) == ItemClass.Layer.None)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static bool RayCastNode(ref NetNode node, Segment3 ray, float snapElevation, out float t, out float priority)
        {
            NetInfo info = node.Info;
            float num = (float)node.m_elevation + info.m_netAI.GetSnapElevation();
            float t2;
            if (info.m_netAI.IsUnderground())
            {
                t2 = Mathf.Clamp01(Mathf.Abs(snapElevation + num) / 12f);
            }
            else
            {
                t2 = Mathf.Clamp01(Mathf.Abs(snapElevation - num) / 12f);
            }
            float collisionHalfWidth = Mathf.Max(3f, info.m_netAI.GetCollisionHalfWidth());
            float num2 = Mathf.Lerp(info.GetMinNodeDistance(), collisionHalfWidth, t2);
            if (Segment1.Intersect(ray.a.y, ray.b.y, node.m_position.y, out t))
            {
                float num3 = Vector3.Distance(ray.Position(t), node.m_position);
                if (num3 < num2)
                {
                    priority = Mathf.Max(0f, num3 - collisionHalfWidth);
                    return true;
                }
            }
            t = 0f;
            priority = 0f;
            return false;
        }
    }


    /* Move It! sub-sub-building fix */

    [HarmonyPatch(typeof(MoveableBuilding))]
    [HarmonyPatch("GetState")]
    class MB_GetState
    {
        protected static Building[] buildingBuffer = BuildingManager.instance.m_buildings.m_buffer;

        public static InstanceState Postfix(InstanceState state, ref MoveableBuilding __instance)
        {
            List<InstanceState> subSubStates = new List<InstanceState>();
            BuildingState buildingState = (BuildingState)state;

            if (buildingState.subStates != null)
            {
                foreach (InstanceState subState in buildingState.subStates)
                {
                    if (subState != null)
                    {
                        if (subState is BuildingState subBuildingState)
                        {
                            if (subBuildingState.instance != null && subBuildingState.instance.isValid)
                            {
                                BuildingState ss = (BuildingState)subState;
                                MoveableBuilding subInstance = (MoveableBuilding)subBuildingState.instance;
                                subSubStates.Clear();

                                ushort parent = buildingBuffer[subInstance.id.Building].m_parentBuilding; // Hack to get around Move It's single layer check
                                buildingBuffer[subInstance.id.Building].m_parentBuilding = 0;
                                foreach (Instance subSubInstance in subInstance.subInstances)
                                {
                                    if (subSubInstance != null && subSubInstance.isValid)
                                    {
                                        subSubStates.Add(subSubInstance.GetState());
                                    }
                                }
                                buildingBuffer[subInstance.id.Building].m_parentBuilding = parent;

                                if (subSubStates.Count > 0)
                                {
                                    ss.subStates = subSubStates.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            return state;
        }
    }


    [HarmonyPatch(typeof(MoveableBuilding))]
    [HarmonyPatch("SetState")]
    class MB_SetState
    {
        protected static Building[] buildingBuffer = BuildingManager.instance.m_buildings.m_buffer;

        public static void Postfix(InstanceState state, MoveableBuilding __instance)
        {
            if (!(state is BuildingState buildingState)) {
                return;
            }

            //Debug.Log($"SS0 - {buildingState.subStates}");
            if (buildingState.subStates != null)
            {
                foreach (InstanceState subState in buildingState.subStates)
                {
                    if (subState != null)
                    {
                        if (subState is BuildingState subBuildingState)
                        {
                            if (subBuildingState.instance != null && subBuildingState.instance.isValid)
                            {
                                BuildingState ss = (BuildingState)subState;
                                MoveableBuilding subInstance = (MoveableBuilding)subBuildingState.instance;
                                if (ss.subStates != null)
                                {
                                    foreach (InstanceState subSubState in ss.subStates)
                                    {
                                        if (subSubState != null)
                                        {
                                            subSubState.instance.SetState(subSubState);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

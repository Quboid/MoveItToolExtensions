using ColossalFramework;
using Harmony;
using MoveIt;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

// More detours in Moveable*.cs, UI_Filters.cs and Filter_Detours.cs

namespace MITE
{
    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("StopAligningHeights")]
    class MIT_StopAligningHeights
    {
        public static bool Prefix(MoveItTool __instance)
        {
            //Debug.Log($"toolState:{__instance.toolState}");
            if (__instance.toolState == MoveItTool.ToolState.AligningHeights)
            {
                // User switched tool
                __instance.toolState = MoveItTool.ToolState.Default;
                MITE.AlignMode = MITE.AlignModes.Off;
                UI.UpdateAlignTools();
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("StartAligningHeights")]
    class MIT_StatAligningHeights
    {
        public static void Postfix(MoveItTool __instance)
        {
            //Debug.Log($"toolState:{__instance.toolState}");
            if (__instance.toolState == MoveItTool.ToolState.AligningHeights)
            {
                MITE.AlignMode = MITE.AlignModes.Height;
                UI.UpdateAlignTools();
            }
            return;
        }
    }


    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("OnLeftClick")]
    class MIT_OnLeftClick
    {
        public static bool Prefix(MoveItTool __instance, ref Instance ___m_hoverInstance, ref int ___m_nextAction)
        {
            if (___m_hoverInstance == null)
                return true;

            if (MITE.AlignMode != MITE.AlignModes.Off && MITE.AlignMode != MITE.AlignModes.Height)
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
                    return MITE.DeactivateAlignTool();
                }

                // Add action to queue, also enables Undo/Redo
                AlignRotationAction action;
                switch (MITE.AlignMode)
                {
                    case MITE.AlignModes.Group:
                        action = new AlignGroupAction();
                        break;

                    default:
                        action = new AlignIndividualAction();
                        break;
                }
                action.newAngle = angle;
                action.followTerrain = MoveItTool.followTerrain;
                ActionQueue.instance.Push(action);
                ___m_nextAction = MITE.Tool_Action_Do;

                //Debug.Log($"Angle:{angle}, from {___m_hoverInstance}");
                return MITE.DeactivateAlignTool(false);
            }

            if (__instance.toolState == MoveItTool.ToolState.AligningHeights)
            {
                //Debug.Log($"toolState is AligningHeights, AlignMode:{MITE.AlignMode}");
                if (MITE.AlignMode == MITE.AlignModes.Height)
                {
                    MITE.AlignMode = MITE.AlignModes.Off;
                }
                UI.UpdateAlignTools();
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
    #region

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
    #endregion
}

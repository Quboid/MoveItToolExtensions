using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using MoveIt;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace MITE
{
    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("RaycastHoverInstance")]
    class MIT_RaycastHoverInstance
    {
        private static bool Prefix(Ray mouseRay, MoveItTool __instance, ref Instance ___m_hoverInstance)
        {
            Traverse _MIT = Traverse.Create(__instance);

            // MoveIt code
            ___m_hoverInstance = null;

            Vector3 origin = mouseRay.origin;
            Vector3 normalized = mouseRay.direction.normalized;
            Vector3 vector = mouseRay.origin + normalized * Camera.main.farClipPlane;
            Segment3 ray = new Segment3(origin, vector);

            Building[] buildingBuffer = BuildingManager.instance.m_buildings.m_buffer;
            PropInstance[] propBuffer = PropManager.instance.m_props.m_buffer;
            NetNode[] nodeBuffer = NetManager.instance.m_nodes.m_buffer;
            NetSegment[] segmentBuffer = NetManager.instance.m_segments.m_buffer;
            TreeInstance[] treeBuffer = TreeManager.instance.m_trees.m_buffer;

            Vector3 location = _MIT.Method("RaycastMouseLocation", mouseRay).GetValue<Vector3>();

            InstanceID id = InstanceID.Empty;

            ItemClass.Layer itemLayers = _MIT.Method("GetItemLayers").GetValue<ItemClass.Layer>();

            bool selectBuilding = true;
            bool selectProps = true;
            bool selectDecals = true;
            bool selectSurfaces = true;
            bool selectNodes = true;
            bool selectSegments = true;
            bool selectTrees = true;

            bool _stepProcessed = false;
            bool _repeatSearch = false;

            if (MoveItTool.marqueeSelection)
            {
                selectBuilding = MoveItTool.filterBuildings;
                selectProps = MoveItTool.filterProps;
                selectDecals = MoveItTool.filterDecals;
                selectSurfaces = MITE.filterSurfaces;
                selectNodes = MoveItTool.filterNodes;
                selectSegments = MoveItTool.filterSegments;
                selectTrees = MoveItTool.filterTrees;
            }

            float smallestDist = 640000f;

            do
            {
                int gridMinX = Mathf.Max((int)((location.x - 16f) / 64f + 135f) - 1, 0);
                int gridMinZ = Mathf.Max((int)((location.z - 16f) / 64f + 135f) - 1, 0);
                int gridMaxX = Mathf.Min((int)((location.x + 16f) / 64f + 135f) + 1, 269);
                int gridMaxZ = Mathf.Min((int)((location.z + 16f) / 64f + 135f) + 1, 269);

                for (int i = gridMinZ; i <= gridMaxZ; i++)
                {
                    for (int j = gridMinX; j <= gridMaxX; j++)
                    {
                        if (selectBuilding || selectSurfaces)
                        {
                            ushort building = BuildingManager.instance.m_buildingGrid[i * 270 + j];
                            int count = 0;
                            while (building != 0u)
                            {
                                if (MITE.StepOver.isValidB(building) && Detour_Utils.IsBuildingValid(ref buildingBuffer[building], itemLayers) && buildingBuffer[building].RayCast(building, ray, out float t) && t < smallestDist)
                                {
                                    //Debug.Log($"Building:{building}");

                                    if (Filters.Filter(buildingBuffer[building].Info, true))
                                    {
                                        id.Building = Building.FindParentBuilding(building);
                                        if (id.Building == 0) id.Building = building;
                                        smallestDist = t;
                                    }
                                }
                                building = buildingBuffer[building].m_nextGridBuilding;

                                if (++count > 49152)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                    break;
                                }
                            }
                        }

                        if (selectProps || selectDecals || selectSurfaces)
                        {
                            ushort prop = PropManager.instance.m_propGrid[i * 270 + j];
                            int count = 0;
                            while (prop != 0u)
                            {
                                if (MITE.StepOver.isValidP(prop) && Filters.Filter(propBuffer[prop].Info) && MITE.StepOver.isValidP(prop))
                                {
                                    //Debug.Log($"Prop:{prop}");
                                    if (propBuffer[prop].RayCast(prop, ray, out float t, out float targetSqr) && t < smallestDist)
                                    {
                                        id.Prop = prop;
                                        smallestDist = t;
                                    }
                                }

                                prop = propBuffer[prop].m_nextGridProp;

                                if (++count > 65536)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }

                        if (selectNodes || selectBuilding)
                        {
                            ushort node = NetManager.instance.m_nodeGrid[i * 270 + j];
                            int count = 0;
                            while (node != 0u)
                            {
                                if (MITE.StepOver.isValidN(node) && Detour_Utils.IsNodeValid(ref nodeBuffer[node], itemLayers) && Detour_Utils.RayCastNode(ref nodeBuffer[node], ray, -1000f, out float t, out float priority) && t < smallestDist)
                                {
                                    //Debug.Log($"Node:{node}");
                                    ushort building = 0;
                                    if (!Event.current.alt)
                                    {
                                        building = NetNode.FindOwnerBuilding(node, 363f);
                                    }

                                    if (building != 0)
                                    {
                                        if (selectBuilding)
                                        {
                                            id.Building = Building.FindParentBuilding(building);
                                            if (id.Building == 0) id.Building = building;
                                            smallestDist = t;
                                        }
                                    }
                                    else if (selectNodes)
                                    {
                                        if (Filters.Filter(nodeBuffer[node]))
                                        {
                                            id.NetNode = node;
                                            smallestDist = t;
                                        }
                                    }
                                }
                                node = nodeBuffer[node].m_nextGridNode;

                                if (++count > 32768)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }

                        if (selectSegments || selectBuilding)
                        {
                            ushort segment = NetManager.instance.m_segmentGrid[i * 270 + j];
                            int count = 0;
                            while (segment != 0u)
                            {
                                if (MITE.StepOver.isValidS(segment) && Detour_Utils.IsSegmentValid(ref segmentBuffer[segment], itemLayers) &&
                                    segmentBuffer[segment].RayCast(segment, ray, -1000f, false, out float t, out float priority) && t < smallestDist)
                                {
                                    //Debug.Log($"Segment:{segment}");
                                    ushort building = 0;
                                    if (!Event.current.alt)
                                    {
                                        building = MoveItTool.FindOwnerBuilding(segment, 363f);
                                    }

                                    if (building != 0)
                                    {
                                        if (selectBuilding)
                                        {
                                            id.Building = Building.FindParentBuilding(building);
                                            if (id.Building == 0) id.Building = building;
                                            smallestDist = t;
                                        }
                                    }
                                    else if (selectSegments)
                                    {
                                        if (!selectNodes || (
                                            !Detour_Utils.RayCastNode(ref nodeBuffer[segmentBuffer[segment].m_startNode], ray, -1000f, out float t2, out priority) &&
                                            !Detour_Utils.RayCastNode(ref nodeBuffer[segmentBuffer[segment].m_endNode], ray, -1000f, out t2, out priority)
                                        ))
                                        {
                                            if (Filters.Filter(segmentBuffer[segment]))
                                            {
                                                id.NetSegment = segment;
                                                smallestDist = t;
                                            }
                                        }
                                    }
                                }
                                segment = segmentBuffer[segment].m_nextGridSegment;

                                if (++count > 36864)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }
                    }
                }

                if (selectTrees)
                {
                    gridMinX = Mathf.Max((int)((location.x - 8f) / 32f + 270f), 0);
                    gridMinZ = Mathf.Max((int)((location.z - 8f) / 32f + 270f), 0);
                    gridMaxX = Mathf.Min((int)((location.x + 8f) / 32f + 270f), 539);
                    gridMaxZ = Mathf.Min((int)((location.z + 8f) / 32f + 270f), 539);

                    for (int i = gridMinZ; i <= gridMaxZ; i++)
                    {
                        for (int j = gridMinX; j <= gridMaxX; j++)
                        {
                            uint tree = TreeManager.instance.m_treeGrid[i * 540 + j];
                            int count = 0;
                            while (tree != 0)
                            {
                                if (MITE.StepOver.isValidT(tree) && treeBuffer[tree].RayCast(tree, ray, out float t, out float targetSqr) && t < smallestDist)
                                {
                                    id.Tree = tree;
                                    smallestDist = t;
                                }
                                tree = treeBuffer[tree].m_nextGridTree;

                                if (++count > 262144)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }
                    }
                }
                _repeatSearch = false;

                if (Event.current.Equals(Event.KeyboardEvent("^tab")))
                {
                    if (!_stepProcessed)
                    {
                        MITE.StepOver.Add(id);
                        _stepProcessed = true;
                        _repeatSearch = true;
                    }
                }
                else
                {
                    _stepProcessed = false;
                }
            }
            while (_repeatSearch);

            //Debug.Log($"ID=({id.Building},{id.Prop},{id.NetNode},{id.NetSegment},{id.Tree})");
            if (UI.DbgPanel != null)
            {
                UI.DbgPanel.Update(id);
            }
            ___m_hoverInstance = id;

            return false;
        }
    }


    [HarmonyPatch(typeof(MoveItTool))]
    [HarmonyPatch("GetMarqueeList")]
    class MIT_GetMarqueeList
    {
        private static HashSet<Instance> Postfix(HashSet<Instance> hashset, Ray mouseRay, MoveItTool __instance, ref Quad3 ___m_selection)
        {
            // Reset from Harmony hooks
            Quad3 m_selection = ___m_selection;
            MoveItTool MIT = __instance;
            Traverse _MIT = Traverse.Create(__instance);

            HashSet<Instance> list = new HashSet<Instance>();

            Building[] buildingBuffer = BuildingManager.instance.m_buildings.m_buffer;
            PropInstance[] propBuffer = PropManager.instance.m_props.m_buffer;
            NetNode[] nodeBuffer = NetManager.instance.m_nodes.m_buffer;
            NetSegment[] segmentBuffer = NetManager.instance.m_segments.m_buffer;
            TreeInstance[] treeBuffer = TreeManager.instance.m_trees.m_buffer;

            if (m_selection.a.x == m_selection.c.x && m_selection.a.z == m_selection.c.z)
            {
                m_selection = default(Quad3);
            }
            else
            {
                float angle = Camera.main.transform.localEulerAngles.y * Mathf.Deg2Rad;
                Vector3 down = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
                Vector3 right = new Vector3(-down.z, 0, down.x);

                Vector3 a = m_selection.c - m_selection.a;
                float dotDown = Vector3.Dot(a, down);
                float dotRight = Vector3.Dot(a, right);

                if ((dotDown > 0 && dotRight > 0) || (dotDown <= 0 && dotRight <= 0))
                {
                    m_selection.b = m_selection.a + dotDown * down;
                    m_selection.d = m_selection.a + dotRight * right;
                }
                else
                {
                    m_selection.b = m_selection.a + dotRight * right;
                    m_selection.d = m_selection.a + dotDown * down;
                }

                Vector3 min = m_selection.Min();
                Vector3 max = m_selection.Max();

                int gridMinX = Mathf.Max((int)((min.x - 16f) / 64f + 135f), 0);
                int gridMinZ = Mathf.Max((int)((min.z - 16f) / 64f + 135f), 0);
                int gridMaxX = Mathf.Min((int)((max.x + 16f) / 64f + 135f), 269);
                int gridMaxZ = Mathf.Min((int)((max.z + 16f) / 64f + 135f), 269);

                InstanceID id = new InstanceID();

                ItemClass.Layer itemLayers = _MIT.Method("GetItemLayers").GetValue<ItemClass.Layer>();

                for (int i = gridMinZ; i <= gridMaxZ; i++)
                {
                    for (int j = gridMinX; j <= gridMaxX; j++)
                    {
                        if (MoveItTool.filterBuildings || MITE.filterSurfaces)
                        {
                            ushort building = BuildingManager.instance.m_buildingGrid[i * 270 + j];
                            int count = 0;
                            while (building != 0u)
                            {
                                //Debug.Log($"Building:{building}");
                                if (Detour_Utils.IsBuildingValid(ref buildingBuffer[building], itemLayers) && _MIT.Method("PointInRectangle", m_selection, buildingBuffer[building].m_position).GetValue<bool>())
                                {
                                    if (Filters.Filter(buildingBuffer[building].Info))
                                    {
                                        id.Building = Building.FindParentBuilding(building);
                                        if (id.Building == 0) id.Building = building;
                                        list.Add(id);
                                    }
                                }
                                building = buildingBuffer[building].m_nextGridBuilding;

                                if (++count > 49152)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                    break;
                                }
                            }
                        }

                        if (MoveItTool.filterProps || MoveItTool.filterDecals || MITE.filterSurfaces)
                        {
                            ushort prop = PropManager.instance.m_propGrid[i * 270 + j];
                            int count = 0;
                            while (prop != 0u)
                            {
                                //Debug.Log($"Prop:{prop}");
                                if (Filters.Filter(propBuffer[prop].Info))
                                {
                                    if (_MIT.Method("PointInRectangle", m_selection, propBuffer[prop].Position).GetValue<bool>())
                                    {
                                        id.Prop = prop;
                                        list.Add(id);
                                    }
                                }

                                prop = propBuffer[prop].m_nextGridProp;

                                if (++count > 65536)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }

                        if (MoveItTool.filterNodes || MoveItTool.filterBuildings)
                        {
                            ushort node = NetManager.instance.m_nodeGrid[i * 270 + j];
                            int count = 0;
                            while (node != 0u)
                            {
                                //Debug.Log($"Node:{node}");
                                if (Detour_Utils.IsNodeValid(ref nodeBuffer[node], itemLayers) && _MIT.Method("PointInRectangle", m_selection, nodeBuffer[node].m_position).GetValue<bool>())
                                {
                                    ushort building = NetNode.FindOwnerBuilding(node, 363f);

                                    if (building != 0)
                                    {
                                        if (MoveItTool.filterBuildings)
                                        {
                                            id.Building = Building.FindParentBuilding(building);
                                            if (id.Building == 0) id.Building = building;
                                            list.Add(id);
                                        }
                                    }
                                    else if (MoveItTool.filterNodes)
                                    {
                                        if (Filters.Filter(nodeBuffer[node]))
                                        {
                                            //Debug.Log($"Node:{node}");
                                            id.NetNode = node;
                                            list.Add(id);
                                        }
                                    }
                                }
                                node = nodeBuffer[node].m_nextGridNode;

                                if (++count > 32768)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }

                        if (MoveItTool.filterSegments || MoveItTool.filterBuildings)
                        {
                            ushort segment = NetManager.instance.m_segmentGrid[i * 270 + j];
                            int count = 0;
                            while (segment != 0u)
                            {
                                //Debug.Log($"Segment:{segment}");
                                if (Detour_Utils.IsSegmentValid(ref segmentBuffer[segment], itemLayers) && _MIT.Method("PointInRectangle", m_selection, segmentBuffer[segment].m_bounds.center).GetValue<bool>())
                                {
                                    ushort building = MoveItTool.FindOwnerBuilding(segment, 363f);

                                    if (building != 0)
                                    {
                                        if (MoveItTool.filterBuildings)
                                        {
                                            id.Building = Building.FindParentBuilding(building);
                                            if (id.Building == 0) id.Building = building;
                                            list.Add(id);
                                        }
                                    }
                                    else if (MoveItTool.filterSegments)
                                    {
                                        if (Filters.Filter(segmentBuffer[segment]))
                                        {
                                            id.NetSegment = segment;
                                            list.Add(id);
                                        }
                                    }
                                }
                                segment = segmentBuffer[segment].m_nextGridSegment;

                                if (++count > 36864)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }
                    }
                }

                if (MoveItTool.filterTrees)
                {
                    gridMinX = Mathf.Max((int)((min.x - 8f) / 32f + 270f), 0);
                    gridMinZ = Mathf.Max((int)((min.z - 8f) / 32f + 270f), 0);
                    gridMaxX = Mathf.Min((int)((max.x + 8f) / 32f + 270f), 539);
                    gridMaxZ = Mathf.Min((int)((max.z + 8f) / 32f + 270f), 539);

                    for (int i = gridMinZ; i <= gridMaxZ; i++)
                    {
                        for (int j = gridMinX; j <= gridMaxX; j++)
                        {
                            uint tree = TreeManager.instance.m_treeGrid[i * 540 + j];
                            int count = 0;
                            while (tree != 0)
                            {
                                //Debug.Log($"Tree:{tree}");
                                if (_MIT.Method("PointInRectangle", m_selection, treeBuffer[tree].Position).GetValue<bool>())
                                {
                                    id.Tree = tree;
                                    list.Add(id);
                                }
                                tree = treeBuffer[tree].m_nextGridTree;

                                if (++count > 262144)
                                {
                                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}

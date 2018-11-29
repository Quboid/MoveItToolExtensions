﻿using Harmony;
using MoveIt;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MITE
{
    class Filters
    {
        static readonly string[] SurfaceMeshNames = new string[]
        {
            "ploppablegravel",
            "ploppablecliffgrass",
            "ploppableasphalt-prop"
        };

        static readonly string[] SurfaceDecalMeshNames = new string[]
        {
            "ploppableasphalt-decal"
        };

        static readonly string[] SurfaceDeczaahNames = new string[]
        {
            "999653286.Ploppable"
        };
        static readonly string[] SurfaceDockNames = new string[]
        {
            "1136492728.R69 Docks"
        };
        static readonly string[] SurfaceBrushNames = new string[]
        {
            "418194210", "416836974", "416837329", "416837674", "416916315", "416924392", // Agriculture
            "418187161", "416103351", "423541340", "416107948", "416109689", "416916560", "416924629", // Concrete
            "416106438", "416108293", "416110102", "416917279", "416924830", "416924830", // Gravel
            "418188094", "416107568", "422323255", "416109334", "416111243", "416917700", "416925008", // Ruined
            "418188427", "418188861", "418415108", // Tiled
            "418187791", "418188652", "418414886" // Marble
        };

        public static Dictionary<string, NetworkFilter> NetworkFilters = new Dictionary<string, NetworkFilter>
        {
            { "Roads", new NetworkFilter(true, new List<Type> { typeof(RoadBaseAI) } ) },
            { "Tracks", new NetworkFilter(true, new List<Type> { typeof(TrainTrackBaseAI), typeof(MonorailTrackAI) } ) },
            { "Paths", new NetworkFilter(true, new List<Type> { typeof(PedestrianPathAI), typeof(PedestrianTunnelAI), typeof(PedestrianBridgeAI), typeof(PedestrianWayAI) } ) },
            { "Fences", new NetworkFilter(true, new List<Type> { typeof(DecorationWallAI) } ) },
            { "Powerlines", new NetworkFilter(true, new List<Type> { typeof(PowerLineAI) } ) },
            { "Others", new NetworkFilter(true,  null ) }
        };


        public static void SetFilter(string name, bool active)
        {
            switch (name)
            {
                case "Buildings":
                    MoveItTool.filterBuildings = active;
                    break;
                case "Props":
                    MoveItTool.filterProps = active;
                    break;
                case "Decals":
                    MoveItTool.filterDecals = active;
                    break;
                case "Surfaces":
                    MITE.filterSurfaces = active;
                    break;
                case "Trees":
                    MoveItTool.filterTrees = active;
                    break;
                case "Nodes":
                    MoveItTool.filterNodes = active;
                    break;
                case "Segments":
                    MoveItTool.filterSegments = active;
                    break;

                default:
                    throw new Exception();
            }

            //UI.RefreshFilters();
        }

        public static void SetNetworkFilter(string name, bool active)
        {
            NetworkFilters.GetValueSafe(name).enabled = active;
            //UI.RefreshFilters();
        }

        public static void ToggleFilter(string name)
        {
            switch (name)
            {
                case "Buildings":
                    MoveItTool.filterBuildings = !MoveItTool.filterBuildings;
                    break;
                case "Props":
                    MoveItTool.filterProps = !MoveItTool.filterProps;
                    break;
                case "Decals":
                    MoveItTool.filterDecals = !MoveItTool.filterDecals;
                    break;
                case "Surfaces":
                    MITE.filterSurfaces = !MITE.filterSurfaces;
                    break;
                case "Trees":
                    MoveItTool.filterTrees = !MoveItTool.filterTrees;
                    break;
                case "Nodes":
                    MoveItTool.filterNodes = !MoveItTool.filterNodes;
                    break;
                case "Segments":
                    MoveItTool.filterSegments = !MoveItTool.filterSegments;
                    break;

                default:
                    throw new Exception();
            }

            //UI.RefreshFilters();
        }

        public static void ToggleNetworkFilter(string name)
        {
            NetworkFilters.GetValueSafe(name).enabled = !NetworkFilters.GetValueSafe(name).enabled;
            //UI.RefreshFilters();

            /*string msg = $"Toggling '{name}' ";
            foreach (KeyValuePair<string, NetworkFilter> pair in Filters.NetworkFilters)
            {
                msg = msg + $"{pair.Key}:({pair.Value.aiType},{pair.Value.enabled}),box:";
                foreach (UICheckBox cb in UI.NetworkCBs)
                {
                    if (cb.name == pair.Key)
                    {
                        msg = msg + $" {cb.name}:{cb.isChecked}";
                        break;
                    }
                }
                msg = msg + "\n";
            }
            Debug.Log(msg);*/
        }


        public static bool IsSurface(BuildingInfo info)
        {
            if (MITE.Settings.DocksAsSurfaces)
            {
                foreach (string subname in SurfaceDockNames)
                {
                    if (subname.Length > info.name.Length) continue;
                    if (subname == info.name.Substring(0, subname.Length))
                    {
                        return true;
                    }
                }
            }

            if (MITE.Settings.DeczaahAsSurfaces)
            {
                foreach (string subname in SurfaceDeczaahNames)
                {
                    if (subname.Length > info.name.Length) continue;
                    if (subname == info.name.Substring(0, subname.Length))
                    {
                        return true;
                    }
                }
            }

            if (MITE.Settings.BrushesAsSurfaces)
            {
                foreach (string subname in SurfaceBrushNames)
                {
                    if (subname.Length > info.name.Length) continue;
                    if (subname == info.name.Substring(0, subname.Length))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsSurface(PropInfo info)
        {
            if (Array.Exists(SurfaceMeshNames, s => s.Equals(info.m_mesh.name))) {
                return true;
            }

            if (MITE.Settings.DecalsAsSurfaces && Array.Exists(SurfaceDecalMeshNames, s => s.Equals(info.m_mesh.name)))
            {
                return true;
            }

            if (MITE.Settings.DeczaahAsSurfaces)
            {
                foreach (string subname in SurfaceDeczaahNames)
                {
                    if (subname.Length > info.name.Length) continue;
                    if (subname == info.name.Substring(0, subname.Length))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public static bool Filter(PropInfo info)
        {
            if (!MoveItTool.marqueeSelection) return true;

            if (info.m_isDecal)
            {
                if (MITE.Settings.DecalsAsSurfaces)
                {
                    if (MITE.filterSurfaces && IsSurface(info))
                    {
                        return true;
                    }
                    if (MoveItTool.filterDecals && !IsSurface(info))
                    {
                        return true;
                    }
                }
                else
                {
                    if (MoveItTool.filterDecals)
                    {
                        return true;
                    }
                }
                return false;
            }

            if (IsSurface(info))
            {
                if (MITE.filterSurfaces)
                {
                    return true;
                }
                return false;
            }

            if (MoveItTool.filterProps)
            {
                return true;
            }
            return false;
        }

        public static bool Filter(NetNode node)
        {
            return _networkFilter(node.Info);
        }

        public static bool Filter(NetSegment segment)
        {
            return _networkFilter(segment.Info);
        }

        private static bool _networkFilter(NetInfo info)
        {
            //Debug.Log($"{info.name}");
            if (!MoveItTool.marqueeSelection) return true;
            if (!MITE.filterNetworks) return true;

            NetworkFilter nf = NetworkFilter.GetNetworkFilter(info.GetAI().GetType());
            return nf.enabled;
        }
    }


    public class NetworkFilter
    {
        public bool enabled;
        public List<Type> aiType;

        public NetworkFilter(bool e, List<Type> a)
        {
            enabled = e;
            aiType = a;
        }

        public static void SetNetworkFilter(string name, bool e)
        {
            Filters.NetworkFilters.GetValueSafe(name).enabled = e;
        }

        public static NetworkFilter GetNetworkFilter(Type ai)
        {
            foreach (NetworkFilter nf in Filters.NetworkFilters.Values)
            {
                //Debug.Log($"ai:{ai}, count:{(nf.aiType == null ? 0 : nf.aiType.Count)}");
                if (nf.aiType != null)
                {
                    foreach (Type t in nf.aiType)
                    {
                        if (ai == t || ai.IsSubclassOf(t))
                        {
                            return nf;
                        }
                    }
                }
            }
            return Filters.NetworkFilters.GetValueSafe("Others");
        }
    }
}

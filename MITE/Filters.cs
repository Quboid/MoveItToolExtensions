using Harmony;
using MoveIt;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MITE
{
    class Filters
    {
        static readonly string[] SurfaceNames = new string[]
        {
            "ploppablegravel",
            "ploppablecliffgrass",
            "ploppableasphalt-prop"
        };

        static readonly string[] SurfaceDecalNames = new string[]
        {
            "ploppableasphalt-decal"
        };

        public static Dictionary<string, NetworkFilter> NetworkFilters = new Dictionary<string, NetworkFilter>
        {
            { "NF-Roads", new NetworkFilter(true, new List<Type> { typeof(RoadBaseAI) } ) },
            { "NF-Tracks", new NetworkFilter(true, new List<Type> { typeof(TrainTrackBaseAI), typeof(MonorailTrackAI) } ) },
            { "NF-Paths", new NetworkFilter(true, new List<Type> { typeof(PedestrianPathAI), typeof(PedestrianTunnelAI), typeof(PedestrianBridgeAI), typeof(PedestrianWayAI) } ) },
            { "NF-Fences", new NetworkFilter(true, new List<Type> { typeof(DecorationWallAI) } ) },
            { "NF-Power", new NetworkFilter(true, new List<Type> { typeof(PowerLineAI) } ) },
            { "NF-Other", new NetworkFilter(true,  null ) }
        };


        public static bool IsSurface(PropInfo info)
        {
            if (Array.Exists(SurfaceNames, s => s.Equals(info.m_mesh.name))) {
                return true;
            }

            if (MITE.Settings.DecalsAsSurfaces && Array.Exists(SurfaceDecalNames, s => s.Equals(info.m_mesh.name)))
            {
                return true;
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

            NetworkFilter nf = NetworkFilter.GetFilter(info.GetAI().GetType());
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

        public static void SetFilter(string name, bool e)
        {
            Filters.NetworkFilters.GetValueSafe(name).enabled = e;
        }

        public static void ToggleFilter(string name)
        {
            Filters.NetworkFilters.GetValueSafe(name).enabled = !Filters.NetworkFilters.GetValueSafe(name).enabled;

            /*string msg = $"Toggling '{name}' ";
            foreach (KeyValuePair<string, NetworkFilter> pair in MITE.NetworkFilters)
            {
                msg = msg + $"{pair.Key}:({pair.Value.aiType},{pair.Value.enabled}),box:";
                foreach (UICheckBox cb in UI.NetworkCheckboxes)
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

        public static List<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach (string name in Filters.NetworkFilters.Keys)
            {
                names.Add(name);
            }

            return names;
        }

        public static NetworkFilter GetFilter(Type ai)
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
            return Filters.NetworkFilters.GetValueSafe("NF-Other");
        }
    }
}

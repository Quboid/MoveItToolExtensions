using Harmony;
using MoveIt;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoveMore
{
    class Filters
    {
        static string[] SurfaceNames = new string[]
        {
            "ploppablegravel",
            "ploppablecliffgrass",
            "ploppableasphalt-prop"
        };


        public static bool IsSurface(PropInfo info)
        {
            if (Array.Exists(SurfaceNames, s => s.Equals(info.m_mesh.name))) {
                return true;
            }

            return false;
        }


        public static bool Filter(PropInfo info)
        {
            if (info.m_isDecal)
            {
                if (MoveItTool.filterDecals)
                {
                    return true;
                }
                return false;
            }

            if (IsSurface(info))
            {
                if (MoveMore.filterSurfaces)
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
            if (!MoveMore.filterNetworks) return true;

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
            MoveMore.NetworkFilters.GetValueSafe(name).enabled = e;
        }

        public static void ToggleFilter(string name)
        {
            MoveMore.NetworkFilters.GetValueSafe(name).enabled = !MoveMore.NetworkFilters.GetValueSafe(name).enabled;
            
            /*string msg = $"Toggling '{name}' ";
            foreach (KeyValuePair<string, NetworkFilter> pair in MoveMore.NetworkFilters)
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
            foreach (string name in MoveMore.NetworkFilters.Keys)
            {
                names.Add(name);
            }

            return names;
        }

        public static NetworkFilter GetFilter(Type ai)
        {
            foreach (NetworkFilter nf in MoveMore.NetworkFilters.Values)
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
            return MoveMore.NetworkFilters.GetValueSafe("NF-Other");
        }
    }
}

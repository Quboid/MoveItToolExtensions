using ColossalFramework.UI;
using UnityEngine;
using System;
using System.Reflection;

namespace MITE
{
    class DebugPanel_ModTools
    {
        public InstanceID Id { get; set; }
        public UIPanel Parent { get; set; }
        public UIButton btn;
        private readonly object ModTools, SceneExplorer;
        private readonly Type tModTools, tSceneExplorer, tReferenceChain;
        private readonly BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        private readonly object rcBuildings, rcProps, rcTrees, rcNodes, rcSegments;

        public DebugPanel_ModTools(UIPanel parent)
        {
            try
            {
                Assembly mtAssembly = null;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName.Length >= 12 && assembly.FullName.Substring(0, 12) == "000_ModTools")
                    {
                        mtAssembly = assembly;
                        break;
                    }
                }
                if (mtAssembly == null)
                {
                    return;
                }

                tModTools = mtAssembly.GetType("ModTools.ModTools");
                tSceneExplorer = mtAssembly.GetType("ModTools.SceneExplorer");
                tReferenceChain = mtAssembly.GetType("ModTools.ReferenceChain");

                ModTools = tModTools.GetField("instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                SceneExplorer = tModTools.GetField("sceneExplorer", flags).GetValue(ModTools);

                //Debug.Log($"\ntModTools:{tModTools}, tSceneExplorer:{tSceneExplorer}, tReferenceChain:{tReferenceChain}");
                //Debug.Log($"Fields:{tModTools.GetFields().Length}, Props:{tModTools.GetProperties().Length}, Methods:{tModTools.GetMethods().Length}");
                //Debug.Log($"{ModTools} ({ModTools.GetType()})\n{SceneExplorer} ({SceneExplorer.GetType()})");

                rcBuildings = Activator.CreateInstance(tReferenceChain);
                rcBuildings = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(GameObject) }, null).Invoke(rcBuildings, new object[] { BuildingManager.instance.gameObject });
                rcBuildings = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(BuildingManager) }, null).Invoke(rcBuildings, new object[] { BuildingManager.instance });
                rcBuildings = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcBuildings, new object[] { typeof(BuildingManager).GetField("m_buildings") });
                rcBuildings = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcBuildings, new object[] { typeof(Array16<Building>).GetField("m_buffer") });
                //Debug.Log($"rcBuildings:{rcBuildings}");

                rcProps = Activator.CreateInstance(tReferenceChain);
                rcProps = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(GameObject) }, null).Invoke(rcProps, new object[] { PropManager.instance.gameObject });
                rcProps = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(PropManager) }, null).Invoke(rcProps, new object[] { PropManager.instance });
                rcProps = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcProps, new object[] { typeof(PropManager).GetField("m_props") });
                rcProps = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcProps, new object[] { typeof(Array16<PropInstance>).GetField("m_buffer") });
                //Debug.Log($"rcProps:{rcProps}");

                rcTrees = Activator.CreateInstance(tReferenceChain);
                rcTrees = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(GameObject) }, null).Invoke(rcTrees, new object[] { TreeManager.instance.gameObject });
                rcTrees = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(TreeManager) }, null).Invoke(rcTrees, new object[] { TreeManager.instance });
                rcTrees = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcTrees, new object[] { typeof(TreeManager).GetField("m_trees") });
                rcTrees = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcTrees, new object[] { typeof(Array32<TreeInstance>).GetField("m_buffer") });
                //Debug.Log($"rcTrees:{rcTrees}");

                rcNodes = Activator.CreateInstance(tReferenceChain);
                rcNodes = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(GameObject) }, null).Invoke(rcNodes, new object[] { NetManager.instance.gameObject });
                rcNodes = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(NetManager) }, null).Invoke(rcNodes, new object[] { NetManager.instance });
                rcSegments = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcNodes, new object[] { typeof(NetManager).GetField("m_segments") });
                rcSegments = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcSegments, new object[] { typeof(Array16<NetSegment>).GetField("m_buffer") });
                rcNodes = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcNodes, new object[] { typeof(NetManager).GetField("m_nodes") });
                rcNodes = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(FieldInfo) }, null).Invoke(rcNodes, new object[] { typeof(Array16<NetNode>).GetField("m_buffer") });
                //Debug.Log($"rcNodes:{rcNodes}\nrcSegments:{rcSegments}");
            }
            catch (ReflectionTypeLoadException)
            {
                SceneExplorer = null;
                Debug.Log($"MITE failed to integrate ModTools (ReflectionTypeLoadException)");
            }
            catch (NullReferenceException)
            {
                SceneExplorer = null;
                Debug.Log($"MITE failed to integrate ModTools (NullReferenceException)");
            }

            if (SceneExplorer == null)
            {
                return;
            }

            Id = InstanceID.Empty;
            Parent = parent;
            btn = parent.AddUIComponent<UIButton>();
            btn.name = "MITE_ToModTools";
            btn.text = ">";
            btn.textScale = 0.7f;
            btn.tooltip = "Open in ModTools Scene Explorer";
            btn.size = new Vector2(20, 20);
            btn.textPadding = new RectOffset(2, 0, 5, 0);
            btn.relativePosition = new Vector3(parent.width - 24, parent.height - 24);
            btn.eventClicked += _toModTools;

            btn.atlas = ResourceLoader.GetAtlas("Ingame");
            btn.normalBgSprite = "OptionBase";
            btn.hoveredBgSprite = "OptionBaseHovered";
            btn.pressedBgSprite = "OptionBasePressed";
            btn.disabledBgSprite = "OptionBaseDisabled";
        }

        private void _toModTools(UIComponent c, UIMouseEventParameter p)
        {
            if (SceneExplorer == null)
            {
                return;
            }

            try
            {
                object rc;
                Type[] t = new Type[] { tReferenceChain };

                if (Id.Building > 0)
                {
                    rc = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(ushort) }, null).Invoke(rcBuildings, new object[] { Id.Building });
                    tSceneExplorer.GetMethod("ExpandFromRefChain", flags, null, t, null).Invoke(SceneExplorer, new object[] { rc });
                }
                else if (Id.Prop > 0)
                {
                    rc = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(ushort) }, null).Invoke(rcProps, new object[] { Id.Prop });
                    tSceneExplorer.GetMethod("ExpandFromRefChain", flags, null, t, null).Invoke(SceneExplorer, new object[] { rc });
                }
                else if (Id.Tree > 0)
                {
                    rc = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(uint) }, null).Invoke(rcTrees, new object[] { Id.Tree });
                    tSceneExplorer.GetMethod("ExpandFromRefChain", flags, null, t, null).Invoke(SceneExplorer, new object[] { rc });
                }
                else if (Id.NetNode > 0)
                {
                    rc = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(ushort) }, null).Invoke(rcNodes, new object[] { Id.NetNode });
                    tSceneExplorer.GetMethod("ExpandFromRefChain", flags, null, t, null).Invoke(SceneExplorer, new object[] { rc });
                }
                else if (Id.NetSegment > 0)
                {
                    rc = tReferenceChain.GetMethod("Add", flags, null, new Type[] { typeof(ushort) }, null).Invoke(rcSegments, new object[] { Id.NetSegment });
                    tSceneExplorer.GetMethod("ExpandFromRefChain", flags, null, t, null).Invoke(SceneExplorer, new object[] { rc });
                }

                tSceneExplorer.GetProperty("visible", BindingFlags.Public | BindingFlags.Instance).SetValue(SceneExplorer, true, null);
            }
            catch (ReflectionTypeLoadException)
            {
                Debug.Log($"MITE failed to call ModTools (ReflectionTypeLoadException)");
            }
            catch (NullReferenceException)
            {
                Debug.Log($"MITE failed to call ModTools (NullReferenceException)");
            }
        }
    }
}


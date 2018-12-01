using ColossalFramework.UI;
using Harmony;
using ModTools;
using MoveIt;
using System;
using UnityEngine;

namespace MITE
{
    class DebugPanel
    {
        public UIPanel Panel;//, HoverInfo;
        public UIButton ToModTools;
        private UILabel HoverLarge, HoverSmall;
        private InstanceID id, lastId;
        private SceneExplorer scene;
        private ReferenceChain buildingsBufferRefChain, nodesBufferRefChain, segmentsBufferRefChain;


        public DebugPanel()
        {
            _initialise();

            try
            {
                scene = Traverse.Create(ModTools.ModTools.Instance).Field("sceneExplorer").GetValue<SceneExplorer>();
                buildingsBufferRefChain = new ReferenceChain()
                    .Add(BuildingManager.instance.gameObject)
                    .Add(BuildingManager.instance)
                    .Add(typeof(BuildingManager).GetField("m_buildings"))
                    .Add(typeof(Array16<Building>).GetField("m_buffer"));
                nodesBufferRefChain = new ReferenceChain()
                    .Add(NetManager.instance.gameObject)
                    .Add(NetManager.instance)
                    .Add(typeof(NetManager).GetField("m_nodes"))
                    .Add(typeof(Array16<Building>).GetField("m_buffer"));
                segmentsBufferRefChain = new ReferenceChain()
                    .Add(NetManager.instance.gameObject)
                    .Add(NetManager.instance)
                    .Add(typeof(NetManager).GetField("m_segments"))
                    .Add(typeof(Array16<Building>).GetField("m_buffer"));
            }
            catch (NullReferenceException ex)
            {
                scene = null;
                Debug.Log($"{ex}");
            }
        }


        public void Visible(bool show)
        {
            Panel.isVisible = show;
        }


        public void UpdateVisible()
        {
            if (MITE.Settings.ShowDebugPanel)
            {
                Panel.isVisible = true;
            }
            else
            {
                Panel.isVisible = false;
            }
        }


        public void Update(InstanceID instanceId)
        {
            id = instanceId;
            if (!MITE.Settings.ShowDebugPanel)
            {
                return;
            }

            if (id == null)
            {
                return;
            }
            if (id == InstanceID.Empty)
            {
                HoverLarge.textColor = new Color32(255, 255, 255, 255);
                return;
            }
            if (lastId == id)
            {
                return;
            }

            HoverLarge.textColor = new Color32(127, 217, 255, 255);
            HoverLarge.text = "";
            HoverSmall.text = "";

            if (id.Building > 0)
            {
                BuildingInfo info = BuildingManager.instance.m_buildings.m_buffer[id.Building].Info;
                HoverLarge.text = $"B:{id.Building}  {info.name}";
                HoverSmall.text = $"{info.GetType()} ({info.GetAI().GetType()})\n{info.m_class.name}\n({info.m_class.m_service}.{info.m_class.m_subService})";
                ToModTools.isVisible = true;
            }
            if (id.Prop > 0)
            {
                string type = "P";
                PropInfo info = PropManager.instance.m_props.m_buffer[id.Prop].Info;
                if (info.m_isDecal) type = "D";
                HoverLarge.text = $"{type}:{id.Prop}  {info.name}";
                HoverSmall.text = $"{info.GetType()}\n{info.m_class.name}";
                ToModTools.isVisible = false;
            }
            if (id.Tree > 0)
            {
                TreeInfo info = TreeManager.instance.m_trees.m_buffer[id.Tree].Info;
                HoverLarge.text = $"T:{id.Tree}  {info.name}";
                HoverSmall.text = $"{info.GetType()}\n{info.m_class.name}";
                ToModTools.isVisible = false;
            }
            if (id.NetNode > 0)
            {
                NetInfo info = NetManager.instance.m_nodes.m_buffer[id.NetNode].Info;
                HoverLarge.text = $"N:{id.NetNode}  {info.name}";
                HoverSmall.text = $"{info.GetType()} ({info.GetAI().GetType()})\n{info.m_class.name}";
                ToModTools.isVisible = true;
            }
            if (id.NetSegment > 0)
            {
                NetInfo info = NetManager.instance.m_segments.m_buffer[id.NetSegment].Info;
                HoverLarge.text = $"S:{id.NetSegment}  {info.name}";
                HoverSmall.text = $"{info.GetType()} ({info.GetAI().GetType()})\n{info.m_class.name}";
                ToModTools.isVisible = true;
            }


        }


        private void _toModTools(UIComponent c, UIMouseEventParameter p)
        {
            if (id.Building > 0)
            {
                scene.ExpandFromRefChain(buildingsBufferRefChain.Add(id.Building));
            }
            else if (id.NetNode > 0)
            {
                scene.ExpandFromRefChain(nodesBufferRefChain.Add(id.NetNode));
            }
            else if (id.NetSegment > 0)
            {
                scene.ExpandFromRefChain(segmentsBufferRefChain.Add(id.NetSegment));
            }
            scene.visible = true;
        }


        private void _initialise()
        {
            Debug.Log($"A");
            Panel = UIView.GetAView().AddUIComponent(typeof(UIPanel)) as UIPanel;
            Panel.name = "MITE_DebugPanel";
            Panel.atlas = ResourceLoader.GetAtlas("Ingame");
            Panel.backgroundSprite = "SubcategoriesPanel";
            Panel.size = new Vector2(260, 62);
            Panel.absolutePosition = new Vector3(Panel.GetUIView().GetScreenResolution().x - 330, 3);
            Panel.clipChildren = true;
            Panel.isVisible = MITE.Settings.ShowDebugPanel;

            HoverLarge = Panel.AddUIComponent<UILabel>();
            HoverLarge.textScale = 0.8f;
            HoverLarge.text = "None";
            HoverLarge.relativePosition = new Vector3(5, 6);
            HoverLarge.width = HoverLarge.parent.width - 20;
            HoverLarge.clipChildren = true;
            HoverLarge.useDropShadow = true;
            HoverLarge.dropShadowOffset = new Vector2(2, -2);

            HoverSmall = Panel.AddUIComponent<UILabel>();
            HoverSmall.textScale = 0.65f;
            HoverSmall.text = "No item being hovered\n ";
            HoverSmall.relativePosition = new Vector3(5, 24);
            HoverSmall.width = HoverSmall.parent.width - 20;
            HoverSmall.clipChildren = true;
            HoverSmall.useDropShadow = true;
            HoverSmall.dropShadowOffset = new Vector2(1, -1);

            ToModTools = Panel.AddUIComponent<UIButton>();
            ToModTools.name = "MITE_ToModTools";
            ToModTools.text = "MT";
            ToModTools.size = new Vector2(24, 16);
            ToModTools.relativePosition = new Vector3(Panel.width - 30, 2);
            ToModTools.eventClicked += _toModTools;

            Debug.Log($"absolutePosition={Panel.absolutePosition}");
        }
    }
}

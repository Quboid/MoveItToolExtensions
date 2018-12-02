using ModTools;
using Harmony;
using ColossalFramework.UI;
using UnityEngine;

namespace MITE
{
    class DebugPanel_ModTools
    {
        public InstanceID Id { get; set; }
        public UIPanel Parent { get; set; }

        public UIButton btn;

        public DebugPanel_ModTools(UIPanel parent)
        {
            if (MITE.Enable_ModTools)
            {
                Id = InstanceID.Empty;
                Parent = parent;
                btn = parent.AddUIComponent<UIButton>();
                btn.name = "MITE_ToModTools";
                btn.text = ">";
                btn.tooltip = "Open item in ModTools' scene explorer";
                btn.size = new Vector2(16, 16);
                btn.relativePosition = new Vector3(parent.width - 26, 4);

                // Disabled ModTools Inegration
                /*
                        btn.eventClicked += _toModTools;

                        _ModTools = Traverse.Create(ModTools.ModTools.Instance).Field("sceneExplorer").GetValue<SceneExplorer>();

                        buildingsBufferRefChain = new ReferenceChain()
                            .Add(BuildingManager.instance.gameObject)
                            .Add(BuildingManager.instance)
                            .Add(typeof(BuildingManager).GetField("m_buildings"))
                            .Add(typeof(Array16<Building>).GetField("m_buffer"));
                        propsBufferRefChain = new ReferenceChain()
                            .Add(PropManager.instance.gameObject)
                            .Add(PropManager.instance)
                            .Add(typeof(PropManager).GetField("m_props"))
                            .Add(typeof(Array16<PropInstance>).GetField("m_buffer"));
                        treesBufferRefChain = new ReferenceChain()
                            .Add(TreeManager.instance.gameObject)
                            .Add(TreeManager.instance)
                            .Add(typeof(TreeManager).GetField("m_trees"))
                            .Add(typeof(Array32<TreeInstance>).GetField("m_buffer"));
                        nodesBufferRefChain = new ReferenceChain()
                            .Add(NetManager.instance.gameObject)
                            .Add(NetManager.instance)
                            .Add(typeof(NetManager).GetField("m_nodes"))
                            .Add(typeof(Array16<NetNode>).GetField("m_buffer"));
                        segmentsBufferRefChain = new ReferenceChain()
                            .Add(NetManager.instance.gameObject)
                            .Add(NetManager.instance)
                            .Add(typeof(NetManager).GetField("m_segments"))
                            .Add(typeof(Array16<NetSegment>).GetField("m_buffer"));
                    }

                    private SceneExplorer _ModTools = null;
                    private ReferenceChain buildingsBufferRefChain, propsBufferRefChain, treesBufferRefChain, nodesBufferRefChain, segmentsBufferRefChain;


                    private void _toModTools(UIComponent c, UIMouseEventParameter p)
                    {
                        if (_ModTools == null)
                        {
                            return;
                        }

                        if (Id.Building > 0)
                        {
                            _ModTools.ExpandFromRefChain(buildingsBufferRefChain.Add(Id.Building));
                        }
                        else if (Id.Prop > 0)
                        {
                            _ModTools.ExpandFromRefChain(propsBufferRefChain.Add(Id.Prop));
                        }
                        else if (Id.Tree > 0)
                        {
                            _ModTools.ExpandFromRefChain(treesBufferRefChain.Add((int)Id.Tree));
                        }
                        else if (Id.NetNode > 0)
                        {
                            _ModTools.ExpandFromRefChain(nodesBufferRefChain.Add(Id.NetNode));
                        }
                        else if (Id.NetSegment > 0)
                        {
                            _ModTools.ExpandFromRefChain(segmentsBufferRefChain.Add(Id.NetSegment));
                        }
                        _ModTools.visible = true;*/
            }
        }
    }
}

  
using ColossalFramework.UI;
using Harmony;
using MoveIt;
using UnityEngine;

namespace MoveMore
{
    [HarmonyPatch(typeof(UIToolOptionPanel))]
    [HarmonyPatch("RefreshAlignHeightButton")]
    class UITOP_RefreshAlignHeightButton
    {
        private static UIButton m_alignTools;

        public static bool Prefix()
        {
            UIToolOptionPanel instance = UIToolOptionPanel.instance;
            m_alignTools = Traverse.Create(instance).Field("m_alignHeight").GetValue<UIButton>();

            if (UIToolOptionPanel.instance != null && m_alignTools != null && MoveItTool.instance != null)
            {
                if (MoveItTool.instance.toolState == MoveItTool.ToolState.AligningHeights)
                {
                    m_alignTools.normalBgSprite = "OptionBaseFocused";
                }
                else
                {
                    m_alignTools.normalBgSprite = "OptionBase";
                }
            }

            return false;
        }
    }


    [HarmonyPatch(typeof(UIToolOptionPanel))]
    [HarmonyPatch("Start")]
    class UITOP_Start
    {
        private static UIButton m_marquee;
        private static UIButton m_alignTools;

        public static void Postfix(UIToolOptionPanel __instance, ref UIButton ___m_marquee, ref UIButton ___m_alignHeight)
        {
            UIPanel filtersPanel, alignToolsPanel;
            m_marquee = ___m_marquee;
            m_alignTools = ___m_alignHeight;
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

            void OnDoubleClickNetworkFilter(UIComponent c, UIMouseEventParameter p)
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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

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
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            filtersPanel.padding = new RectOffset(10, 10, 10, 10);
            filtersPanel.autoLayoutDirection = LayoutDirection.Vertical;
            filtersPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
            filtersPanel.autoLayout = true;
            filtersPanel.height = 210;
            filtersPanel.absolutePosition = m_marquee.absolutePosition - new Vector3(0, filtersPanel.height + 5);


            m_marquee.eventDoubleClick += (UIComponent c, UIMouseEventParameter p) =>
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


            m_alignTools.tooltip = "Alignment Tools";
            m_alignTools.atlas = UI.GetIconsAtlas(__instance);
            m_alignTools.normalFgSprite = "AlignTools";

            alignToolsPanel = __instance.AddUIComponent<UIPanel>();
            UI.AlignToolsPanel = alignToolsPanel;
            alignToolsPanel.clipChildren = true;
            alignToolsPanel.autoLayout = false;
            alignToolsPanel.size = new Vector2(36, 160);
            alignToolsPanel.isVisible = true;
            alignToolsPanel.absolutePosition = m_alignTools.absolutePosition + new Vector3(5, 10 - alignToolsPanel.height);
            alignToolsPanel.zOrder = __instance.zOrder + 1;

            UIPanel atpBackground = alignToolsPanel.AddUIComponent<UIPanel>();
            atpBackground.atlas = UI.GetIconsAtlas(__instance);
            atpBackground.backgroundSprite = "ColumnBG";
            atpBackground.relativePosition = new Vector3(5, 5);
            atpBackground.width = atpBackground.parent.width - 10;
            atpBackground.opacity = 0.8f;

            UIPanel atpContainer = alignToolsPanel.AddUIComponent<UIPanel>();
            atpContainer.autoLayoutDirection = LayoutDirection.Vertical;
            atpContainer.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
            atpContainer.autoLayout = true;
            atpContainer.relativePosition = Vector3.zero;

            UI.AlignButtons.Add("AlignRandom", atpContainer.AddUIComponent<UIButton>());
            UIButton alignRandom = UI.AlignButtons.GetValueSafe("AlignRandom");
            alignRandom.name = "AlignRandom";
            alignRandom.atlas = UI.GetIconsAtlas(alignToolsPanel);
            alignRandom.tooltip = "Immediate rotate valid items randomly";
            alignRandom.playAudioEvents = true;
            alignRandom.size = new Vector2(36, 36);
            alignRandom.normalBgSprite = "OptionBase";
            alignRandom.hoveredBgSprite = "OptionBaseHovered";
            alignRandom.pressedBgSprite = "OptionBasePressed";
            alignRandom.disabledBgSprite = "OptionBaseDisabled";
            alignRandom.normalFgSprite = "AlignRandom";

            UI.AlignButtons.Add("AlignAll", atpContainer.AddUIComponent<UIButton>());
            UIButton alignAll = UI.AlignButtons.GetValueSafe("AlignAll");
            alignAll.name = "AlignAll";
            alignAll.atlas = UI.GetIconsAtlas(alignToolsPanel);
            alignAll.tooltip = "Align rotation all around single point";
            alignAll.playAudioEvents = true;
            alignAll.size = new Vector2(36, 36);
            alignAll.normalBgSprite = "OptionBase";
            alignAll.hoveredBgSprite = "OptionBaseHovered";
            alignAll.pressedBgSprite = "OptionBasePressed";
            alignAll.disabledBgSprite = "OptionBaseDisabled";
            alignAll.normalFgSprite = "AlignAll";

            UI.AlignButtons.Add("AlignEach", atpContainer.AddUIComponent<UIButton>());
            UIButton alignEach = UI.AlignButtons.GetValueSafe("AlignEach");
            alignEach.name = "AlignEach";
            alignEach.atlas = UI.GetIconsAtlas(alignToolsPanel);
            alignEach.tooltip = "Align rotation individually";
            alignEach.playAudioEvents = true;
            alignEach.size = new Vector2(36, 36);
            alignEach.normalBgSprite = "OptionBase";
            alignEach.hoveredBgSprite = "OptionBaseHovered";
            alignEach.pressedBgSprite = "OptionBasePressed";
            alignEach.disabledBgSprite = "OptionBaseDisabled";
            alignEach.normalFgSprite = "AlignEach";

            UI.AlignButtons.Add("AlignHeight", atpContainer.AddUIComponent<UIButton>());
            UIButton alignHeight = UI.AlignButtons.GetValueSafe("AlignHeight");
            alignHeight.name = "AlignHeight";
            alignHeight.atlas = UI.GetIconsAtlas(alignToolsPanel);
            alignHeight.tooltip = "Align height";
            alignHeight.playAudioEvents = true;
            alignHeight.size = new Vector2(36, 36);
            alignHeight.normalBgSprite = "OptionBase";
            alignHeight.hoveredBgSprite = "OptionBaseHovered";
            alignHeight.pressedBgSprite = "OptionBasePressed";
            alignHeight.disabledBgSprite = "OptionBaseDisabled";
            alignHeight.normalFgSprite = "AlignHeight";
        }
    }
}

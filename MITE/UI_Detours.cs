using ColossalFramework.UI;
using Harmony;
using MoveIt;
using UnityEngine;

namespace MITE
{
    [HarmonyPatch(typeof(UIToolOptionPanel))]
    [HarmonyPatch("Start")]
    class UITOP_Start
    {
        public static void Postfix(UIToolOptionPanel __instance, ref UIButton ___m_marquee, ref UIButton ___m_alignHeight, ref UIButton ___m_single, ref UIButton ___m_copy, ref UIButton ___m_bulldoze, ref UITabstrip ___m_tabStrip)
        {
            UIPanel filtersPanel, alignToolsPanel;
            ___m_alignHeight.isVisible = false;
            ___m_alignHeight.enabled = false;

            __instance.RemoveUIComponent(__instance.filtersPanel);
            Traverse _UITOP = Traverse.Create(__instance);

            filtersPanel = __instance.filtersPanel = __instance.AddUIComponent(typeof(UIPanel)) as UIPanel;
            #region Filter Panel
            filtersPanel.atlas = SamsamTS.UIUtils.GetAtlas("Ingame");
            filtersPanel.backgroundSprite = "SubcategoriesPanel";
            filtersPanel.clipChildren = true;
            UI.FilterPanel = filtersPanel;

            filtersPanel.size = new Vector2(150, 140);
            filtersPanel.isVisible = false;

            void OnDoubleClick(UIComponent c, UIMouseEventParameter p)
            {
                foreach (UICheckBox cb in UI.FilterCBs)
                {
                    cb.isChecked = false;
                    Filters.SetFilter(cb.name, false);
                }
                ((UICheckBox)c).isChecked = true;
                Filters.SetFilter(c.name, true);

                UI.RefreshFilters();
            }

            UICheckBox checkBox = UI.CreateFilterCB(filtersPanel, "Buildings");
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = UI.CreateFilterCB(filtersPanel, "Props");
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = UI.CreateFilterCB(filtersPanel, "Decals");
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = UI.CreateFilterCB(filtersPanel, "Surfaces");
            checkBox.eventDoubleClick += OnDoubleClick;;

            checkBox = UI.CreateFilterCB(filtersPanel, "Trees");
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = UI.CreateFilterCB(filtersPanel, "Nodes");
            checkBox.eventDoubleClick += OnDoubleClick;

            checkBox = UI.CreateFilterCB(filtersPanel, "Segments");
            checkBox.eventDoubleClick += OnDoubleClick;


            #region Network Filters
            UIButton btnNetworks = UI.CreateToggleNFBtn();
            void OnDoubleClickNetworkFilter(UIComponent c, UIMouseEventParameter p)
            {
                foreach (UICheckBox cb in UI.NetworkCBs)
                {
                    cb.isChecked = false;
                    Filters.SetNetworkFilter(cb.name, false);
                }
                ((UICheckBox)c).isChecked = true;
                Filters.SetNetworkFilter(c.name, true);

                UI.RefreshFilters();
            }

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Roads");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Tracks");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Paths");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Fences");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Powerlines");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            checkBox = UI.CreateNetworkFilterCB(filtersPanel, "Others");
            checkBox.eventDoubleClick += OnDoubleClickNetworkFilter;

            UI.RefreshFilters();
            #endregion

            filtersPanel.padding = new RectOffset(10, 10, 10, 10);
            filtersPanel.autoLayoutDirection = LayoutDirection.Vertical;
            filtersPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
            filtersPanel.autoLayout = true;
            filtersPanel.height = 210;
            filtersPanel.absolutePosition = ___m_marquee.absolutePosition + new Vector3(-47, -5 - filtersPanel.height);
            #endregion

            ___m_marquee.eventDoubleClick += (UIComponent c, UIMouseEventParameter p) =>
            {
                bool newChecked = false;
                foreach (UICheckBox cb in UI.FilterCBs)
                {
                    if (!cb.isChecked)
                    {
                        newChecked = true;
                        break;
                    }
                }
                if (!newChecked)
                {
                    foreach (UICheckBox cb in UI.NetworkCBs)
                    {
                        if (!cb.isChecked)
                        {
                            newChecked = true;
                            break;
                        }
                    }
                }


                foreach (UICheckBox cb in UI.FilterCBs)
                {
                    cb.isChecked = newChecked;
                    Filters.SetFilter(cb.name, newChecked);
                }
                foreach (UICheckBox cb in UI.NetworkCBs)
                {
                    cb.isChecked = newChecked;
                    Filters.SetNetworkFilter(cb.name, newChecked);
                }

                UI.RefreshFilters();
            };

            UI.AlignToolsBtn = __instance.AddUIComponent<UIButton>();
            #region AlignTool Panel
            ___m_copy.relativePosition -= new Vector3(___m_alignHeight.width, 0);
            ___m_bulldoze.relativePosition -= new Vector3(___m_alignHeight.width, 0);
            UI.AlignToolsBtn.relativePosition = new Vector3(___m_bulldoze.relativePosition.x + ___m_bulldoze.width, 0);
            UI.AlignToolsBtn.name = "AlignToolsBtn";
            UI.AlignToolsBtn.tooltip = "Alignment Tools";
            UI.AlignToolsBtn.atlas = UI.GetIconsAtlas();
            UI.AlignToolsBtn.normalFgSprite = "AlignTools";
            UI.AlignToolsBtn.group = ___m_tabStrip;
            UI.AlignToolsBtn.playAudioEvents = true;
            UI.AlignToolsBtn.size = new Vector2(36, 36);
            UI.AlignToolsBtn.normalBgSprite = "OptionBase";
            UI.AlignToolsBtn.hoveredBgSprite = "OptionBaseHovered";
            UI.AlignToolsBtn.pressedBgSprite = "OptionBasePressed";
            UI.AlignToolsBtn.disabledBgSprite = "OptionBaseDisabled";
            UI.AlignToolsBtn.eventClicked += UI.AlignToolsClicked;

            alignToolsPanel = __instance.AddUIComponent<UIPanel>();
            UI.AlignToolsPanel = alignToolsPanel;
            alignToolsPanel.autoLayout = false;
            alignToolsPanel.clipChildren = true;
            alignToolsPanel.size = new Vector2(36, 166);
            alignToolsPanel.isVisible = false;
            alignToolsPanel.absolutePosition = UI.AlignToolsBtn.absolutePosition + new Vector3(0, 10 - alignToolsPanel.height);
            UI.AlignToolsBtn.zOrder = alignToolsPanel.zOrder + 10;

            UIPanel atpBackground = alignToolsPanel.AddUIComponent<UIPanel>();
            atpBackground.size = new Vector2(26, 166);
            atpBackground.clipChildren = true;
            atpBackground.relativePosition = new Vector3(5, 10);
            atpBackground.atlas = SamsamTS.UIUtils.GetAtlas("Ingame");
            atpBackground.backgroundSprite = "InfoPanelBack";

            UIPanel atpContainer = alignToolsPanel.AddUIComponent<UIPanel>();
            atpContainer.autoLayoutDirection = LayoutDirection.Vertical;
            atpContainer.autoLayoutPadding = new RectOffset(0, 0, 0, 3);
            atpContainer.autoLayout = true;
            atpContainer.relativePosition = Vector3.zero;

            UI.AlignButtons.Add("AlignRandom", atpContainer.AddUIComponent<UIButton>());
            UIButton alignRandom = UI.AlignButtons.GetValueSafe("AlignRandom");
            alignRandom.name = "AlignRandom";
            alignRandom.atlas = UI.GetIconsAtlas();
            alignRandom.tooltip = "Immediate rotate valid items randomly";
            alignRandom.playAudioEvents = true;
            alignRandom.size = new Vector2(36, 36);
            alignRandom.normalBgSprite = "OptionBase";
            alignRandom.hoveredBgSprite = "OptionBaseHovered";
            alignRandom.pressedBgSprite = "OptionBasePressed";
            alignRandom.disabledBgSprite = "OptionBaseDisabled";
            alignRandom.normalFgSprite = "AlignRandom";
            alignRandom.eventClicked += UI.AlignToolsClicked;

            UI.AlignButtons.Add("AlignGroup", atpContainer.AddUIComponent<UIButton>());
            UIButton alignGroup = UI.AlignButtons.GetValueSafe("AlignGroup");
            alignGroup.name = "AlignGroup";
            alignGroup.atlas = UI.GetIconsAtlas();
            alignGroup.tooltip = "Align rotation all around single point";
            alignGroup.playAudioEvents = true;
            alignGroup.size = new Vector2(36, 36);
            alignGroup.normalBgSprite = "OptionBase";
            alignGroup.hoveredBgSprite = "OptionBaseHovered";
            alignGroup.pressedBgSprite = "OptionBasePressed";
            alignGroup.disabledBgSprite = "OptionBaseDisabled";
            alignGroup.normalFgSprite = "AlignGroup";
            alignGroup.eventClicked += UI.AlignToolsClicked;

            UI.AlignButtons.Add("AlignIndividual", atpContainer.AddUIComponent<UIButton>());
            UIButton alignIndividual = UI.AlignButtons.GetValueSafe("AlignIndividual");
            alignIndividual.name = "AlignIndividual";
            alignIndividual.atlas = UI.GetIconsAtlas();
            alignIndividual.tooltip = "Align rotation individually";
            alignIndividual.playAudioEvents = true;
            alignIndividual.size = new Vector2(36, 36);
            alignIndividual.normalBgSprite = "OptionBase";
            alignIndividual.hoveredBgSprite = "OptionBaseHovered";
            alignIndividual.pressedBgSprite = "OptionBasePressed";
            alignIndividual.disabledBgSprite = "OptionBaseDisabled";
            alignIndividual.normalFgSprite = "AlignIndividual";
            alignIndividual.eventClicked += UI.AlignToolsClicked;

            UI.AlignButtons.Add("AlignHeight", atpContainer.AddUIComponent<UIButton>());
            UIButton alignHeight = UI.AlignButtons.GetValueSafe("AlignHeight");
            alignHeight.name = "AlignHeight";
            alignHeight.atlas = _UITOP.Method("GetIconsAtlas").GetValue<UITextureAtlas>();
            alignHeight.tooltip = "Align height";
            alignHeight.playAudioEvents = true;
            alignHeight.size = new Vector2(36, 36);
            alignHeight.normalBgSprite = "OptionBase";
            alignHeight.hoveredBgSprite = "OptionBaseHovered";
            alignHeight.pressedBgSprite = "OptionBasePressed";
            alignHeight.disabledBgSprite = "OptionBaseDisabled";
            alignHeight.normalFgSprite = "AlignHeight";
            alignHeight.eventClicked += UI.AlignToolsClicked;
            #endregion

            UI.DbgPanel = new DebugPanel();
        }
    }
}

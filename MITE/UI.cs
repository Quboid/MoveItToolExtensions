using ColossalFramework.UI;
using Harmony;
using MoveIt;
using System.Collections.Generic;
using UnityEngine;

namespace MITE
{
    class UI
    {
        public static List<UICheckBox> NetworkCheckboxes = new List<UICheckBox>();
        public static UIButton ToggleNF, AlignToolsBtn;
        public static UIPanel FilterPanel, AlignToolsPanel;
        public static Dictionary<string, UIButton> AlignButtons = new Dictionary<string, UIButton>();
        public static Color32 TextColor = new Color32(175, 216, 235, 255);

        public static void AlignToolsClicked(UIComponent c, UIMouseEventParameter p)
        {
            switch (c.name)
            {
                case "AlignToolsBtn":
                    //MITE.AlignMode = MITE.AlignModes.Off;

                    if (AlignToolsPanel.isVisible)
                    {
                        AlignToolsPanel.isVisible = false;
                        //MITE.AlignMode = MITE.AlignModes.Off;
                    }
                    else
                    {
                        AlignToolsPanel.isVisible = true;
                    }
                    UpdateAlignTools();
                    break;

                case "AlignHeight":
                    if (MITE.AlignMode == MITE.AlignModes.Height)
                    {
                        MITE.AlignMode = MITE.AlignModes.Off;
                        MoveItTool.instance.toolState = MoveItTool.ToolState.Default;
                    }
                    else
                    {
                        MoveItTool.instance.StartAligningHeights();
                        if (MoveItTool.instance.toolState == MoveItTool.ToolState.AligningHeights)
                        { // Change MITE's mode only if MoveIt changed to AligningHeights
                            MITE.AlignMode = MITE.AlignModes.Height;
                        }
                    }
                    //AlignToolsPanel.isVisible = false;
                    UpdateAlignTools();
                    break;

                case "AlignIndividual":
                    if (MITE.AlignMode == MITE.AlignModes.Individual)
                    {
                        MITE.AlignMode = MITE.AlignModes.Off;
                    }
                    else
                    {
                        if (MoveItTool.instance.toolState == MoveItTool.ToolState.Cloning || MoveItTool.instance.toolState == MoveItTool.ToolState.RightDraggingClone)
                        {
                            MoveItTool.instance.StopCloning();
                        }

                        if (Action.selection.Count > 0)
                        {
                            MITE.AlignMode = MITE.AlignModes.Individual;
                        }
                    }
                    //AlignToolsPanel.isVisible = false;
                    UpdateAlignTools();
                    break;

                case "AlignGroup":
                    if (MITE.AlignMode == MITE.AlignModes.Group)
                    {
                        MITE.AlignMode = MITE.AlignModes.Off;
                    }
                    else
                    {
                        if (MoveItTool.instance.toolState == MoveItTool.ToolState.Cloning || MoveItTool.instance.toolState == MoveItTool.ToolState.RightDraggingClone)
                        {
                            MoveItTool.instance.StopCloning();
                        }

                        if (Action.selection.Count > 0)
                        {
                            MITE.AlignMode = MITE.AlignModes.Group;
                        }
                    }
                    //AlignToolsPanel.isVisible = false;
                    UpdateAlignTools();
                    break;

                case "AlignRandom":
                    MITE.AlignMode = MITE.AlignModes.Random;

                    if (MoveItTool.instance.toolState == MoveItTool.ToolState.Cloning || MoveItTool.instance.toolState == MoveItTool.ToolState.RightDraggingClone)
                    {
                        MoveItTool.instance.StopCloning();
                    }

                    AlignRotationAction action = new AlignRandomAction();
                    action.followTerrain = MoveItTool.followTerrain;
                    ActionQueue.instance.Push(action);
                    ActionQueue.instance.Do();
                    MITE.DeactivateAlignTool();

                    //AlignToolsPanel.isVisible = false;
                    UpdateAlignTools();
                    break;
            }
            //Debug.Log($"{c.name} clicked, mode is {MITE.AlignMode}");
        }


        public static void UpdateAlignTools()
        {
            //string msg = "";
            AlignToolsBtn.atlas = AlignButtons.GetValueSafe("AlignGroup").atlas;
            AlignToolsBtn.normalFgSprite = "AlignTools";
            foreach (UIButton btn in AlignButtons.Values)
            {
                //msg = msg + $"\n{btn.name} ({MITE.AlignMode},{MoveItTool.instance.toolState})";
                btn.normalBgSprite = "OptionBase";
            }
            //Debug.Log(msg);

            switch (MITE.AlignMode)
            {
                case MITE.AlignModes.Height:
                    if (!AlignToolsPanel.isVisible)
                    {
                        AlignToolsBtn.atlas = AlignButtons.GetValueSafe("AlignHeight").atlas;
                        AlignToolsBtn.normalFgSprite = "AlignHeight";
                    }
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    AlignButtons.GetValueSafe("AlignHeight").normalBgSprite = "OptionBaseFocused";
                    break;

                case MITE.AlignModes.Individual:
                    AlignToolsBtn.atlas = AlignButtons.GetValueSafe("AlignIndividual").atlas;
                    if (!AlignToolsPanel.isVisible) AlignToolsBtn.normalFgSprite = "AlignIndividual";
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    AlignButtons.GetValueSafe("AlignIndividual").normalBgSprite = "OptionBaseFocused";
                    break;

                case MITE.AlignModes.Group:
                    AlignToolsBtn.atlas = AlignButtons.GetValueSafe("AlignGroup").atlas;
                    if (!AlignToolsPanel.isVisible) AlignToolsBtn.normalFgSprite = "AlignGroup";
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    AlignButtons.GetValueSafe("AlignGroup").normalBgSprite = "OptionBaseFocused";
                    break;
                
                // Random mode is instant, button isn't relevant
                default:
                    if (AlignToolsPanel.isVisible)
                    {
                        AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    }
                    else
                    {
                        AlignToolsBtn.normalBgSprite = "OptionBase";
                    }
                    break;
            }
        }


        public static UITextureAtlas GetIconsAtlas()
        {
            Texture2D[] textures =
            {
                UIToolOptionPanel.instance.atlas["OptionBase"].texture,
                UIToolOptionPanel.instance.atlas["OptionBaseHovered"].texture,
                UIToolOptionPanel.instance.atlas["OptionBasePressed"].texture,
                UIToolOptionPanel.instance.atlas["OptionBaseDisabled"].texture,
                UIToolOptionPanel.instance.atlas["OptionBaseFocused"].texture
            };

            string[] spriteNames = new string[]
            {
                "AlignTools",
                "AlignIndividual",
                "AlignGroup",
                "AlignRandom",
                "NFExpand",
                "NFExpandHover",
                "NFCollapse",
                "NFCollapseHover"
            };

            UITextureAtlas loadedAtlas = ResourceLoader.CreateTextureAtlas("MITE", spriteNames, "MITE.Icons.");
            ResourceLoader.AddTexturesInAtlas(loadedAtlas, textures);

            return loadedAtlas;
        }


        public static UIButton CreateToggleNF()
        {
            ToggleNF = SamsamTS.UIUtils.CreateButton(FilterPanel);
            ToggleNF.textHorizontalAlignment = UIHorizontalAlignment.Center;
            ToggleNF.textColor = TextColor;
            ToggleNF.disabledTextColor = TextColor;
            ToggleNF.focusedTextColor = TextColor;
            ToggleNF.pressedTextColor = TextColor;
            ToggleNF.autoSize = false;
            ToggleNF.width = 130f;
            ToggleNF.height = 16f;
            ToggleNF.horizontalAlignment = UIHorizontalAlignment.Center;
            ToggleNF.relativePosition = new Vector2(10f, 0f);
            ToggleNF.atlas = GetIconsAtlas();
            ToggleNF.normalBgSprite = null;
            ToggleNF.hoveredBgSprite = null;
            ToggleNF.pressedBgSprite = null;
            ToggleNF.disabledBgSprite = null;
            ToggleNF.normalFgSprite = "NFExpand";
            ToggleNF.hoveredFgSprite = "NFExpandHover";
            ToggleNF.tooltip = "Network Filters";
            ToggleNF.eventClicked += (c, p) =>
            {
                ToggleNetworks();
            };

            _updateToggleNF();
            return ToggleNF;
        }


        private static void _updateToggleNF()
        {
            if (MITE.filterNetworks)
            { // Network Filters visible
                ToggleNF.normalFgSprite = "NFCollapse";
                ToggleNF.hoveredFgSprite = "NFCollapseHover";
            }
            else
            { // Network Filters hidden
                ToggleNF.normalFgSprite = "NFExpand";
                ToggleNF.hoveredFgSprite = "NFExpandHover";
            }
        }


        public static UILabel CreateSeparator(UIComponent parent, float height)
        {
            UILabel separator;
            separator = parent.AddUIComponent<UILabel>();
            separator.relativePosition = new Vector3(22f, 2f);
            separator.size = new Vector2(90f, height);
            separator.height = height;
            separator.padding = new RectOffset(0, 0, 0, 0);
            separator.color = new Color32(255, 255, 255, 200);
            separator.autoHeight = false;
            separator.autoSize = false;
            return separator;
        }


        public static void ToggleNetworks()
        {
            MITE.filterNetworks = !MITE.filterNetworks;
            int filterRows = Filters.NetworkFilters.Count;

            if (MITE.filterNetworks)
            {
                foreach (UICheckBox cb in NetworkCheckboxes)
                {
                    if (cb != null)
                    {
                        cb.isVisible = true;
                    }
                    else
                    {
                        Debug.Log($"On - CB is null");
                    }
                }

                FilterPanel.height += MITE.UI_FILTER_CB_HEIGHT * filterRows;
                FilterPanel.absolutePosition += new Vector3(0f, 0 - (MITE.UI_FILTER_CB_HEIGHT * filterRows));
                _updateToggleNF();
            }
            else
            {
                foreach (UICheckBox cb in NetworkCheckboxes)
                {
                    if (cb != null)
                    {
                        cb.isVisible = false;
                    }
                    else
                    {
                        Debug.Log($"Off - CB is null");
                    }
                }

                FilterPanel.height -= MITE.UI_FILTER_CB_HEIGHT * filterRows;
                FilterPanel.absolutePosition -= new Vector3(0f, 0 - (MITE.UI_FILTER_CB_HEIGHT * filterRows));
                _updateToggleNF();
            }

        }
    }
}

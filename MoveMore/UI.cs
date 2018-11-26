using ColossalFramework.UI;
using Harmony;
using MoveIt;
using System.Collections.Generic;
using UnityEngine;

namespace MoveMore
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
                    //MoveMore.AlignMode = MoveMore.AlignModes.Off;

                    if (AlignToolsPanel.isVisible)
                    {
                        AlignToolsPanel.isVisible = false;
                    }
                    else
                    {
                        AlignToolsPanel.isVisible = true;
                    }
                    break;

                case "AlignHeight":
                    if (MoveMore.AlignMode == MoveMore.AlignModes.Height)
                    {
                        Debug.Log("MM.mode turning off");
                        MoveMore.AlignMode = MoveMore.AlignModes.Off;
                        MoveItTool.instance.toolState = MoveItTool.ToolState.Default;
                        break;
                    }
                    
                    if (MoveItTool.instance != null)
                    {
                        if (MoveItTool.instance.toolState == MoveItTool.ToolState.AligningHeights)
                        {
                            Debug.Log("MIT.tool turning off");
                            MoveMore.AlignMode = MoveMore.AlignModes.Off;
                            MoveItTool.instance.toolState = MoveItTool.ToolState.Default;
                        }
                        else
                        {
                            Debug.Log("MIT.tool turning on");
                            MoveItTool.instance.StartAligningHeights();
                            if (MoveItTool.instance.toolState == MoveItTool.ToolState.AligningHeights)
                            {
                                MoveMore.AlignMode = MoveMore.AlignModes.Height;
                            }
                        }
                    }

                    AlignToolsPanel.isVisible = false;
                    UpdateAlignToolsBtn();
                    break;

                case "AlignIndividual":
                    MoveMore.AlignMode = MoveMore.AlignModes.Individual;

                    AlignToolsPanel.isVisible = false;
                    UpdateAlignToolsBtn();
                    break;

                case "AlignGroup":
                    if (MoveMore.AlignMode == MoveMore.AlignModes.Group)
                    {
                        MoveMore.AlignMode = MoveMore.AlignModes.Off;
                    }
                    else
                    {
                        MoveMore.AlignMode = MoveMore.AlignModes.Group;
                    }

                    AlignToolsPanel.isVisible = false;
                    UpdateAlignToolsBtn();
                    break;

                case "AlignRandom":
                    MoveMore.AlignMode = MoveMore.AlignModes.Random;

                    AlignRotationAction action = new AlignRandomAction();
                    action.followTerrain = MoveItTool.followTerrain;
                    ActionQueue.instance.Push(action);
                    ActionQueue.instance.Do();
                    MoveMore.DeactivateAlignTool();

                    AlignToolsPanel.isVisible = false;
                    UpdateAlignToolsBtn();
                    break;
            }
            Debug.Log($"{c.name} clicked, mode is {MoveMore.AlignMode}");
        }


        public static void UpdateAlignToolsBtn()
        {
            switch (MoveMore.AlignMode)
            {
                case MoveMore.AlignModes.Height:
                    AlignToolsBtn.atlas = Traverse.Create(UIToolOptionPanel.instance).Method("GetIconsAtlas").GetValue<UITextureAtlas>();
                    AlignToolsBtn.normalFgSprite = "AlignHeight";
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    break;

                case MoveMore.AlignModes.Individual:
                    AlignToolsBtn.atlas = GetIconsAtlas();
                    AlignToolsBtn.normalFgSprite = "AlignIndividual";
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    break;

                case MoveMore.AlignModes.Group:
                    AlignToolsBtn.atlas = GetIconsAtlas();
                    AlignToolsBtn.normalFgSprite = "AlignGroup";
                    AlignToolsBtn.normalBgSprite = "OptionBaseFocused";
                    break;
                
                // Random mode is instant, button isn't relevant
                default:
                    AlignToolsBtn.atlas = GetIconsAtlas();
                    AlignToolsBtn.normalFgSprite = "AlignTools";
                    AlignToolsBtn.normalBgSprite = "OptionBase";
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
                "ColumnBG"
            };

            UITextureAtlas loadedAtlas = ResourceLoader.CreateTextureAtlas("MoveMore", spriteNames, "MoveMore.Icons.");
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
            ToggleNF.width = FilterPanel.width - 20f;
            ToggleNF.height = 16f;
            ToggleNF.relativePosition = new Vector2(10f, 0f);
            ToggleNF.normalBgSprite = null;
            ToggleNF.hoveredBgSprite = null;
            ToggleNF.pressedBgSprite = null;
            ToggleNF.disabledBgSprite = null;
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
            if (MoveMore.filterNetworks)
            { // Network Filters visible
                ToggleNF.text = "^";
                ToggleNF.textPadding = new RectOffset(0, 0, 6, 0);
                ToggleNF.textScale = 0.65f;
            }
            else
            { // Network Filters hidden
                ToggleNF.textPadding = new RectOffset(0, 0, 2, 0);
                ToggleNF.text = "+";
                ToggleNF.textScale = 0.8f;
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
            MoveMore.filterNetworks = !MoveMore.filterNetworks;
            int filterRows = MoveMore.NetworkFilters.Count;

            if (MoveMore.filterNetworks)
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

                FilterPanel.height += MoveMore.UI_FILTER_CB_HEIGHT * filterRows;
                FilterPanel.absolutePosition += new Vector3(0f, 0 - (MoveMore.UI_FILTER_CB_HEIGHT * filterRows), 0);
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

                FilterPanel.height -= MoveMore.UI_FILTER_CB_HEIGHT * filterRows;
                FilterPanel.absolutePosition -= new Vector3(0f, 0 - (MoveMore.UI_FILTER_CB_HEIGHT * filterRows), 0);
                _updateToggleNF();
            }

        }
    }
}

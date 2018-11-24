using ColossalFramework.UI;
using MoveIt;
using System.Collections.Generic;
using UnityEngine;

namespace MoveMore
{
    class UI
    {
        public static List<UICheckBox> NetworkCheckboxes = new List<UICheckBox>();
        public static UIButton ToggleNF;
        public static UIPanel FilterPanel;
        public static Color32 TextColor = new Color32(175, 216, 235, 255);


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

                FilterPanel.height += MoveMore.UI_FILTER_CB_HEIGHT * 5;
                FilterPanel.absolutePosition += new Vector3(0f, 0 - (MoveMore.UI_FILTER_CB_HEIGHT * 5), 0);
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

                FilterPanel.height -= MoveMore.UI_FILTER_CB_HEIGHT * 5;
                FilterPanel.absolutePosition -= new Vector3(0f, 0 - (MoveMore.UI_FILTER_CB_HEIGHT * 5), 0);
                _updateToggleNF();
            }

        }
    }
}

﻿using ColossalFramework.UI;
using Harmony;
using MoveIt;
using UnityEngine;

namespace MoveMore
{
    [HarmonyPatch(typeof(UIToolOptionPanel))]
    [HarmonyPatch("Start")]
    class UITOP_Start
    {
        private static UIButton m_marquee;
        private static UIButton m_alignHeight;
        private static Traverse _panel = null;


        private static Traverse _getPanel()
        {
            if (_panel == null)
            {
                _panel = Traverse.Create(m_marquee);
            }
            return _panel;
        }


        public static void Postfix(UIToolOptionPanel __instance, ref UIButton ___m_marquee, ref UIButton ___m_alignHeight)
        {
            UIPanel filtersPanel;
            m_marquee = ___m_marquee;
            m_alignHeight = ___m_alignHeight;
            __instance.RemoveUIComponent(__instance.filtersPanel);

            Debug.Log($"");

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

            Component[] component = filtersPanel.GetComponentsInChildren(typeof(UICheckBox));
            Component[] label = filtersPanel.GetComponentsInChildren(typeof(UILabel));
            //MoveMore.DebugLine($"{filtersPanel.childCount},{filtersPanel.height} component-length:{component.Length}, label-length:{label.Length}");
        }
    }
}

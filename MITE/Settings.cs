using ColossalFramework.UI;
using ICities;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MITE
{
    public class Settings
    {
        public bool DecalsAsSurfaces = true;
        public bool ExtraAsSurfaces = true;
        public bool DocksAsSurfaces = true;
        public bool BrushesAsSurfaces = true;
        public bool PillarsAsNotBuildings = true;
        public bool PylonsAsNotBuildings = true;
        public bool ShowDebugPanel = false;

        public Settings() { }
        public void OnPreSerialize() { }
        public void OnPostDeserialize() { }

        public void SaveConfiguration()
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(MITE.settingsFilePath))
            {
                MITE.Settings.OnPreSerialize();
                serializer.Serialize(writer, MITE.Settings);
            }
        }


        public static Settings LoadConfiguration()
        {
            var fileName = MITE.settingsFilePath;
            if (!File.Exists(fileName)) return null;
            var serializer = new XmlSerializer(typeof(Settings));
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var config = serializer.Deserialize(reader) as Settings;
                    return config;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[MITE]: Error Parsing {fileName}: {ex}");
                return null;
            }
        }


        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddSpace(20);
            UICheckBox cb;
            UIHelper group = (UIHelper)helper.AddGroup("Recognise as surfaces:");
            cb = (UICheckBox)group.AddCheckbox("Ploppable asphalt decals", MITE.Settings.DecalsAsSurfaces, (i) =>
            {
                MITE.Settings.DecalsAsSurfaces = i;
                SaveConfiguration();
            });
            cb = (UICheckBox)group.AddCheckbox("[RWB] FxUK's Brushes", MITE.Settings.BrushesAsSurfaces, (i) =>
            {
                MITE.Settings.BrushesAsSurfaces = i;
                SaveConfiguration();
            });
            cb = (UICheckBox)group.AddCheckbox("Extras: Ronyx69's Docks, Deczaah's Surfaces", MITE.Settings.ExtraAsSurfaces, (i) =>
            {
                MITE.Settings.ExtraAsSurfaces = i;
                SaveConfiguration();
            });
            helper.AddSpace(20);
            group = (UIHelper)helper.AddGroup("Only select nodes for these items:");
            cb = (UICheckBox)group.AddCheckbox("Pillars\n", MITE.Settings.PillarsAsNotBuildings, (i) =>
            {
                MITE.Settings.PillarsAsNotBuildings = i;
                SaveConfiguration();
            });
            cb.name = "MITE_PillarsAsNotBuildings";
            cb = (UICheckBox)group.AddCheckbox("Powerline pylons/posts", MITE.Settings.PylonsAsNotBuildings, (i) =>
            {
                MITE.Settings.PylonsAsNotBuildings = i;
                SaveConfiguration();
            });
            cb.name = "MITE_PylonsAsNotBuildings";
            UIPanel groupPanel = (UIPanel)group.self;
            UILabel note = groupPanel.AddUIComponent<UILabel>();
            note.text = "\nNote: Items can be separately selected by holding Alt. Disable to marquee select these buildings.";
            helper.AddSpace(20);
            group = (UIHelper)helper.AddGroup("Advanced Options:");
            cb = (UICheckBox)group.AddCheckbox("Show MITE debug panel\n(Affects performance, do not enable unless you have a specific reason)", MITE.Settings.ShowDebugPanel, (i) =>
            {
                MITE.Settings.ShowDebugPanel = i;
                UI.DbgPanel.Visible(i);
                SaveConfiguration();
            });
            cb.name = "MITE_DebugPanel_Toggle";
            helper.AddSpace(20);
        }
    }
}

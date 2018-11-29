﻿using ColossalFramework.UI;
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
            UIHelperBase group = helper.AddGroup("Recognise as surfaces:");
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
            group = helper.AddGroup("Hide from selection:");
            cb = (UICheckBox)group.AddCheckbox("Pylons and network pillars\n(Will still be moved/deleted with attached node)", MITE.Settings.PillarsAsNotBuildings, (i) =>
            {
                MITE.Settings.PillarsAsNotBuildings = i;
                SaveConfiguration();
            });
            helper.AddSpace(20);
        }
    }
}

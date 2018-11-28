
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
        public bool DecalsAsSurfaces;
        public bool TinySegments;

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
            UIHelperBase group = helper.AddGroup("More It Tool Extensions");
            UICheckBox CBdecal = (UICheckBox)group.AddCheckbox("Filter ploppable asphalt decals as surfaces", MITE.Settings.DecalsAsSurfaces, (i) =>
            {
                MITE.Settings.DecalsAsSurfaces = i;
                SaveConfiguration();
            });
            helper.AddSpace(20);
        }
    }
}

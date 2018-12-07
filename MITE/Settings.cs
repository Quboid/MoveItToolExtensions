using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using ICities;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace MITE
{
    [Serializable()]
    public class SerializableShortcut
    {
        public KeyCode Key;
        public bool Control;
        public bool Alt;
        public bool Shift;


        public SerializableShortcut() { }

        public SerializableShortcut(KeyCode k, bool c, bool s, bool a)
        {
            Key = k;
            Control = c;
            Shift = s;
            Alt = a;
        }

        public override string ToString()
        {
            return $"Key:{Key}, C:{Control} S:{Shift} A:{Alt}";
        }

        [XmlIgnore]
        public SavedInputKey SIK {
            get => new SavedInputKey("SIK", Settings.SettingsFileLocation, SavedInputKey.Encode(this.Key, this.Control, this.Shift, this.Alt), true);
            set {
                this.Key = value.Key;
                this.Control = value.Control;
                this.Shift = value.Shift;
                this.Alt = value.Alt;
                _keyPress = null;
            }
        }

        [XmlIgnore]
        private Event _keyPress;
        public Event KeyPress {
            get {
                if (_keyPress == null)
                {
                    string str = "";
                    if (Control) str += "^";
                    if (Shift) str += "#";
                    if (Alt) str += "&";
                    _keyPress = Event.KeyboardEvent(str + Key);
                }
                return _keyPress;
            }
        }

        public bool isPressed()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (!Control) return false;
            }
            else
            {
                if (Control) return false;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (!Shift) return false;
            }
            else
            {
                if (Shift) return false;
            }

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (!Alt) return false;
            }
            else
            {
                if (Alt) return false;
            }

            return Input.GetKey(Key);
        }

        //public bool CheckEvent(Event current)
        //{
        //    if (current.control != Control) return false;
        //    if (current.shift != Shift) return false;
        //    if (current.alt != Alt) return false;

        //    char currentChar = current.character;
        //    if (currentChar < 96) currentChar += (char)32;
        //    Debug.Log($"ascii:{currentChar},{(char)Key}  -  Unity:{current.keyCode},{Key}");
        //    if (currentChar == (char)Key) return true;
        //    if (current.keyCode == Key) return true;
        //    return false;
        //}

        //public string getEventKey()
        //{
        //    string msg = "";
        //    if (Control) msg += "^";
        //    if (Shift) msg += "#";
        //    if (Alt) msg += "%";

        //    return msg + Key;
        //}
    }

    public class Settings
    {
        [XmlIgnore]
        public static readonly string SettingsFileLocation = Path.Combine(DataLocation.localApplicationData, "MITE.xml");

        public bool DecalsAsSurfaces = true;
        public bool ExtraAsSurfaces = true;
        public bool DocksAsSurfaces = true;
        public bool BrushesAsSurfaces = true;
        public bool PillarsAsNotBuildings = false;
        public bool PylonsAsNotBuildings = false;
        public bool AutoCollapseAlignTools = false;
        public SerializableShortcut keyStepOver = new SerializableShortcut(KeyCode.Tab, true, false, false);
        //public SavedInputKey keyStepOver = new SavedInputKey("stepOver", SettingsClass.SettingsFileLocation, SavedInputKey.Encode(KeyCode.Tab, true, false, false), true);
        public bool ShowDebugPanel = false;

        public Settings() { }
        public void OnPreSerialize() { }
        public void OnPostDeserialize() { }

        public void SaveConfiguration()
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(SettingsFileLocation))
            {
                MITE.Settings.OnPreSerialize();
                serializer.Serialize(writer, MITE.Settings);
            }
        }


        public static Settings LoadConfiguration()
        {
            if (!File.Exists(SettingsFileLocation)) return null;
            var serializer = new XmlSerializer(typeof(Settings));
            try
            {
                using (var reader = new StreamReader(SettingsFileLocation))
                {
                    var config = serializer.Deserialize(reader) as Settings;
                    return config;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[MITE]: Error Parsing {SettingsFileLocation}: {ex}");
                return null;
            }
        }


        public void OnSettingsUI(UIHelperBase helper)
        {
            UICheckBox cb;

            helper.AddSpace(20);
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
            note.text = "\nNote: Items can be separately selected by holding Alt. Disable\nto marquee select these buildings.";
            helper.AddSpace(20);
            group = (UIHelper)helper.AddGroup("Additional Options:");
            UIPanel panel = group.self as UIPanel;

            panel.gameObject.AddComponent<OptionsKeymapping>();
            group.AddSpace(3);

            cb = (UICheckBox)group.AddCheckbox("Automatically close the Align Tools menu after choosing a tool", MITE.Settings.AutoCollapseAlignTools, (i) =>
            {
                MITE.Settings.AutoCollapseAlignTools = i;
                SaveConfiguration();
                if (UI.AlignToolsPanel != null)
                {
                    UI.AlignToolsPanel.isVisible = false;
                    UI.UpdateAlignTools();
                }
            });
            cb.name = "MITE_AutoCollapse_AlignTools";
            group.AddSpace(3);
            cb = (UICheckBox)group.AddCheckbox("Show MITE debug panel\n(Affects performance, do not enable unless you have a specific reason)", MITE.Settings.ShowDebugPanel, (i) =>
            {
                MITE.Settings.ShowDebugPanel = i;
                SaveConfiguration();
                if (UI.DbgPanel != null)
                {
                    UI.DbgPanel.Visible(i);
                }
            });
            cb.name = "MITE_DebugPanel_Toggle";
            helper.AddSpace(10);
        }
    }
}

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using PropertiesPanel.Localization;

namespace PropertiesPanel
{
    public delegate void SettingsChangesEvent();

    [Serializable]
    public class Settings
    {
        [field: NonSerialized]
        public event SettingsChangesEvent OnSettingsChanged;

        private const bool DEFAULT_TRACK_OPEN_FILE = false;

        private bool _trackOpenFile = DEFAULT_TRACK_OPEN_FILE;

        [LocalizedCategory("TrackOpenFile.Category.General")]
        [LocalizedDisplayName("TrackOpenFile.Label.TrackOpenFile")]
        [LocalizedDescription("TrackOpenFile.Description.TrackOpenFile")]
        [DefaultValue(DEFAULT_TRACK_OPEN_FILE)]
        public bool TrackOpenFile
        {
            get { return _trackOpenFile; }
            set
            {
                if (_trackOpenFile != value)
                {
                    _trackOpenFile = value;
                    FireChanged();
                }
            }
        }

        private void FireChanged()
        {
            if (OnSettingsChanged != null) OnSettingsChanged();
        }
    }
}

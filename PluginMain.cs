using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using PropertiesPanel.Helpers;
using WeifenLuo.WinFormsUI.Docking;

namespace PropertiesPanel
{
    public class PluginMain : IPlugin
    {
        private const int API = 1;
        private const string NAME = "TrackOpenFile";
        private const string GUID = "F8B62816-8086-4211-8085-2A68141D6F42";
        private const string HELP = "www.flashdevelop.org/community/";
        private const string DESCRIPTION = "Adds a setting that will have the Project Panel track the open file.";
        private const string AUTHOR = "Joey Robichaud";

        private string _settingFilename = "";
        private Settings _settings;
        private Timer _timer;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return NAME; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return GUID; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return AUTHOR; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return DESCRIPTION; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return HELP; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this._settings; }
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            if (e.Type == EventType.FileSwitch)
            {
                if (_settings.TrackOpenFile)
                    SyncOpenFile();
            }
        }

        #endregion

        #region Custom Methods

        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            SyncOpenFile();
        }

        public void SyncOpenFile()
        {
            foreach (var pane in PluginBase.MainForm.DockPanel.Panes)
            {
                var panel = pane.ActiveContent as Form;
                if (panel == null)
                    continue;

                if (panel.Controls.Count == 0)
                    continue;

                var projectUI = panel.Controls[0] as ProjectManager.PluginUI;
                if (projectUI == null)
                    continue;

                ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                if (doc != null && doc.IsEditable && !doc.IsUntitled)
                {
                    if (projectUI.Tree.Nodes.Count > 0)
                    {
                        projectUI.Tree.Select(doc.FileName);

                        if (projectUI.Tree.SelectedNode != null)
                            projectUI.Tree.SelectedNode.EnsureVisible();
                    }
                    else
                    {
                        _timer.Start();
                    }
                }
                break;
            }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            String dataPath = Path.Combine(PathHelper.DataDir, NAME);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this._settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            // Set events you want to listen (combine as flags)
            EventManager.AddEventHandler(this, EventType.FileSwitch);
            _settings.OnSettingsChanged += _settings_OnSettingsChanged;
        }

        void _settings_OnSettingsChanged()
        {
            if (_settings.TrackOpenFile)
                SyncOpenFile();
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this._settings = new Settings();
            if (!File.Exists(this._settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this._settingFilename, this._settings);
                this._settings = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this._settingFilename, this._settings);
        }

        #endregion
    }
}
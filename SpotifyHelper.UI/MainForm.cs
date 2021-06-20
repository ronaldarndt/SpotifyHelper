using SpotifyAPI.Web;
using SpotifyHelper.Core;
using SpotifyHelper.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyHelper.UI
{
    public partial class MainForm : Form
    {
        private Services m_services;
        private ProgramStartup m_startup;
        private Services.PlaylistDTO[] m_playlists;

        private int m_hotkeyId = -1;

        public MainForm()
        {
            InitializeComponent();
        }

        private async Task Initialize(IAuthenticator authenticator)
        {
            if (authenticator is null)
            {
                return;
            }

            AuthButton.Visible = false;
            ConfigButton.Visible = true;

            HotKeyManager.HotKeyPressed += HandleHotkey;
            ConfigProvider<HotKeyManager.HotkeyConfig>.ConfigChanged += ConfigureHotkey;

            await ConfigProvider<HotKeyManager.HotkeyConfig>
                .InitializeAsync(new HotKeyManager.HotkeyConfig()
                {
                    Keys = Keys.F1,
                    Modifiers = HotKeyManager.KeyModifiers.Control
                });

            var spotifyConfig = SpotifyClientConfig
                .CreateDefault()
                .WithHTTPLogger(null)
                .WithAuthenticator(authenticator);

            var client = new SpotifyClient(spotifyConfig);
            m_services = new Services(client);

            m_playlists = (await m_services.GetPlaylists()).ToArray();

            PlaylistsList.Items.AddRange(m_playlists.Select(x => x.Name).ToArray());
        }

        private void ConfigureHotkey(HotKeyManager.HotkeyConfig config)
        {
            if (m_hotkeyId > -1)
            {
                HotKeyManager.UnregisterHotKey(m_hotkeyId);
            }

            m_hotkeyId = HotKeyManager.RegisterHotKey(config.Keys, config.Modifiers);
        }

        private IEnumerable<string> GetCheckedItems()
        {
            foreach (var index in PlaylistsList.CheckedIndices)
            {
                yield return m_playlists[(int)index].Id;
            }
        }

        private async void HandleHotkey(object sender, HotKeyManager.HotKeyEventArgs e)
        {
            foreach (var selectedKey in this.Invoke(GetCheckedItems))
            {
                await m_services.AddCurrentlyPlayingToPlaylist(selectedKey);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var authenticator = await Auth.GetAuthenticatorAsync();

            await Initialize(authenticator);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            ConfigButton.Visible = false;

            var executable = Path.ChangeExtension(Application.ExecutablePath, "exe");

            m_startup = new ProgramStartup(Application.ProductName, executable);

            StartupCheckbox.Checked = m_startup.GetStatus();

            var authenticator = await Auth.GetAuthenticatorFromFileAsync();

            await Initialize(authenticator);
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            using var configForm = new ConfigForm();

            configForm.ShowDialog();
        }

        private void StartupCheckbox_Click(object sender, EventArgs e)
        {
            m_startup.Toggle();
        }
    }
}

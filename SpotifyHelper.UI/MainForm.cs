using SpotifyAPI.Web;
using SpotifyHelper.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyHelper.UI
{
    public partial class MainForm : Form
    {
        private delegate object GetCheckedItemDelegate();

        private Services m_services;
        private int m_hotkeyId = -1;

        public MainForm()
        {
            InitializeComponent();

            ConfigButton.Visible = false;
        }

        private async Task Initialize(IAuthenticator authenticator)
        {
            if (authenticator is null)
            {
                return;
            }

            AuthButton.Visible = false;
            ConfigButton.Visible = true;

            ConfigProvider<HotkeyConfig>.ConfigChanged += ConfigureHotkey;

            await ConfigProvider<HotkeyConfig>.InitializeAsync(new HotkeyConfig()
            {
                Keys = Keys.F1,
                Modifiers = KeyModifiers.Control
            });

            var spotifyConfig = SpotifyClientConfig
                .CreateDefault()
                .WithHTTPLogger(null)
                .WithAuthenticator(authenticator);

            var client = new SpotifyClient(spotifyConfig);
            m_services = new Services(client);

            var playlists = await m_services.GetPlaylists();

            PlaylistsList.Items.AddRange(playlists.Select(x => x.Name + " - " + x.Id).ToArray());
        }

        private void ConfigureHotkey(HotkeyConfig config)
        {
            if (m_hotkeyId == -1)
            {
                HotKeyManager.HotKeyPressed += HandleHotkey;
            }
            else
            {
                HotKeyManager.UnregisterHotKey(m_hotkeyId);
            }

            m_hotkeyId = HotKeyManager.RegisterHotKey(config.Keys, config.Modifiers);
        }

        private object GetCheckedItems()
        {
            var checkedItems = new List<string>();

            foreach (var item in PlaylistsList.CheckedItems)
            {
                checkedItems.Add(item.ToString());
            }

            return checkedItems;
        }

        private async void HandleHotkey(object sender, HotKeyEventArgs e)
        {
            var selectedKeys = (List<string>)Invoke(new GetCheckedItemDelegate(GetCheckedItems));

            foreach (var selectedKey in selectedKeys.Select(x => x.Split("- ")[1]))
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
            var authenticator = await Auth.GetAuthenticatorFromFileAsync();

            await Initialize(authenticator);
        }

        private void ConfigButton_Click(object sender, EventArgs e)
        {
            using var configForm = new ConfigForm();

            configForm.ShowDialog();
        }
    }
}

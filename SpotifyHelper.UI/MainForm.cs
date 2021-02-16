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
        private delegate object GetSelectedItemDelegate();

        private SpotifyClient _client;
        private Dictionary<string, string> _playlists;

        public MainForm()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var authenticator = await Auth.GetAuthenticator();

            await Initialize(authenticator);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var authenticator = await Auth.GetAuthenticatorFromFileAsync();

            await Initialize(authenticator);
        }

        private async Task Initialize(IAuthenticator authenticator)
        {
            if (authenticator is null)
            {
                return;
            }

            button1.Visible = false;

            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(authenticator);

            _client = new SpotifyClient(config);

            var playlists = await _client.PaginateAll(await _client.Playlists.CurrentUsers());

            _playlists = playlists.ToDictionary(x => $"{x.Name} - {x.Id}", x => x.Id);

            checkedListBox1.Items.AddRange(_playlists.Select(x => x.Key).ToArray());

            playlists.Clear();

            var hotkey = HotKeyManager.RegisterHotKey(Keys.E, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += async (sender, e) =>
            {
                var selectedKey = Invoke(new GetSelectedItemDelegate(() => checkedListBox1.SelectedItem)).ToString();

                if (string.IsNullOrEmpty(selectedKey) || !_playlists.ContainsKey(selectedKey))
                {
                    return;
                }

                var playback = await _client.Player.GetCurrentPlayback();

                if (playback != null && playback.IsPlaying && playback.Item is FullTrack track)
                {
                    var selected = _playlists[selectedKey];
                    var playlist = await _client.Playlists.Get(selected);

                    if (playlist.Tracks.Items.Any(x => x.Track is FullTrack playlistTrack && playlistTrack.Id == track.Id))
                    {
                        return;
                    }

                    var currentlyPlaying = new List<string>() { track.Uri };

                    await _client.Playlists.AddItems(selected, new PlaylistAddItemsRequest(currentlyPlaying));
                }
            };

            await Task.Delay(0);
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyHelper.Core;
using SpotifyHelper.Core.Config;
using SpotifyHelper.Core.Storage;
using SpotifyHelper.Core.Token;
using SpotifyHelper.Core.Utils;
using static SpotifyHelper.UI.HotKeyManager;

namespace SpotifyHelper.UI;

public partial class MainForm : Form
{
    private static readonly FileStorage s_storage = new();
    public static ConfigProvider<ConfigModel> ConfigProvider { get; } = new(s_storage);

    private readonly ProgramStartup m_startup;
    private readonly Auth m_auth;
    private Services? m_services;
    private int m_hotkeyId = -1;

    public MainForm()
    {
        InitializeComponent();

        ConfigureHotkey(ConfigProvider.Config);

        HotKeyPressed += HandleHotkey;
        ConfigProvider.ConfigChanged += ConfigureHotkey;

        var executable = Path.ChangeExtension(Application.ExecutablePath, "exe");
        m_startup = new ProgramStartup(Application.ProductName, executable);

        m_auth = new Auth(s_storage, new TokenProvider(ConfigProvider.Config.ClientId));

        StartupCheckbox.Checked = m_startup.GetStatus();
    }

    private async Task Initialize(IAuthenticator? authenticator)
    {
        if (authenticator is null)
        {
            return;
        }

        AuthButton.Visible = false;
        ConfigButton.Visible = true;

        var spotifyConfig = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(authenticator);

        var client = new SpotifyClient(spotifyConfig);
        m_services = new Services(client);

        var playlists = await m_services.GetPlaylists();

        PlaylistsList.Items.AddRange(playlists.Select(x => x.Name + " - " + x.Id).ToArray());
    }

    private void ConfigureHotkey(ConfigModel config)
    {
        if (m_hotkeyId > -1)
        {
            UnregisterHotKey(m_hotkeyId);
        }

        m_hotkeyId = RegisterHotKey(config.Key, config.KeyModifiers);
    }

    private object GetCheckedItems()
    {
        var checkedItems = new List<string>();

        foreach (var item in PlaylistsList.CheckedItems)
        {
            checkedItems.Add(item?.ToString() ?? "");
        }

        return checkedItems;
    }

    private async void HandleHotkey(object? sender, HotKeyEventArgs e)
    {
        if (m_services is null)
        {
            return;
        }

        var selectedKeys = (List<string>)Invoke(GetCheckedItems);

        foreach (var selectedKey in selectedKeys.Select(x => x.Split("- ")[1]))
        {
            await m_services.AddCurrentlyPlayingToPlaylist(selectedKey);
        }
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        var authenticator = await m_auth.GetAuthenticatorAsync();

        await Initialize(authenticator);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        ConfigButton.Visible = false;

        var authenticator = await m_auth.GetAuthenticatorFromFileAsync();

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

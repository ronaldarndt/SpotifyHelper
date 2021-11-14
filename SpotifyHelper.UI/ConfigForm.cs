using SpotifyHelper.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using static SpotifyHelper.UI.HotKeyManager;
using static SpotifyHelper.Core.Extensions.EnumExtensions;

namespace SpotifyHelper.UI;

public partial class ConfigForm : Form
{
    private Keys m_key;
    private bool m_recordingKey;
    private readonly ConfigProvider<HotkeyConfig> m_configProvider;

    public ConfigForm()
    {
        InitializeComponent();

        m_configProvider = MainForm.ConfigProvider;

        SetSelectedKey(m_configProvider.Config.Keys);
        SetSelectedModifiers(m_configProvider.Config.Modifiers);
    }

    private void SetSelectedModifiers(KeyModifiers modifiers)
    {
        CtrlModifierCheckbox.Checked = modifiers.HasFlag(KeyModifiers.Control);
        AltModifierCheckbox.Checked = modifiers.HasFlag(KeyModifiers.Alt);
        ShiftModifierCheckbox.Checked = modifiers.HasFlag(KeyModifiers.Shift);
    }

    private void SetSelectedKey(Keys key)
    {
        m_key = key;

        SelectedLabel.Text = key.ToString();
        SelectedLabel.ForeColor = Color.Black;
        SelectedLabel.BorderStyle = BorderStyle.FixedSingle;
        SelectedLabel.Font = new Font(SelectedLabel.Font, FontStyle.Bold);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        m_recordingKey = true;

        SelectedLabel.ForeColor = Color.DarkOrange;
        SelectedLabel.Text = "Press any key";
        SelectedLabel.BorderStyle = BorderStyle.None;
        SelectedLabel.Font = new Font(SelectedLabel.Font, FontStyle.Regular);
    }

    private void ConfigForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (!m_recordingKey)
        {
            return;
        }

        SetSelectedKey(e.KeyCode);

        m_recordingKey = false;
        e.Handled = true;
    }

    private async void SaveButton_Click(object sender, EventArgs e)
    {
        var mods = KeyModifiers.None
            .SetFlagIf(KeyModifiers.Control, CtrlModifierCheckbox.Checked)
            .SetFlagIf(KeyModifiers.Alt, AltModifierCheckbox.Checked)
            .SetFlagIf(KeyModifiers.Shift, ShiftModifierCheckbox.Checked);

        await m_configProvider.UpdateAsync(new HotkeyConfig(m_key, mods));

        DialogResult = DialogResult.OK;
    }
}

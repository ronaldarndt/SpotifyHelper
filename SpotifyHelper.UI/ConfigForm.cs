using SpotifyHelper.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpotifyHelper.UI
{
    public partial class ConfigForm : Form
    {
        private Keys m_key;
        private KeyModifiers m_modifiers;

        private bool m_recordingKey;

        public ConfigForm()
        {
            InitializeComponent();

            var config = ConfigProvider<HotkeyConfig>.Config;

            SetSelectedKey(config.Keys);
            SetSelectedModifiers(config.Modifiers);
        }

        private void SetSelectedModifiers(KeyModifiers modifiers)
        {
            m_modifiers = modifiers;

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
            var modifiers = KeyModifiers.None;

            if (CtrlModifierCheckbox.Checked)
            {
                modifiers |= KeyModifiers.Control;
            }

            if (AltModifierCheckbox.Checked)
            {
                modifiers |= KeyModifiers.Alt;
            }

            if (ShiftModifierCheckbox.Checked)
            {
                modifiers |= KeyModifiers.Shift;
            }

            await ConfigProvider<HotkeyConfig>.UpdateAsync(new HotkeyConfig()
            {
                Keys = m_key,
                Modifiers = modifiers
            });

            DialogResult = DialogResult.OK;
        }
    }
}

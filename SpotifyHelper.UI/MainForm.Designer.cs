
namespace SpotifyHelper.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AuthButton = new System.Windows.Forms.Button();
            this.PlaylistsList = new System.Windows.Forms.CheckedListBox();
            this.currentPlayback = new System.Windows.Forms.Label();
            this.ConfigButton = new System.Windows.Forms.Button();
            this.StartupCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // AuthButton
            // 
            this.AuthButton.Location = new System.Drawing.Point(12, 12);
            this.AuthButton.Name = "AuthButton";
            this.AuthButton.Size = new System.Drawing.Size(75, 23);
            this.AuthButton.TabIndex = 0;
            this.AuthButton.Text = "Auth";
            this.AuthButton.UseVisualStyleBackColor = true;
            this.AuthButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // PlaylistsList
            // 
            this.PlaylistsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlaylistsList.CheckOnClick = true;
            this.PlaylistsList.FormattingEnabled = true;
            this.PlaylistsList.Location = new System.Drawing.Point(12, 41);
            this.PlaylistsList.Name = "PlaylistsList";
            this.PlaylistsList.Size = new System.Drawing.Size(313, 328);
            this.PlaylistsList.TabIndex = 1;
            // 
            // currentPlayback
            // 
            this.currentPlayback.AutoSize = true;
            this.currentPlayback.Location = new System.Drawing.Point(93, 16);
            this.currentPlayback.Name = "currentPlayback";
            this.currentPlayback.Size = new System.Drawing.Size(0, 15);
            this.currentPlayback.TabIndex = 2;
            // 
            // ConfigButton
            // 
            this.ConfigButton.Location = new System.Drawing.Point(228, 12);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(97, 23);
            this.ConfigButton.TabIndex = 3;
            this.ConfigButton.Text = "Config Bind";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.ConfigButton_Click);
            // 
            // StartupCheckbox
            // 
            this.StartupCheckbox.AutoSize = true;
            this.StartupCheckbox.Location = new System.Drawing.Point(99, 15);
            this.StartupCheckbox.Name = "StartupCheckbox";
            this.StartupCheckbox.Size = new System.Drawing.Size(112, 19);
            this.StartupCheckbox.TabIndex = 4;
            this.StartupCheckbox.Text = "Open on startup";
            this.StartupCheckbox.UseVisualStyleBackColor = true;
            this.StartupCheckbox.Click += new System.EventHandler(this.StartupCheckbox_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 372);
            this.Controls.Add(this.StartupCheckbox);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.currentPlayback);
            this.Controls.Add(this.PlaylistsList);
            this.Controls.Add(this.AuthButton);
            this.MinimumSize = new System.Drawing.Size(347, 300);
            this.Name = "MainForm";
            this.Text = "Config";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AuthButton;
        private System.Windows.Forms.CheckedListBox PlaylistsList;
        private System.Windows.Forms.Label currentPlayback;
        private System.Windows.Forms.Button ConfigButton;
        private System.Windows.Forms.CheckBox StartupCheckbox;
    }
}



namespace SpotifyHelper.UI
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.CtrlModifierCheckbox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.AltModifierCheckbox = new System.Windows.Forms.CheckBox();
            this.ShiftModifierCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectedLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Modifiers";
            // 
            // CtrlModifierCheckbox
            // 
            this.CtrlModifierCheckbox.AutoSize = true;
            this.CtrlModifierCheckbox.Location = new System.Drawing.Point(12, 38);
            this.CtrlModifierCheckbox.Name = "CtrlModifierCheckbox";
            this.CtrlModifierCheckbox.Size = new System.Drawing.Size(45, 19);
            this.CtrlModifierCheckbox.TabIndex = 1;
            this.CtrlModifierCheckbox.Text = "Ctrl";
            this.CtrlModifierCheckbox.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(77, 167);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // AltModifierCheckbox
            // 
            this.AltModifierCheckbox.AutoSize = true;
            this.AltModifierCheckbox.Location = new System.Drawing.Point(12, 63);
            this.AltModifierCheckbox.Name = "AltModifierCheckbox";
            this.AltModifierCheckbox.Size = new System.Drawing.Size(41, 19);
            this.AltModifierCheckbox.TabIndex = 4;
            this.AltModifierCheckbox.Text = "Alt";
            this.AltModifierCheckbox.UseVisualStyleBackColor = true;
            // 
            // ShiftModifierCheckbox
            // 
            this.ShiftModifierCheckbox.AutoSize = true;
            this.ShiftModifierCheckbox.Location = new System.Drawing.Point(12, 88);
            this.ShiftModifierCheckbox.Name = "ShiftModifierCheckbox";
            this.ShiftModifierCheckbox.Size = new System.Drawing.Size(50, 19);
            this.ShiftModifierCheckbox.TabIndex = 5;
            this.ShiftModifierCheckbox.Text = "Shift";
            this.ShiftModifierCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Key";
            // 
            // SelectedLabel
            // 
            this.SelectedLabel.AutoSize = true;
            this.SelectedLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectedLabel.ForeColor = System.Drawing.Color.Red;
            this.SelectedLabel.Location = new System.Drawing.Point(135, 39);
            this.SelectedLabel.Name = "SelectedLabel";
            this.SelectedLabel.Padding = new System.Windows.Forms.Padding(5);
            this.SelectedLabel.Size = new System.Drawing.Size(52, 27);
            this.SelectedLabel.TabIndex = 8;
            this.SelectedLabel.Text = "NONE";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(135, 84);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Record";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 202);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SelectedLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ShiftModifierCheckbox);
            this.Controls.Add(this.AltModifierCheckbox);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.CtrlModifierCheckbox);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Name = "ConfigForm";
            this.Text = "ConfigForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ConfigForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CtrlModifierCheckbox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.CheckBox AltModifierCheckbox;
        private System.Windows.Forms.CheckBox ShiftModifierCheckbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label SelectedLabel;
        private System.Windows.Forms.Button button1;
    }
}
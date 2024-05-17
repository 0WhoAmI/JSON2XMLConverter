using System;
using System.Drawing;
using System.Windows.Forms;

namespace JsonXmlConverter.Program
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            buttonSelectFile = new Button();
            textBoxInput = new TextBox();
            textBoxOutput = new TextBox();
            buttonConvert = new Button();
            buttonSaveAs = new Button();
            SuspendLayout();
            // 
            // buttonSelectFile
            // 
            buttonSelectFile.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonSelectFile.Location = new Point(10, 10);
            buttonSelectFile.Name = "buttonSelectFile";
            buttonSelectFile.Size = new Size(120, 40);
            buttonSelectFile.TabIndex = 0;
            buttonSelectFile.Text = "Wybierz plik";
            buttonSelectFile.UseVisualStyleBackColor = true;
            buttonSelectFile.Click += buttonSelectFile_Click;
            // 
            // textBoxInput
            // 
            textBoxInput.AcceptsTab = true;
            textBoxInput.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxInput.Location = new Point(10, 60);
            textBoxInput.Multiline = true;
            textBoxInput.Name = "textBoxInput";
            textBoxInput.ScrollBars = ScrollBars.Vertical;
            textBoxInput.Size = new Size(500, 320);
            textBoxInput.TabIndex = 1;
            textBoxInput.KeyDown += TextBox_KeyDown;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxOutput.Location = new Point(530, 60);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = ScrollBars.Vertical;
            textBoxOutput.Size = new Size(500, 320);
            textBoxOutput.TabIndex = 2;
            // 
            // buttonConvert
            // 
            buttonConvert.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonConvert.Location = new Point(781, 410);
            buttonConvert.Name = "buttonConvert";
            buttonConvert.Size = new Size(120, 40);
            buttonConvert.TabIndex = 3;
            buttonConvert.Text = "Konwertuj";
            buttonConvert.UseVisualStyleBackColor = true;
            buttonConvert.Click += buttonConvert_Click;
            // 
            // buttonSaveAs
            // 
            buttonSaveAs.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonSaveAs.Location = new Point(911, 410);
            buttonSaveAs.Name = "buttonSaveAs";
            buttonSaveAs.Size = new Size(120, 40);
            buttonSaveAs.TabIndex = 4;
            buttonSaveAs.Text = "Zapisz jako";
            buttonSaveAs.UseVisualStyleBackColor = true;
            buttonSaveAs.Click += buttonSaveAs_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1040, 460);
            Controls.Add(buttonSaveAs);
            Controls.Add(buttonConvert);
            Controls.Add(textBoxOutput);
            Controls.Add(textBoxInput);
            Controls.Add(buttonSelectFile);
            Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinimumSize = new Size(800, 460);
            Name = "MainForm";
            Text = "Konwerter plików";
            Resize += MainForm_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonSelectFile;
        private TextBox textBoxInput;
        private TextBox textBoxOutput;
        private Button buttonConvert;
        private Button buttonSaveAs;

        private void MainForm_Resize(object sender, EventArgs e)
        {
            const int margin = 10;
            const int buttonWidth = 120;
            const int buttonHeight = 40;

            int halfWidth = ClientSize.Width / 2;
            int inputOutputWidth = halfWidth - 2 * margin;
            int inputOutputHeight = ClientSize.Height - 2 * margin - buttonSelectFile.Height - buttonHeight - 40;

            buttonSelectFile.Location = new Point(margin, margin);
            buttonSelectFile.Size = new Size(buttonWidth, buttonHeight);

            textBoxInput.Location = new Point(margin, buttonSelectFile.Bottom + margin);
            textBoxInput.Size = new Size(inputOutputWidth, inputOutputHeight);

            textBoxOutput.Location = new Point(halfWidth + margin, buttonSelectFile.Bottom + margin);
            textBoxOutput.Size = new Size(inputOutputWidth, inputOutputHeight);

            // Calculate new position for buttons
            int buttonX = ClientSize.Width - margin * 2 - buttonWidth * 2;
            int buttonY = ClientSize.Height - margin - buttonHeight;

            buttonConvert.Location = new Point(buttonX, buttonY);
            buttonSaveAs.Location = new Point(buttonX + buttonWidth + margin, buttonY);
        }
    }
}
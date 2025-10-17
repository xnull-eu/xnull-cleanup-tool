namespace XNullCleanup
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
            if (disposing)
            {
                // Dispose of the timer
                sizeRefreshTimer?.Stop();
                sizeRefreshTimer?.Dispose();
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
            this.titlePanel = new System.Windows.Forms.Panel();
            this.minimizeButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.riskWarningLabel = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.RichTextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.cleanButton = new System.Windows.Forms.Button();
            this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
            this.cleanupOptionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.optionsPanel = new System.Windows.Forms.Panel();
            this.descriptionPanel = new System.Windows.Forms.Panel();
            this.titlePanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.optionsPanel.SuspendLayout();
            this.descriptionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // titlePanel
            // 
            this.titlePanel.Controls.Add(this.minimizeButton);
            this.titlePanel.Controls.Add(this.exitButton);
            this.titlePanel.Controls.Add(this.titleLabel);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(900, 56);
            this.titlePanel.TabIndex = 0;
            this.titlePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseDown);
            // 
            // minimizeButton
            // 
            this.minimizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeButton.FlatAppearance.BorderSize = 0;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.minimizeButton.Location = new System.Drawing.Point(828, 0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(36, 56);
            this.minimizeButton.TabIndex = 2;
            this.minimizeButton.Text = "_";
            this.minimizeButton.UseVisualStyleBackColor = true;
            this.minimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.FlatAppearance.BorderSize = 0;
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.exitButton.Location = new System.Drawing.Point(864, 0);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(36, 56);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "✕";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.titleLabel.Location = new System.Drawing.Point(24, 13);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(214, 30);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "XNull Cleanup Tool";
            this.titleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitlePanel_MouseDown);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.statusLabel);
            this.mainPanel.Controls.Add(this.progressBar);
            this.mainPanel.Controls.Add(this.cleanButton);
            this.mainPanel.Controls.Add(this.optionsPanel);
            this.mainPanel.Controls.Add(this.descriptionPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 56);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(24);
            this.mainPanel.Size = new System.Drawing.Size(900, 544);
            this.mainPanel.TabIndex = 1;
            // 
            // optionsPanel
            // 
            this.optionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.optionsPanel.Controls.Add(this.selectAllCheckBox);
            this.optionsPanel.Controls.Add(this.cleanupOptionsPanel);
            this.optionsPanel.Location = new System.Drawing.Point(24, 24);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(400, 400);
            this.optionsPanel.TabIndex = 8;
            // 
            // descriptionPanel
            // 
            this.descriptionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionPanel.Controls.Add(this.riskWarningLabel);
            this.descriptionPanel.Controls.Add(this.descriptionTextBox);
            this.descriptionPanel.Controls.Add(this.descriptionLabel);
            this.descriptionPanel.Location = new System.Drawing.Point(448, 24);
            this.descriptionPanel.Name = "descriptionPanel";
            this.descriptionPanel.Size = new System.Drawing.Size(428, 400);
            this.descriptionPanel.TabIndex = 9;
            // 
            // riskWarningLabel
            // 
            this.riskWarningLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.riskWarningLabel.AutoSize = true;
            this.riskWarningLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.riskWarningLabel.Location = new System.Drawing.Point(16, 370);
            this.riskWarningLabel.Name = "riskWarningLabel";
            this.riskWarningLabel.Size = new System.Drawing.Size(289, 19);
            this.riskWarningLabel.TabIndex = 7;
            this.riskWarningLabel.Text = "⚠️ WARNING: This option is potentially risky!";
            this.riskWarningLabel.Visible = false;
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descriptionTextBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.descriptionTextBox.Location = new System.Drawing.Point(16, 48);
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ReadOnly = true;
            this.descriptionTextBox.Size = new System.Drawing.Size(396, 316);
            this.descriptionTextBox.TabIndex = 6;
            this.descriptionTextBox.Text = "Select a cleanup option to see its description.";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.descriptionLabel.Location = new System.Drawing.Point(16, 16);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(96, 21);
            this.descriptionLabel.TabIndex = 5;
            this.descriptionLabel.Text = "Description:";
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.statusLabel.Location = new System.Drawing.Point(24, 480);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(46, 19);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Ready";
            this.statusLabel.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(24, 504);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(852, 16);
            this.progressBar.TabIndex = 3;
            this.progressBar.Visible = false;
            // 
            // cleanButton
            // 
            this.cleanButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.cleanButton.Location = new System.Drawing.Point(24, 440);
            this.cleanButton.Name = "cleanButton";
            this.cleanButton.Size = new System.Drawing.Size(852, 40);
            this.cleanButton.TabIndex = 2;
            this.cleanButton.Text = "Clean Selected Items";
            this.cleanButton.UseVisualStyleBackColor = true;
            this.cleanButton.Click += new System.EventHandler(this.CleanButton_Click);
            // 
            // selectAllCheckBox
            // 
            this.selectAllCheckBox.AutoSize = true;
            this.selectAllCheckBox.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.selectAllCheckBox.Location = new System.Drawing.Point(16, 16);
            this.selectAllCheckBox.Name = "selectAllCheckBox";
            this.selectAllCheckBox.Size = new System.Drawing.Size(91, 24);
            this.selectAllCheckBox.TabIndex = 1;
            this.selectAllCheckBox.Text = "Select All";
            this.selectAllCheckBox.UseVisualStyleBackColor = true;
            this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.SelectAllCheckBox_CheckedChanged);
            // 
            // cleanupOptionsPanel
            // 
            this.cleanupOptionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanupOptionsPanel.AutoScroll = true;
            this.cleanupOptionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.cleanupOptionsPanel.Location = new System.Drawing.Point(0, 48);
            this.cleanupOptionsPanel.Name = "cleanupOptionsPanel";
            this.cleanupOptionsPanel.Size = new System.Drawing.Size(400, 352);
            this.cleanupOptionsPanel.TabIndex = 0;
            this.cleanupOptionsPanel.WrapContents = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.titlePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XNull Cleanup Tool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
            this.descriptionPanel.ResumeLayout(false);
            this.descriptionPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel titlePanel;
        private Label titleLabel;
        private Button exitButton;
        private Button minimizeButton;
        private Panel mainPanel;
        private FlowLayoutPanel cleanupOptionsPanel;
        private CheckBox selectAllCheckBox;
        private Button cleanButton;
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label descriptionLabel;
        private RichTextBox descriptionTextBox;
        private Label riskWarningLabel;
        private Panel optionsPanel;
        private Panel descriptionPanel;
    }
} 
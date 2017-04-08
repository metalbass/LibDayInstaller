namespace LibDayDataExtractor
{
    partial class InstallationForm
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
            this.m_originalFilesPath = new System.Windows.Forms.TextBox();
            this.m_originalFilesBrowseButton = new System.Windows.Forms.Button();
            this.m_newFilesBrowseButton = new System.Windows.Forms.Button();
            this.m_newFilesPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_installButton = new System.Windows.Forms.Button();
            this.m_folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Liberation Day\'s disc drive or folder:";
            // 
            // m_originalFilesPath
            // 
            this.m_originalFilesPath.Location = new System.Drawing.Point(16, 30);
            this.m_originalFilesPath.Name = "m_originalFilesPath";
            this.m_originalFilesPath.Size = new System.Drawing.Size(228, 20);
            this.m_originalFilesPath.TabIndex = 1;
            this.m_originalFilesPath.Text = "D:\\";
            // 
            // m_originalFilesBrowseButton
            // 
            this.m_originalFilesBrowseButton.Location = new System.Drawing.Point(250, 28);
            this.m_originalFilesBrowseButton.Name = "m_originalFilesBrowseButton";
            this.m_originalFilesBrowseButton.Size = new System.Drawing.Size(32, 23);
            this.m_originalFilesBrowseButton.TabIndex = 2;
            this.m_originalFilesBrowseButton.Text = "...";
            this.m_originalFilesBrowseButton.UseVisualStyleBackColor = true;
            this.m_originalFilesBrowseButton.Click += new System.EventHandler(this.OnOriginalFilesBrowseButtonClick);
            // 
            // m_newFilesBrowseButton
            // 
            this.m_newFilesBrowseButton.Location = new System.Drawing.Point(250, 68);
            this.m_newFilesBrowseButton.Name = "m_newFilesBrowseButton";
            this.m_newFilesBrowseButton.Size = new System.Drawing.Size(32, 23);
            this.m_newFilesBrowseButton.TabIndex = 5;
            this.m_newFilesBrowseButton.Text = "...";
            this.m_newFilesBrowseButton.UseVisualStyleBackColor = true;
            this.m_newFilesBrowseButton.Click += new System.EventHandler(this.OnNewFilesBrowseButtonClick);
            // 
            // m_newFilesPath
            // 
            this.m_newFilesPath.Location = new System.Drawing.Point(15, 70);
            this.m_newFilesPath.Name = "m_newFilesPath";
            this.m_newFilesPath.Size = new System.Drawing.Size(229, 20);
            this.m_newFilesPath.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Installation path:";
            // 
            // m_installButton
            // 
            this.m_installButton.Location = new System.Drawing.Point(206, 98);
            this.m_installButton.Name = "m_installButton";
            this.m_installButton.Size = new System.Drawing.Size(75, 23);
            this.m_installButton.TabIndex = 6;
            this.m_installButton.Text = "Install";
            this.m_installButton.UseVisualStyleBackColor = true;
            this.m_installButton.Click += new System.EventHandler(this.OnInstallButtonClick);
            // 
            // InstallationForm
            // 
            this.ClientSize = new System.Drawing.Size(294, 128);
            this.Controls.Add(this.m_installButton);
            this.Controls.Add(this.m_newFilesBrowseButton);
            this.Controls.Add(this.m_newFilesPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_originalFilesBrowseButton);
            this.Controls.Add(this.m_originalFilesPath);
            this.Controls.Add(this.label1);
            this.Name = "InstallationForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_originalFilesPath;
        private System.Windows.Forms.Button m_originalFilesBrowseButton;
        private System.Windows.Forms.Button m_newFilesBrowseButton;
        private System.Windows.Forms.TextBox m_newFilesPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button m_installButton;
        private System.Windows.Forms.FolderBrowserDialog m_folderBrowserDialog;
    }
}


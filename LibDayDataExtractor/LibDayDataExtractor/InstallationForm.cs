using System;
using System.IO;
using System.Windows.Forms;

namespace LibDayDataExtractor
{
    public partial class InstallationForm : Form
    {
        public InstallationForm()
        {
            InitializeComponent();
        }

        private void OnOriginalFilesBrowseButtonClick(object sender, EventArgs e)
        {
            if (m_folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                m_originalFilesPath.Text = m_folderBrowserDialog.SelectedPath;
            }
        }

        private void OnNewFilesBrowseButtonClick(object sender, EventArgs e)
        {
            if (m_folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                m_newFilesPath.Text = m_folderBrowserDialog.SelectedPath;
            }
        }

        private void OnInstallButtonClick(object sender, EventArgs e)
        {
            if (ArePathsCorrect(m_originalFilesPath.Text, m_newFilesPath.Text))
            {
                new DataExtractor(m_originalFilesPath.Text, m_newFilesPath.Text).Start();
            }
        }

        private bool ArePathsCorrect(string originalFilesPath, string newFilesPath)
        {
            return Directory.Exists(originalFilesPath)
                && Directory.Exists(newFilesPath)
                && CheckPresenceOfLiberationDayFiles(originalFilesPath);
        }

        private bool CheckPresenceOfLiberationDayFiles(string originalFilesPath)
        {
            // TODO: we need to check if the path the user has selected has LibDay files.
            return true;
        }
    }
}

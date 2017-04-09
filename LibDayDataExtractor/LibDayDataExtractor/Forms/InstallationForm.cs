using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LibDayDataExtractor.Extractors;

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
                m_backgroundWorker.RunWorkerAsync();
            }
            else
            {
                // TODO: show error dialog
            }
        }

        private bool ArePathsCorrect(string originalFilesPath, string newFilesPath)
        {
            return Directory.Exists(originalFilesPath)
                && Directory.Exists(newFilesPath)
                && CheckPresenceOfLiberationDayFiles(originalFilesPath);
        }

        private static bool CheckPresenceOfLiberationDayFiles(string originalFilesPath)
        {
            // TODO: we need to check if the path the user has selected has LibDay files.
            return true;
        }

        private void StartBackgroundWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            new DataExtractor(m_originalFilesPath.Text, m_newFilesPath.Text).Start(worker);

            stopWatch.Stop();

            MessageBox.Show($"Finished after {stopWatch.Elapsed}");
        }

        private void OnBackgroundProgress(object sender, ProgressChangedEventArgs e)
        {
            m_progressBar.Value = e.ProgressPercentage;
        }
    }
}

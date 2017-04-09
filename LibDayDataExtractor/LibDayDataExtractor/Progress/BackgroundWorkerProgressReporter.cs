using System.ComponentModel;

namespace LibDayDataExtractor.Progress
{
    public class BackgroundWorkerProgressReporter : IProgressReporter
    {
        public BackgroundWorkerProgressReporter(BackgroundWorker worker)
        {
            m_worker = worker;
        }

        public float Progress { get; private set; }

        public void Report(float value)
        {
            Progress = value;

            m_worker.ReportProgress((int)(Progress * 100));
        }

        private BackgroundWorker m_worker;
    }

}

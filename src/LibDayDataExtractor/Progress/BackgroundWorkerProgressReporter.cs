using System;
using System.ComponentModel;

namespace LibDayDataExtractor.Progress
{
    public class BackgroundWorkerProgressReporter : IProgressReporter
    {
        public BackgroundWorkerProgressReporter(BackgroundWorker worker)
        {
            m_worker = worker;
        }

        public int Progress { get; private set; }

        public void Report(int value)
        {
            if (0 > value || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Progress = value;

            m_worker.ReportProgress(Progress);
        }

        private BackgroundWorker m_worker;
    }
}

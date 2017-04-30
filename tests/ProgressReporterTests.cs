using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibDayDataExtractor.Progress;

namespace LibDayDataExtractorTests
{
    [TestClass]
    public class ProgressReporterTests
    {
        [TestMethod]
        public void ProgressIsZeroAfterConstruction()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            Assert.AreEqual(0, parentProgress.Progress);
            Assert.AreEqual(0, progress.Progress);
        }

        [TestMethod]
        public void ProgressUpdatesWithSimpleProgress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.Report(50);

            Assert.AreEqual(50, parentProgress.Progress);
            Assert.AreEqual(50, progress.Progress);
        }

        [TestMethod]
        public void ProgressIsZeroAfterAddingOneSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);

            Assert.AreEqual(0, parentProgress.Progress);
            Assert.AreEqual(0, progress.Progress);
            Assert.AreEqual(0, progress[0].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingOneSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);

            progress[0].Report(40);

            Assert.AreEqual(40, parentProgress.Progress);
            Assert.AreEqual(40, progress.Progress);
            Assert.AreEqual(40, progress[0].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingTwoSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(2);

            progress[0].Report(100);
            progress[1].Report(50);

            Assert.AreEqual(75 , parentProgress.Progress);
            Assert.AreEqual(75 , progress.Progress);
            Assert.AreEqual(100, progress[0].Progress);
            Assert.AreEqual(50 , progress[1].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingTwoLevelsOfSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);
            progress[0].AddSubProgress(1);

            progress[0][0].Report(45);

            Assert.AreEqual(45, parentProgress.Progress);
            Assert.AreEqual(45, progress.Progress);
            Assert.AreEqual(45, progress[0].Progress);
            Assert.AreEqual(45, progress[0][0].Progress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProgressThrowsWithBigProgressReport()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.Report(150);
        }

        private class FakeProgressReporter : IProgressReporter
        {
            public int Progress
            {
                get; private set;
            }

            public void Report(int value)
            {
                Progress = value;
            }
        }
    }
}

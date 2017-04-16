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

            Assert.AreEqual(0f, parentProgress.Progress);
            Assert.AreEqual(0f, progress.Progress);
        }

        [TestMethod]
        public void ProgressUpdatesWithSimpleProgress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.Report(0.5f);

            Assert.AreEqual(0.5f, parentProgress.Progress);
            Assert.AreEqual(0.5f, progress.Progress);
        }

        [TestMethod]
        public void ProgressIsZeroAfterAddingOneSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);

            Assert.AreEqual(0f, parentProgress.Progress);
            Assert.AreEqual(0f, progress.Progress);
            Assert.AreEqual(0f, progress[0].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingOneSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);

            progress[0].Report(.4f);

            Assert.AreEqual(.4f, parentProgress.Progress);
            Assert.AreEqual(.4f, progress.Progress);
            Assert.AreEqual(.4f, progress[0].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingTwoSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(2);

            progress[0].Report(1f);
            progress[1].Report(.5f);

            Assert.AreEqual(.75f, parentProgress.Progress);
            Assert.AreEqual(.75f, progress.Progress);
            Assert.AreEqual(1f, progress[0].Progress);
            Assert.AreEqual(.5f, progress[1].Progress);
        }

        [TestMethod]
        public void ProgressAfterReportingTwoLevelsOfSubprogress()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.AddSubProgress(1);
            progress[0].AddSubProgress(1);

            progress[0][0].Report(.45f);

            Assert.AreEqual(.45f, parentProgress.Progress);
            Assert.AreEqual(.45f, progress.Progress);
            Assert.AreEqual(.45f, progress[0].Progress);
            Assert.AreEqual(.45f, progress[0][0].Progress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ProgressThrowsWithBigProgressReport()
        {
            IProgressReporter parentProgress = new FakeProgressReporter();
            ProgressReporter progress = new ProgressReporter(parentProgress);

            progress.Report(1.5f);
        }

        private class FakeProgressReporter : IProgressReporter
        {
            public float Progress
            {
                get; private set;
            }

            public void Report(float value)
            {
                Progress = value;
            }
        }
    }
}

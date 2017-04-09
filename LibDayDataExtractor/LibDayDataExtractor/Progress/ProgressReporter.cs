using System;
using System.Collections.Generic;
using System.Linq;

namespace LibDayDataExtractor.Progress
{
    public class ProgressReporter : IProgressReporter
    {
        public ProgressReporter(IProgressReporter parent)
        {
            m_parent = parent;

            m_tasksProgress = new List<Tuple<ProgressReporter, float>>();
        }

        public void AddSubProgress(int count, float weight = DefaultWeight)
        {
            for (int i = 0; i < count; i++)
            {
                m_tasksProgress.Add(Tuple.Create(new ProgressReporter(this), weight));
            }
        }

        public ProgressReporter this[int i]
        {
            get
            {
                if (i >= m_tasksProgress.Count)
                    throw new ArgumentOutOfRangeException(nameof(i));

                return m_tasksProgress[i].Item1;
            }
        }

        public float Progress
        {
            get
            {
                if (m_tasksProgress.Count == 0)
                {
                    return m_simpleProgress;
                }

                float totalWeight = m_tasksProgress.Select(x => x.Item2).Sum();

                float progress = 0;

                foreach (var part in m_tasksProgress)
                {
                    float relativeWeight = part.Item2 / totalWeight;

                    progress += relativeWeight * part.Item1.Progress;
                }

                return progress;
            }
        }

        public void Report(float value)
        {
            m_simpleProgress = value;

            m_parent.Report(Progress);
        }

        private const float DefaultWeight = 1;

        private List<Tuple<ProgressReporter, float>> m_tasksProgress;
        private IProgressReporter m_parent;

        private float m_simpleProgress;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibDayDataExtractor.Progress
{
    public class ProgressReporter : IProgressReporter
    {
        public ProgressReporter(IProgressReporter parent)
        {
            m_parent = parent;

            m_tasksProgress = new List<Tuple<ProgressReporter, float>>();

            m_uniformWeight = DefaultWeight;
        }

        public void AddSubProgress(int count, float weight = DefaultWeight)
        {
            if (m_tasksProgress.Count == 0)
            {
                m_uniformWeight = weight;
            }
            else if (m_uniformWeight.HasValue)
            {
                if (m_uniformWeight.Value != weight)
                {
                    m_uniformWeight = null;
                }
            }

            for (int i = 0; i < count; i++)
            {
                m_tasksProgress.Add(Tuple.Create(new ProgressReporter(this), weight));
            }
        }

        public ProgressReporter this[int index]
        {
            get
            {
                if (index >= m_tasksProgress.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return m_tasksProgress[index].Item1;
            }
        }

        public int Progress
        {
            get
            {
                if (m_tasksProgress.Count == 0 || !m_dirty)
                {
                    return m_simpleProgress;
                }

                if (m_uniformWeight.HasValue)
                {
                    m_simpleProgress = ComputeProgressWithUniformWeights();
                }
                else
                {
                    m_simpleProgress = ComputeProgressWithNonUniformWeights();
                }

                m_dirty = false;
                return m_simpleProgress;
            }
        }

        public void Report(int value)
        {
            if (0 > value || value > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            m_dirty = true;
            m_simpleProgress = value;

            m_parent.Report(Progress);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            ToString(builder, indentationLevel: 0);

            return builder.ToString();
        }

        private int ComputeProgressWithUniformWeights()
        {
            int accumulatedProgress = m_tasksProgress.Select(x => x.Item1.Progress).Sum();

            return accumulatedProgress / m_tasksProgress.Count;
        }

        private int ComputeProgressWithNonUniformWeights()
        {
            double totalWeight = m_tasksProgress.Select(x => x.Item2).Sum();

            double progress = 0;

            foreach (var part in m_tasksProgress)
            {
                progress += part.Item1.Progress * part.Item2 / totalWeight;
            }

            return (int)progress;
        }

        private void ToString(StringBuilder builder, int indentationLevel)
        {
            if (m_tasksProgress.Count == 0)
            {
                builder.AppendFormat("Simple {0:000}%\n", this.m_simpleProgress);
                return;
            }

            builder.AppendFormat("Composite {0:000}% {1:0000} children, total weight {2:0000} \n",
                Progress, m_tasksProgress.Count, m_tasksProgress.Select(x => x.Item2).Sum());

            for (int i = 0; i < m_tasksProgress.Count; i++)
            {
                builder.Append('\t', indentationLevel + 1);
                builder.AppendFormat("{0:0000} w: {1:0000} ", i, m_tasksProgress[i].Item2);
                m_tasksProgress[i].Item1.ToString(builder, indentationLevel + 1);
            }
        }

        private const float DefaultWeight = 1;

        private List<Tuple<ProgressReporter, float>> m_tasksProgress;
        private IProgressReporter m_parent;

        private int m_simpleProgress;
        private float? m_uniformWeight;
        private bool m_dirty;
    }
}

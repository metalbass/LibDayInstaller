using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDayDataExtractor.Progress
{
    public interface IProgressReporter : IProgress<float>
    {
        float Progress { get; }
    }
}

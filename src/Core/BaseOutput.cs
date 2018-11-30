using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal enum OutputLevel{
        BASIC = 0,
        MATCHING = 1,
        DETAILED = 2
    }
    internal abstract class BaseOutput{
        public abstract void Write(List<FileMatchingScore> results, OutputLevel level = OutputLevel.BASIC);
    }
}
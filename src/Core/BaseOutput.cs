using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal abstract class BaseOutput{
        public abstract void Write(List<FileMatchingScore> results, bool details = false);
    }
}
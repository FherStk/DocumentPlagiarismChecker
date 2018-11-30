using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal abstract class BaseOutput{
        public abstract void Write(List<Result> results, int level);
    }
}
using System.Collections.Generic;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseOutput{
        public abstract void Write(List<ResultHeader> results, int level);
    }
}
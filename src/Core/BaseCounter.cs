using System.Collections.Generic;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseCounter<T> where T: BaseDocument{
        public void Run(string path){
            List<T> docs = Parse(path);
            List<ResultHeader> res = Compare(docs);
            SendToOutput(res);
        }

        protected abstract List<T> Parse(string path);
        protected abstract List<ResultHeader>  Compare(List<T> input);
        protected abstract void  SendToOutput(List<ResultHeader> results, int level = 0);
    }
}
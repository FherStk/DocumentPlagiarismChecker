using System.Collections.Generic;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseCounter{
        public void Run(string path){
            List<BaseDocument> docs = Parse(path);
            List<ResultHeader> res = Compare(docs);
            SendToOutput(res);
        }

        protected abstract List<BaseDocument> Parse(string path);
        protected abstract List<ResultHeader>  Compare(List<BaseDocument> input);
        protected abstract void  SendToOutput(List<ResultHeader> results, int level = 0);
    }
}
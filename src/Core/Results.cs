using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal class Results{                                 
        public List<FilePairResults> ResultFiles {get; set;}
       
        private static Results instance;

        private Results() {}

        public static Results Instance
        {
            get{
                if(instance == null){
                    instance = new Results();
                    instance.ResultFiles = new List<FilePairResults>();
                }
                return instance;
            }
        }                       
    }      
}
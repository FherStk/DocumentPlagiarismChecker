using System.Collections.Generic;
using System.Linq;

namespace DocumentPlagiarismChecker.Core
{
    internal class FilePairResults{
        public string LeftFileName {get; private set;}
        public string RightFileName {get; private set;}      
        public float Matching {get; private set;}            
        public List<ComparerResults> ComparerResults{get; set;}                   
        
        public FilePairResults(string leftFileName, string rightFileName){
            this.Matching = 0;
            this.LeftFileName = leftFileName;
            this.RightFileName = rightFileName;            
            this.ComparerResults = new List<ComparerResults>();
        }             
    }      
}
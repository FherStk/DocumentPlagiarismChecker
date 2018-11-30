using System.Collections.Generic;
using System.Linq;

namespace DocumentPlagiarismChecker.Core
{
    internal class FileMatchingScore{
        public string LeftFileName {get; private set;}
        public string RightFileName {get; private set;}      
        public List<ComparerMatchingScore> ComparerResults{get; set;}
        public float Matching {
            get{
                 return (ComparerResults.Count == 0 ? 0 : ComparerResults.Sum(x => x.Matching)/ComparerResults.Count);
            }
        }
        
        
        public FileMatchingScore(string leftFileName, string rightFileName){
            this.LeftFileName = leftFileName;
            this.RightFileName = rightFileName;            
            this.ComparerResults = new List<ComparerMatchingScore>();
        }             
    }      
}
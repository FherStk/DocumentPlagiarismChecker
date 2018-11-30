using System.Collections.Generic;
using System.Linq;

namespace DocumentPlagiarismChecker.Core
{
    internal class FileMatchingScore{
        public string LeftFileName {get; private set;}
        public string RightFileName {get; private set;}      
        public float Matching {get; private set;}            
        public List<ComparerMatchingScore> ComparerResults{get; set;}                   
        
        public FileMatchingScore(string leftFileName, string rightFileName){
            this.Matching = 0;
            this.LeftFileName = leftFileName;
            this.RightFileName = rightFileName;            
            this.ComparerResults = new List<ComparerMatchingScore>();
        }             
    }      
}
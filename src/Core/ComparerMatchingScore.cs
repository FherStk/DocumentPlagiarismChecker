using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal class ComparerMatchingScore{       
        public string Comparer {get; private set;}              
        public string[] DetailsCaption {get; set;}
        public List<string[]> DetailsData {get; set;}   
        private List<float> _matching;
        public float Matching {
            get{                
                return (_matching.Count == 0 ? 0 : _matching.Sum(x => x)/_matching.Count);
            }            
        }     

        public void AddMatch(float match){
              _matching.Add(match);
        }
        public ComparerMatchingScore(string comparer){
            _matching = new List<float>();
            this.DetailsData = new List<string[]>();
            this.Comparer = comparer;                        
        }
    }      
}
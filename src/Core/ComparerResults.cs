using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal class ComparerResults{       
        public string Comparer {get; private set;}              
        public string[] DetailsCaption {get; private set;}
        public List<string[]> DetailsData {get; private set;}   
        private List<float> _matching;
        public float Matching {
            get{                
                return (_matching.Count == 0 ? 0 : _matching.Sum(x => x)/_matching.Count);
            }
        }

        public ComparerResults(string comparer){
            this.Comparer = comparer;
        }
    }      
}
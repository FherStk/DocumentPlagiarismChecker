using System.Collections.Generic;
using System.Linq;

namespace PdfPlagiarismChecker.Core
{
    internal class ResultHeader{
        public string Comparer {get; private set;}
        public string LeftCaption {get; private set;}
        public string RightCaption {get; private set;}      
        public float Matching {get; private set;}            
        private Dictionary<string, ResultLine> _lines;
        public List<ResultLine> Lines {
            get{
                return _lines.Values.ToList();
            } 
            private set{
                _lines = value.ToDictionary(x => x.Item, x => x);
            }
        }
        
        public ResultHeader(string comparer, string leftCaption, string rightCaption){
            this.Comparer = comparer;
            this.LeftCaption = leftCaption;
            this.RightCaption = rightCaption;
            this.Lines = new List<ResultLine>();
        }

        public ResultLine AddRight(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.RightValue += appearence; 

            Refresh();  
            return rl;         
        }

        public ResultLine AddLeft(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.LeftValue += appearence;    

            Refresh();  
            return rl;        
        }

        private ResultLine GetLine(string word){
            ResultLine rl = null;
            if(_lines.ContainsKey(word)) rl = _lines[word];
            else
            {
                rl =  new ResultLine(word);
                _lines.Add(word, rl);
            }            

            return rl; 
        } 
        
        private void Refresh(){
            int count = 0;
            float total = 0;
            foreach(ResultLine rl in this.Lines){
                total += rl.Matching;
                count++;
            }
            
            this.Matching = (count > 0 ?  total / count : 0);
        }         
    }      
}
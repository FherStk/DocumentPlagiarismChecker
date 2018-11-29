using System.Collections.Generic;
using System.Linq;

namespace PdfPlagiarismChecker.Comparers.WordCounter
{
    internal class ResultHeader{
        public string _left;
        public string Left {
            get{
                return _left;
            } 
            private set{
                _left = value;
            }
        }
        public string _right;
        public string Right {
            get{
                return _right;
            } 
            private set{
                _right = value;
            }
        }
        public float _matching;
        public float Matching {
            get{
                return _matching;
            } 
            private set{
                _matching = value;
            }
        }
        public Dictionary<string, ResultLine> _lines;
        public List<ResultLine> Lines {
            get{
                return _lines.Values.ToList();
            } 
            private set{
                _lines = value.ToDictionary(x => x.Word, x => x);
            }
        }
        
        public ResultHeader(string left, string right){
            this.Left = left;
            this.Right = right;
            this.Lines = new List<ResultLine>();
        }

        public void AddRight(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.AppearenceRight += appearence; 

            Refresh();           
        }

        public void AddLeft(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.AppearenceLeft += appearence;    

            Refresh();          
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
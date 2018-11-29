using System.Collections.Generic;
using System.Linq;

namespace PdfPlagiarismChecker.Core
{
    internal class ResultHeader{
        private string _leftCaption;
        public string LeftCaption {
            get{
                return _leftCaption;
            } 
            private set{
                _leftCaption = value;
            }
        }
        private string _rightCaption;
        public string RightCaption {
            get{
                return _rightCaption;
            } 
            private set{
                _rightCaption = value;
            }
        }
        private float _matching;
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
                _lines = value.ToDictionary(x => x.Item, x => x);
            }
        }
        
        public ResultHeader(string left, string right){
            this.LeftCaption = left;
            this.RightCaption = right;
            this.Lines = new List<ResultLine>();
        }

        public void AddRight(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.RightValue += appearence; 

            Refresh();           
        }

        public void AddLeft(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.LeftValue += appearence;    

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
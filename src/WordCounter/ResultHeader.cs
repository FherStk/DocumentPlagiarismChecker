using System.Collections.Generic;
using System.Linq;

namespace PdfPlagiarismChecker.WordCounter
{
    public class ResultHeader{
        public string left {get;}
        public string right {get;}
        public float similitude {get; private set;}

        public Dictionary<string, ResultLine> lines {get; private set;}
        
        public ResultHeader(string left, string right){
            this.left = left;
            this.right = right;
            this.lines = new Dictionary<string, ResultLine>();
        }

        public void AddRight(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.appearenceRight += appearence; 

            Refresh();           
        }

        public void AddLeft(string word, int appearence){
            ResultLine rl = GetLine(word);            
            rl.appearenceLeft += appearence;    

            Refresh();          
        }

        private ResultLine GetLine(string word){
            ResultLine rl = null;
            if(this.lines.ContainsKey(word)) rl = this.lines[word];
            else
            {
                rl =  new ResultLine(word);
                this.lines.Add(word, rl);
            }            

            return rl; 
        } 

        private void Refresh(){
            int count = 0;
            float total = 0;
            foreach(ResultLine rl in this.lines.Select(x => x.Value)){
                total += rl.similitude;
                count++;
            }
            
            this.similitude = (count > 0 ?  total / count : 0);
        }         
    }      
}
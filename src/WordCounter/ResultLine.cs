namespace PdfPlagiarismChecker.WordCounter
{
    internal class ResultLine{
        public string _word;
         public string Word {
            get{
                return _word;
            } 
            set{
                _word = value;
            }
        }

        private int _appearenceLeft;
        public int AppearenceLeft {
            get{
                return _appearenceLeft;
            } 
            set{
                _appearenceLeft = value;
                Refresh();
            }
        }
        private int _appearenceRight;
        public int AppearenceRight {
            get{
                return _appearenceRight;
            } 
            set{
                _appearenceRight = value;
                Refresh();
            }
        }
        public float _matching;
         public float Matching {
            get{
                return _matching;
            } 
            set{
                _matching = value;
            }
        }

        public  ResultLine(string word){
            this.Word = word;
            this.Matching = 0;
            _appearenceLeft = 0;
            _appearenceRight = 0;            
        } 

        private void Refresh(){
            if(this.AppearenceLeft == 0 || this.AppearenceRight == 0) 
                this.Matching = 0;
            else
                this.Matching = (this.AppearenceLeft < this.AppearenceRight ? (float)this.AppearenceLeft / (float)this.AppearenceRight : (float)this.AppearenceRight / (float)this.AppearenceLeft);
        }      
    }      
}
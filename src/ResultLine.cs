namespace PdfPlagiarismChecker
{
    public class ResultLine{
        public string word {get;}

        private int _appearenceLeft;
        public int appearenceLeft {
            get{
                return _appearenceLeft;
            } 
            set{
                _appearenceLeft = value;
                Refresh();
            }
        }
        private int _appearenceRight;
        public int appearenceRight {
            get{
                return _appearenceRight;
            } 
            set{
                _appearenceRight = value;
                Refresh();
            }
        }
        public float similitude {get; private set;}

        public  ResultLine(string word){
            this.word = word;
            this.similitude = 0;
            _appearenceLeft = 0;
            _appearenceRight = 0;            
        } 

        private void Refresh(){
            if(this.appearenceLeft == 0 || this.appearenceRight == 0) 
                this.similitude = 0;
            else
                this.similitude = (this.appearenceLeft < this.appearenceRight ? (float)this.appearenceLeft / (float)this.appearenceRight : (float)this.appearenceRight / (float)this.appearenceLeft);
        }      
    }      
}
namespace PdfPlagiarismChecker.Core
{
    internal class ResultLine{       
        private string _item;
        public string Item {
            get{
                return _item;
            } 
            set{
                _item = value;
            }
        }

        private int _leftValue;
        public int LeftValue {
            get{
                return _leftValue;
            } 
            set{
                _leftValue = value;
                Refresh();
            }
        }
        private int _rightValue;
        public int RightValue {
            get{
                return _rightValue;
            } 
            set{
                _rightValue = value;
                Refresh();
            }
        }
        private float _matching;
        public float Matching {
            get{
                return _matching;
            } 
            set{
                _matching = value;
            }
        }

        public  ResultLine(string word){
            this.Item = word;
            this.Matching = 0;
            _leftValue = 0;
            _rightValue = 0;            
        } 

        private void Refresh(){
            if(this.LeftValue == 0 || this.RightValue == 0) 
                this.Matching = 0;
            else
                this.Matching = (this.LeftValue < this.RightValue ? (float)this.LeftValue / (float)this.RightValue : (float)this.RightValue / (float)this.LeftValue);
        }      
    }      
}
namespace DocumentPlagiarismChecker.Core
{
    internal class ResultLine{       
        public string Item {get; set;}      
        public float Matching {get; private set;}     
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

        public  ResultLine(string word){
            this.Item = word;       
        } 

        private void Refresh(){
            if(this.LeftValue == 0 || this.RightValue == 0) 
                this.Matching = 0;
            else
                this.Matching = (this.LeftValue < this.RightValue ? (float)this.LeftValue / (float)this.RightValue : (float)this.RightValue / (float)this.LeftValue);
        }      
    }      
}
using System.Collections.Generic;
using System.Linq;

namespace PdfPlagiarismChecker.Core
{
    internal class Result{     
        public float Matching {get; private set;}            
        private Dictionary<string, ResultHeader> _headers;
        public List<ResultHeader> Headers {
            get{
                return _headers.Values.ToList();
            } 
            private set{
                _headers = value.ToDictionary(x => x.Comparer, x => x);
            }
        }
        
        public Result(){
            this.Headers = new List<ResultHeader>();
        }
       
        public ResultHeader AddHeader(string comparer, string leftCaption, string rightCaption){
            ResultHeader rh = null;
            if(_headers.ContainsKey(comparer)) rh = _headers[comparer];
            else
            {
                rh =  new ResultHeader(comparer, leftCaption, rightCaption);
                _headers.Add(comparer, rh);
            }            

            Refresh(); 
            return rh;         
        }     
        
        //TODO: must be private and be auto-called. Dirty fix during migration...
        public void Refresh(){
            int count = 0;
            float total = 0;
            foreach(ResultHeader rh in this.Headers){
                total += rh.Matching;
                count++;
            }
            
            this.Matching = (count > 0 ?  total / count : 0);
        }         
    }      
}
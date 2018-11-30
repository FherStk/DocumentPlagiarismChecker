using System.Collections.Generic;
using System.Linq;

namespace DocumentPlagiarismChecker.Core
{
    internal class Result{                         
        private Dictionary<string, ResultHeader> _headers;
        public List<ResultHeader> Headers {
            get{
                return _headers.Values.ToList();
            } 
            private set{
                _headers = value.ToDictionary(x => x.Comparer, x => x);
            }
        }    
       
        private static Result instance;

        private Result() {}

        public static Result Instance
        {
            get{
                if(instance == null){
                    instance = new Result();
                    instance.Headers = new List<ResultHeader>();
                }
                return instance;
            }
        }

        public ResultHeader AddHeader(string comparer, string leftCaption, string rightCaption){
            ResultHeader rh = null;
            if(_headers.ContainsKey(comparer)) rh = _headers[comparer];
            else
            {
                rh =  new ResultHeader(comparer, leftCaption, rightCaption);
                _headers.Add(comparer, rh);
            }            

            return rh;         
        }                    
    }      
}
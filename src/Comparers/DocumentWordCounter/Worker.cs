using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparers.WordCounter
{
    internal class Worker: Core.BaseWorker<Document>
    {  
        internal Worker(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
        

        /// <summary>
        /// Counts how many words and how many times appears within each document, and checks the matching percentage.
        /// </summary>
        /// <returns></returns>
        public override void Run(){            
            //TODO: each comparer must own its own ResultHeader and add it to a global Result that will be sent to print.
            //this is just a dirty hard-code patch for transition and test.            
            foreach(var wLeft in this.Left.Words)
                this.ResultHeader.AddLeft(wLeft.Text, wLeft.Count);

            foreach(var wRight in this.Right.Words)
                this.ResultHeader.AddRight(wRight.Text, wRight.Count);
        }    
    }   
}
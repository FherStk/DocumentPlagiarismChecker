using System.IO;
using System.Linq;
using System.Collections.Generic;
using PdfPlagiarismChecker.Core;

namespace PdfPlagiarismChecker.Comparers.WordCounter
{
    internal class Worker: Core.BaseWorker<Document>
    {               
        /// <summary>
        /// Counts how many words and how many times appears within each document.
        /// </summary>
        /// <param name="input">A set of documents</param>
        /// <returns></returns>
        protected override List<ResultHeader> Compare(List<Document> input){
            //Col1=Word; Col2=#Appearences in File1; Col3=#Appearences in File2; Col4=%Coincidences
            //Last=%Global Coincidences

            Document doc1 = null;
            Document doc2 = null;              
            List<ResultHeader> result = new List<ResultHeader>();
            for(int i = 0; i < input.Count(); i++){                                
                doc1 = input.ElementAt(i);
             
                for(int j = i+1; j < input.Count(); j++){                                
                    doc2 = input.ElementAt(j);
                    result.Add(Compare(doc1, doc2));
                }                    
            }

            return result;
        }

        /// <summary>
        /// Counts how many words and how many times appears within each document, and checks the matching percentage.
        /// </summary>
        /// <param name="left">Left-side document</param>
        /// <param name="right">Right-side document</param>
        /// <returns></returns>
        private static ResultHeader Compare(Document left,  Document right){
            ResultHeader r = new ResultHeader(left.Name, right.Name);
            
            foreach(var wLeft in left.Words)
                r.AddLeft(wLeft.Text, wLeft.Count);

            foreach(var wRight in right.Words)
                r.AddRight(wRight.Text, wRight.Count);

            
            return r;
        }    
    }   
}
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PdfPlagiarismChecker.Core;

namespace PdfPlagiarismChecker.Comparers.WordCounter
{
    internal class Counter: Core.BaseCounter<Document>
    {       
        /// <summary>
        /// Goes through all the PDF files stored into the given path (not recursively) and counts how many words and how many times appears in each document.
        /// </summary>
        /// <param name="path">The folder where the PDF files are stored.</param>
        /// <returns>A set of Content items</returns>
        protected override List<Document> Parse(string path){
            //Check pre-conditions
            if(!Directory.Exists(path)) 
                throw new FolderNotFoundException();
            
            //Loop over all the PDF files inside the folder
            List<Document> res = new List<Document>();
            foreach(string filePath in Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower().Equals(".pdf")))
                res.Add(new Document(filePath));                       

            return res;
        }

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

        private static ResultHeader Compare(Document left,  Document right){
            ResultHeader r = new ResultHeader(left.Name, right.Name);
            
            foreach(var wLeft in left.Words)
                r.AddLeft(wLeft.Text, wLeft.Count);

            foreach(var wRight in right.Words)
                r.AddRight(wRight.Text, wRight.Count);

            
            return r;
        }

        protected override void  SendToOutput(List<ResultHeader> results, int level = 0){
            Outputs.Terminal output = new Outputs.Terminal();
            output.Write(results, level);
        }        
    }   
}
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace PdfPlagiarismChecker.WordCounter
{
    class Worker
    {

        public static void Run(string path){
            List<Document> docs = Parse(path);
            List<ResultHeader> res = Compare(docs);  
            Print(res);      
        }

        /// <summary>
        /// Goes through all the PDF files stored into the given path (not recursively) and counts how many words and how many times appears in each document.
        /// </summary>
        /// <param name="path">The folder where the PDF files are stored.</param>
        /// <returns>A set of Content items</returns>
        private static List<Document> Parse(string path){
            //Check pre-conditions
            if(!Directory.Exists(path)) 
                throw new FolderNotFoundException();
            
            //Loop over all the PDF files inside the folder
            List<Document> res = new List<Document>();
            foreach(string filePath in Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower().Equals(".pdf")))                
                 res.Add(new Document(filePath));                

            return res;
        }

        private static List<ResultHeader> Compare(List<Document> input){
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
            ResultHeader r = new ResultHeader(left.name, right.name);
            
            foreach(var wLeft in left.words)
                r.AddLeft(wLeft.text, wLeft.count);

            foreach(var wRight in right.words)
                r.AddRight(wRight.text, wRight.count);

            
            return r;
        }

        private static void Print(List<ResultHeader> results){
            foreach(ResultHeader r in results){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Left file: {0}", r.left);
                System.Console.WriteLine("Right file: {0}", r.right);
                System.Console.WriteLine("Similitude: {0}%", System.Math.Round(r.similitude*100, 2));
                System.Console.WriteLine("------------------------------------------------------------------------------");

                foreach(ResultLine rl in r.lines.Values){
                    System.Console.WriteLine("Word: {0} | Left: {1} | Right: {2} | Similitude: {3}%", rl.word, rl.appearenceLeft, rl.appearenceRight, System.Math.Round(rl.similitude*100, 2));
                }

                System.Console.WriteLine("");
            }
        }
    }   
}
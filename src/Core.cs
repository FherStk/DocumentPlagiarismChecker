using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace PdfPlagiarismChecker
{
    class Core
    {

        /// <summary>
        /// Goes through all the PDF files stored into the given path (not recursively) and counts how many words and how many times appears in each document.
        /// </summary>
        /// <param name="path">The folder where the PDF files are stored.</param>
        /// <returns>A set of Content items</returns>
        private List<Content> Extract(string path){
            //Check pre-conditions
            if(!Directory.Exists(path)) 
                throw new FolderNotFoundException();
            
            //Loop over all the PDF files inside the folder
            List<Content> res = new List<Content>();
            foreach(string filePath in Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower().Equals("pdf")))                
                 res.Add(new Content(filePath));                

            return res;
        }

        private void Anaylze(List<Content> input){
            //Col1=Word; Col2=#Appearences in File1; Col3=#Appearences in File2; Col4=%Coincidences
            //Last=%Global Coincidences

            Content doc1 = null;
            Content doc2 = null;  

            for(int i = 0; i < input.Count(); i++){                                
                doc1 = input.ElementAt(i);
             
                for(int j = i+1; j < input.Count(); j++){                                
                    doc2 = input.ElementAt(j);                                    
                }                    
            }
        }

        private void Compare(Content c1,  Content c2){
            
        }
    }   
}
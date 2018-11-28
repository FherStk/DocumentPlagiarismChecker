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
        /// <returns>A dictionary containing pairs of [FILE_NAME; RESULTS] on each RESULT is also a dictionary containing pairs of [WORD; #APPEARENCE] for each previous file.</returns>
        private Dictionary<string, Dictionary<string, int>> Extract(string path){
            //Check pre-conditions
            if(!Directory.Exists(path)) 
                throw new FolderNotFoundException();

            //Init pre-loop vars.
            PrTokeniser tokenizer = null;
            PdfReader reader = null;
            Dictionary<string, int> counter = null;
            Dictionary<string, Dictionary<string, int>> result = new Dictionary<string, Dictionary<string, int>>();            

            //Loop over all the PDF files inside the folder
            foreach(string filePath in Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower().Equals("pdf"))){
                reader = new PdfReader(filePath);
                tokenizer = new PrTokeniser(new RandomAccessFileOrArray(reader.GetPageContent(1)));

                //Store the appearence of each word inside the document.
                counter = new Dictionary<string, int>();
                while (tokenizer.NextToken())
                {
                    if (tokenizer.TokenType == PrTokeniser.TK_STRING)
                    {
                        if(!counter.ContainsKey(tokenizer.StringValue))
                            counter.Add(tokenizer.StringValue, 0);
                        
                        counter[tokenizer.StringValue]++;                        
                    }
                }

                //Store the result of each file.
                result.Add(Path.GetFileName(filePath), counter);
                reader.Close();
            }

            return result;
        }

        private void Anaylze(Dictionary<string, Dictionary<string, int>> input){
            //Col1=Word; Col2=#Appearences in File1; Col3=#Appearences in File2; Col4=%Coincidences
            //Last=%Global Coincidences

            Dictionary<string, int> doc1 = null;
            Dictionary<string, int> doc2 = null;

            //incluir visitados
            foreach(string docName1 in input.Keys){
                foreach(string docName2 in input.Keys){
                    if(docName1.Equals(docName2)) 
                        continue;

                    doc1 = input[docName1];
                    doc2 = input[docName2];
                }

                input.Remove(docName1);
            }
        }
    }   
}
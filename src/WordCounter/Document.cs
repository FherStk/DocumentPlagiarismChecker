using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfPlagiarismChecker.WordCounter
{
    class Document
    {
        public string name {get;}
        public List<Word> words {get; private set;}            

        /// <summary>
        /// Loads the content of a PDF file and counts how many words and how many times appears along the document.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path){
            //Check pre-conditions
            if(!File.Exists(path)) 
                throw new FolderNotFoundException();

            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new FileNotPdfException();

            //Init object attributes.
            this.name = System.IO.Path.GetFileName(path);

            Dictionary<string, Word> scan = new Dictionary<string, Word>();
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    text = text.Replace("\n", "");

                    foreach(string word in text.Split(" ").Where(x => x.Length > 0)){
                        if(!scan.ContainsKey(word))
                            scan.Add(word, new Word(){text = word, count = 0});
                                    
                        scan[word].count++;     
                    }
                }
            }            

            this.words = scan.Values.ToList();
        }
    }
}
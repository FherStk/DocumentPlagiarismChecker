using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfPlagiarismChecker.WordCounter
{
    internal class Document
    {
        private string _name;
        public string Name {
            get{
                return _name;
            } 
            private set{
                _name = value;
            }
        }
        private Dictionary<string, Word> _words;
        public List<Word> Words {
            get{
                return _words.Values.ToList();
            } 
            private set{
                _words = value.ToDictionary(x => x.text, x => x);
            }
        }       

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
            this.Name = System.IO.Path.GetFileName(path);
            this._words = new Dictionary<string, Word>();

            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    text = text.Replace("\n", "");

                    foreach(string word in text.Split(" ").Where(x => x.Length > 0)){
                        if(!this._words.ContainsKey(word))
                            this._words.Add(word, new Word(){text = word, count = 0});
                                    
                        this._words[word].count++;     
                    }
                }
            }            
        }
    }
}
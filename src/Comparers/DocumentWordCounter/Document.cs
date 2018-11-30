using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Comparers.WordCounter
{
    internal class Document: Core.BaseDocument
    {        
        private Dictionary<string, Word> _words;
        public List<Word> Words {
            get{
                return _words.Values.ToList();
            } 
            private set{
                _words = value.ToDictionary(x => x.Text, x => x);
            }
        }       

        /// <summary>
        /// Loads the content of a PDF file and counts how many words and how many times appears along the document.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new FileNotPdfException();

            //Init object attributes.
            _words = new Dictionary<string, Word>();

            //Read PDF file and sotre the word count.
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    text = text.Replace("\n", "");

                    foreach(string word in text.Split(" ").Where(x => x.Length > 0)){
                        if(!_words.ContainsKey(word))
                            _words.Add(word, new Word(){Text = word, Count = 0});
                                    
                        _words[word].Count++;     
                    }
                }
            }            
        }
    }
}
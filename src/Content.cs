using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace PdfPlagiarismChecker
{
    class Content
    {
        private string name {get;}
        private Dictionary<string, int> words;

        /// <summary>
        /// Loads the content of a PDF file and counts how many words and how many times appears along the document.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public Content(string filePath){
            //Check pre-conditions
            if(!File.Exists(filePath)) 
                throw new FolderNotFoundException();

            if(!Path.GetExtension(filePath).ToLower().Equals("pdf"))
                throw new FileNotPdfException();

            //Init object attributes.
            this.name = Path.GetFileName(filePath);
            this.words = new Dictionary<string, int>();

            //Read pdf file
            PdfReader reader = new PdfReader(filePath);
            PrTokeniser tokenizer = new PrTokeniser(new RandomAccessFileOrArray(reader.GetPageContent(1)));

            //Store the appearence of each word inside the document.                
            while (tokenizer.NextToken())
            {
                if (tokenizer.TokenType == PrTokeniser.TK_STRING)                        
                    this.AddWord(tokenizer.StringValue);
            }

            reader.Close();
        }

        private void AddWord(string word){
             if(!words.ContainsKey(word))
                words.Add(word, 0);
                        
            words[word]++;     
        }
    }
}
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Comparators.SentencePartialMatch
{
    /// <summary>
    /// This document must be used with the Sentence Word Match Comparator, and stores all the sentences inside a document.
    /// </summary>
    internal class Document: Core.BaseDocument
    {   
        internal class TextLine{
            public int Count {
                get{
                    return this.Words.Count;
                }
            }
            public List<string> Words {get; private set;}
            private Dictionary<string, List<int>> Index {get; set;}
            public string Text{
                get{
                    return string.Join(" ", this.Words);
                }
            }

            public TextLine(){
                this.Words = new List<string>();
                this.Index = new Dictionary<string, List<int>>();
            }

            public void AddWord(string word){
                //First removes hidden chars from any word and all the punctuation symbols; also diacritics are removed.
                string clean = RemoveDiacritics(new string(word.Where(c =>  !char.IsPunctuation(c) && (char.IsLetterOrDigit(c) || c.Equals('’') || (c >= ' ' && c <= byte.MaxValue))).ToArray()).Trim());                
                if(clean.Length > 0){
                    //Now the word is added to the collection and also indexed.
                    if(!this.Index.ContainsKey(clean.ToLower())) this.Index.Add(clean.ToLower(), new List<int>());
                    this.Index[clean.ToLower()].Add(this.Count);
                    this.Words.Add(clean);
                }
            }

            public bool ContainsWord(string word){
                return this.Index.ContainsKey(word.ToLower());
            }

            public List<int> GetIndex(string word){
                return this.Index[word.ToLower()];
            }

            private string RemoveDiacritics(string text) 
            {
                //Source: https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
                string normalizedString = text.Normalize(NormalizationForm.FormD);
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var c in normalizedString)
                {
                    UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                        stringBuilder.Append(c);                    
                }

                return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }
        }

        /// <summary>
        /// Contains the phrases inside the document.
        /// </summary>
        /// <value></value>
        public Dictionary<string, TextLine> Sentences {get; set;}        

        /// <summary>
        /// Loads the content of a PDF file and gets all the phrases inside it.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new Exceptions.FileExtensionNotAllowed();

            //Init object attributes.
            Sentences = new Dictionary<string, TextLine>();            

            //Read PDF file and sotre each word appearence inside its paragraph.
            using (PdfReader reader = new PdfReader(path))
            {
                /*
                //Note: use this in order to extract paragraphs (experimental)
                Utils.TextAsParagraphsExtractionStrategy paragraphReader = new Utils.TextAsParagraphsExtractionStrategy();
                for (int i = 1; i <= reader.NumberOfPages; i++)
                    PdfTextExtractor.GetTextFromPage(reader, i, paragraphReader);    
                */
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);                                  //gets all the text without hidden chars
                    foreach(string line in text.Split("\n").Where(x => !string.IsNullOrEmpty(x.Trim()))){       //splits the text lines
                        TextLine s = new TextLine();
                        foreach(string word in line.Split(" ").Where(x => !string.IsNullOrEmpty(x.Trim()))){ //splits the words
                            s.AddWord(word);
                        }

                        //Avoiding repeated sentences and also the short ones
                        if(!this.Sentences.ContainsKey(s.Text)) 
                            this.Sentences.Add(s.Text, s);
                    }
                }
            }
        }
    }   
}
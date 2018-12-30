/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Comparators.SentenceWordMatch
{
    /// <summary>
    /// This document must be used with the Sentence Word Match Comparator, and stores all the sentences inside a document.
    /// </summary>
    internal class Document: Core.BaseDocument
    {   
        internal class Sentence{
            public int Count {
                get{
                    return this.Words.Count;
                }
            }
            public List<string> Words {get; private set;}
            private Dictionary<string, List<int>> Index {get; set;}
            public string Text{
                get{
                    return string.Format("{0}.", string.Join(" ", this.Words));
                }
            }

            public Sentence(){
                this.Words = new List<string>();
                this.Index = new Dictionary<string, List<int>>();
            }

            public void AddWord(string word){
                if(!this.Index.ContainsKey(word.ToLower())) this.Index.Add(word.ToLower(), new List<int>());
                this.Index[word.ToLower()].Add(this.Count);
                this.Words.Add(word);
            }

            public bool ContainsWord(string word){
                return this.Index.ContainsKey(word.ToLower());
            }

            public List<int> GetIndex(string word){
                return this.Index[word.ToLower()];
            }
        }

        /// <summary>
        /// Contains the phrases inside the document.
        /// </summary>
        /// <value></value>
        public Dictionary<string, Sentence> Sentences {get; set;}        

        /// <summary>
        /// Loads the content of a PDF file and gets all the phrases inside it.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new Exceptions.FileExtensionNotAllowed();

            //Init object attributes.
            Sentences = new Dictionary<string, Sentence>();            

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
                    string text = CleanText(PdfTextExtractor.GetTextFromPage(reader, i));                       //gets all the text without new lines and hidden chars
                    foreach(string sentence in text.Split(".").Where(x => !string.IsNullOrEmpty(x.Trim()))){    //splits the sentences
                        Sentence s = new Sentence();
                        foreach(string word in sentence.Split(" ").Where(x => !string.IsNullOrEmpty(x.Trim()))){ //splits the words
                            s.AddWord(word);
                        }

                        //Avoiding repeated sentences and also the short ones
                        if(!this.Sentences.ContainsKey(s.Text)) 
                            this.Sentences.Add(s.Text, s);
                    }
                }
            }
        }

        protected string CleanText(string text){            
            return new string(text.Replace("\n", " ").Trim().Where(c =>  char.IsLetterOrDigit(c) || c.Equals('’') || (c >= ' ' && c <= byte.MaxValue)).ToArray());
        } 
    }   
}
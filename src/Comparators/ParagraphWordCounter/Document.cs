/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Comparators.ParagraphWordCounter
{
    /// <summary>
    /// This document must be used with the Paragraph Counter Comparator, and stores how many words and how many times appears withing each paragraph inside a document.
    /// </summary>
    internal class Document: Core.BaseDocument
    {        
        /// <summary>
        /// Contains the paragraphs (key) and the appearances of each word inside it (value).
        /// </summary>
        /// <value></value>
        public Dictionary<string, Dictionary<string, int>> Paragraphs {get; set;}
        

        /// <summary>
        /// Loads the content of a PDF file and counts how many words and how many times appears on each paragraph within the document.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new FileNotPdfException();

            //Init object attributes.
            Paragraphs = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> words = null;

            //Read PDF file and sotre each word appearence inside its paragraph.
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    
                    foreach(string paragraph in text.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))){                                                
                        //TODO: settings file in order to exclude a set of words.
                                                   
                        words = new Dictionary<string, int>();
                        foreach(string word in paragraph.Split(" ").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))){
                             if(!words.ContainsKey(word))
                                words.Add(word, 0);
                                        
                            words[word]++;         
                        }

                        if(!Paragraphs.ContainsKey(paragraph)) Paragraphs.Add(paragraph, words);                                    
                        else
                        {
                            //Hash collision or repeater paragraph... can be ignored.
                        }
                    }
                }
            }
        }
    }
}
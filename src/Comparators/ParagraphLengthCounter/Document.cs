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
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparators.ParagraphLengthCounter
{
    /// <summary>
    /// This document must be used with the Paragraph Length Counter Comparator, and stores the length (amount of words) of any paragraph along the document.
    /// </summary>
    internal class Document: Core.BaseDocument
    {        
        /// <summary>
        /// Contains the paragraphs (key) and the number of words inside it (value).
        /// </summary>
        /// <value></value>
        public Dictionary<string, int> Lengths {get; set;}
        

        /// <summary>
        /// Loads the content of a PDF file and counts how many words and how many times appears on each paragraph within the document.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new FileExtensionNotAllowed();

            //Init object attributes.
            Lengths = new Dictionary<string, int>();

            //Read PDF file and sotre each word appearence inside its paragraph.
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);                    
                    foreach(string paragraph in text.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))){                                                                                                                                                   
                        if(!Lengths.ContainsKey(paragraph)) Lengths.Add(paragraph, paragraph.Split(" ").Select(x => x.Trim()).Count());                                    
                        else
                        {
                            //Repeated paragraph... can be ignored.
                        }
                    }
                }
            }
        }
    }
}
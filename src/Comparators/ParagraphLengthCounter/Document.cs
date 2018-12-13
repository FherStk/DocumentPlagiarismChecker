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
        /// Contains how many paragraphs (value) has an specific length (key)
        /// </summary>
        /// <value></value>
        public Dictionary<float, int> Lengths {get; set;}        

        /// <summary>
        /// Loads the content of a PDF file and counts the length of the paragraphs.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Document(string path): base(path){
            //Check pre-conditions        
            if(!System.IO.Path.GetExtension(path).ToLower().Equals(".pdf"))
                throw new FileExtensionNotAllowed();

            //Init object attributes.
            Lengths = new Dictionary<float, int>();

            //Read PDF file and sotre each word appearence inside its paragraph.
            using (PdfReader reader = new PdfReader(path))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);                    
                    foreach(string paragraph in text.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x))){
                        float length = paragraph.Length;
                        if(!Lengths.ContainsKey(length)) Lengths.Add(length, 0);
                        Lengths[length] += 1;                        
                    }
                }
            }
        }
    }
}
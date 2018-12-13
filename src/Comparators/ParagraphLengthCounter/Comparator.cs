/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparators.ParagraphLengthCounter
{
    /// <summary>
    /// The Paragraph Length Counter Comparator reads a pair of files and counts how many words (length) appears on each paragraph within a file, and 
    /// then calculates how many of those lengths matches between documents. So, two documents containing paragraphs with the same length and can be a 
    /// copy with a high level of provability.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Creates a new instance for the Comparator.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <returns></returns>
        public Comparator(string fileLeftPath, string fileRightPath, string sampleFilePath=null): base(fileLeftPath, fileRightPath, sampleFilePath){
        }  
        
        /// <summary>
        /// Counts how many words and how many times appears within each paragraph in a document, and checks the matching percentage.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){      
            //Counting the words appearences for each document (left and right).
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string paragraph in this.Left.Lengths.Select(x => x.Key)){
                if(!counter.ContainsKey(paragraph)) counter.Add(paragraph, new int[]{0, 0});
                counter[paragraph][0] += Left.Lengths[paragraph];
            }

            foreach(string paragraph in this.Right.Lengths.Select(x => x.Key)){
                if(!counter.ContainsKey(paragraph)) counter.Add(paragraph, new int[]{0, 0});
                counter[paragraph][1] += Right.Lengths[paragraph];
            }

            //Counting sample file word appearences, in order to ignore those from the previous files.
            if(this.Sample != null){
                 foreach(string paragraph in this.Sample.Lengths.Select(x => x.Key)){
                    if(counter.ContainsKey(paragraph)){
                        counter[paragraph][0] = Math.Max(0, counter[paragraph][0] - Sample.Lengths[paragraph]);
                        counter[paragraph][1] = Math.Max(0, counter[paragraph][1] - Sample.Lengths[paragraph]);
                        
                        if(counter[paragraph][0] == 0 && counter[paragraph][1] == 0)
                            counter.Remove(paragraph);
                    }                    
                }
            }

            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Paragraph Length Counter", DisplayLevel.DETAILED);            
            cr.DetailsCaption = new string[] { "Paragraph", "Left length", "Right length", "Matching" };
            cr.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

            //Calculate the matching for each individual word.            
            foreach(string paragraph in counter.Select(x => x.Key)){                
                int left = counter[paragraph][0];
                int right = counter[paragraph][1];                
                float match = (left == 0 || right == 0 ? 0 : (left < right ? (float)left / (float)right : (float)right / (float)left));

                cr.AddMatch(match);
                cr.DetailsData.Add(new object[]{paragraph, left, right, match});                
            }                                    
            
            return cr;
        }
    }   
}
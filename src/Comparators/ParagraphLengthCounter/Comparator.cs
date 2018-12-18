/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;
using DocumentPlagiarismChecker.Scores;

namespace DocumentPlagiarismChecker.Comparators.ParagraphLengthCounter
{
    /// <summary>
    /// The Paragraph Length Counter Comparator reads a pair of files and counts how many words (length) appears on each paragraph within a file, and 
    /// then calculates how many of those lengths matches between documents. So, two documents containing paragraphs with the same length could have been copied
    /// and replaced with synonyms.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Creates a new instance for the Comparator.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <param name="settings">The settings instance that will use the comparator.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Counts how many words and how many times appears within each paragraph in a document, and checks the matching percentage.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){      
            //Counting the words appearences for each document (left and right).
            Dictionary<float, int[]> counter = new Dictionary<float, int[]>();
            foreach(float length in this.Left.Lengths.Select(x => x.Key)){
                if(!counter.ContainsKey(length)) counter.Add(length, new int[]{0, 0});
                counter[length][0] += Left.Lengths[length];
            }

            foreach(float length in this.Right.Lengths.Select(x => x.Key)){
                if(!counter.ContainsKey(length)) counter.Add(length, new int[]{0, 0});
                counter[length][1] += Right.Lengths[length];
            }

            //Counting sample file word appearences, in order to ignore those from the previous files.
            if(this.Sample != null){
                 foreach(float length in this.Sample.Lengths.Select(x => x.Key)){
                    if(counter.ContainsKey(length)){
                        counter[length][0] = Math.Max(0, counter[length][0] - Sample.Lengths[length]);
                        counter[length][1] = Math.Max(0, counter[length][1] - Sample.Lengths[length]);
                        
                        if(counter[length][0] == 0 && counter[length][1] == 0)
                            counter.Remove(length);
                    }                    
                }
            }

            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Paragraph Length Counter", DisplayLevel.DETAILED);            
            cr.DetailsCaption = new string[] {"Length", "Left side", "Right side", "Matching" };
            cr.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

            //Calculate the matching for each individual word.            
            foreach(float length in counter.Select(x => x.Key)){                
                int left = counter[length][0];
                int right = counter[length][1];                
                float match = (left == 0 || right == 0 ? 0 : (left < right ? (float)left / (float)right : (float)right / (float)left));

                cr.AddMatch(match);
                cr.DetailsData.Add(new object[]{length, left, right, match});                
            }                                    
            
            return cr;
        }
    }   
}
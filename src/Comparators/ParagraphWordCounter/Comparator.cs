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

namespace DocumentPlagiarismChecker.Comparators.ParagraphWordCounter
{
    /// <summary>
    /// The Word Counter Comparator reads a pair of files and counts how many words and how many times appear on each paragraph within a file, and 
    /// then calculates how many of those appearences matches between documents. So, two documents with the same amount of the same paragraphs and 
    /// words can be a copy with a high level of provability.
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
            ComparatorMatchingScore cr = new ComparatorMatchingScore("Document Word Counter");            
            cr.DetailsCaption = new string[] { "Word", "Count left", "Count right", "Matching" };
            
            //Counting the words and its appearences for each document (left and right) within their paragraphs.                        
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string pLeft in this.Left.Paragraphs.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.Paragraphs[word];
            }

            foreach(string word in this.Right.Paragraphs.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Right.Paragraphs[word];
            }

            //Counting sample file word appearences, in order to ignore those from the previous files.
            if(this.Sample != null){
                 foreach(string word in this.Sample.Paragraphs.Select(x => x.Key)){
                    if(counter.ContainsKey(word)){
                        counter[word][0] = Math.Max(0, counter[word][0] - Sample.Paragraphs[word]);
                        counter[word][1] = Math.Max(0, counter[word][1] - Sample.Paragraphs[word]);
                        
                        if(counter[word][0] == 0 && counter[word][1] == 0)
                            counter.Remove(word);
                    }                    
                }
            }

            //Calculate the matching for each individual word.
            float match = 0;
            foreach(string word in counter.Select(x => x.Key)){                
                int left = counter[word][0];
                int right = counter[word][1];                

                if(left == 0 || right == 0) 
                    match = 0;
                else 
                    match = (left < right ? (float)left / (float)right : (float)right / (float)left);

                cr.AddMatch(match);
                cr.DetailsData.Add(new string[]{word, left.ToString(), right.ToString(), string.Format("{0}%", MathF.Round(match, 2))});                
            }                                    
            
            return cr;
        }        
    }   
}
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
            //In order to improve the performance, all the sample paragraphs will be excluded first from both documents (exact match only).
            if(this.Sample != null){
                foreach(string paragraph in this.Sample.Paragraphs.Select(x => x.Key)){
                    this.Left.Paragraphs.Remove(paragraph);
                    this.Right.Paragraphs.Remove(paragraph);           
                }
            }

            //Counting the words and its appearences for each document (left and right) within their paragraphs.                          
            Dictionary<string, int[]> wordCounter = null;                      
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in this.Left.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in this.Right.Paragraphs.Select(x => x.Key)){                    
                    
                    //Counting the words withing one of the left document's paragraph
                    wordCounter = new Dictionary<string, int[]>();
                    Dictionary<string, int> pLeft = this.Left.Paragraphs[plKey];

                    foreach(string wLeft in pLeft.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wLeft)) wordCounter.Add(wLeft, new int[]{0, 0});
                        wordCounter[wLeft][0] += pLeft[wLeft];
                    }

                    //Counting the words withing one of the right document's paragraph
                    Dictionary<string, int> pRight = this.Right.Paragraphs[prKey];
                    foreach(string wRight in pRight.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wRight)) wordCounter.Add(wRight, new int[]{0, 0});
                        wordCounter[wRight][1] += pRight[wRight];
                    }

                    //Adding the word count to the global paragapg comparisson (the key are a subset of the paragraph in order to show it 
                    //at the input).
                    paragraphCounter.Add(new string[]{ 
                        plKey.Length > 25 ? plKey.Substring(0, 25): plKey, 
                        prKey.Length > 25 ? prKey.Substring(0, 25) : prKey 
                    }, wordCounter);
                }
            }

            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore("Paragraph Word Counter");
            //cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Left length", "Right length", "Word", "Count left", "Count right", "Matching" };
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph"};

            //Calculate the matching for each individual word within each paragraph.
            float match, matchWord, matchLength = 0;
            int leftLengt, rightLength, countLeft, countRight = 0;
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                wordCounter = paragraphCounter[paragraphs];                

                //Matching with paragraph length
                leftLengt = wordCounter.Values.Select(x => x[0]).Where(x => x > 0).Count();
                rightLength = wordCounter.Values.Select(x => x[1]).Where(x => x > 0).Count();

                if(leftLengt == 0 || rightLength == 0)  matchLength = 0;
                else matchLength = (leftLengt < rightLength ? (float)leftLengt / (float)rightLength : (float)rightLength / (float)leftLengt);

                //Counting for each word inside an especific paragraph
                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    countLeft = wordCounter[word][0];
                    countRight = wordCounter[word][1];                

                    //Mathing with word appearences
                    if(countLeft == 0 || countRight == 0)  matchWord = 0;
                    else matchWord = (countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft);                    

                    //Calculate the total match: 50% - 50% (must be tested in order to tweak) and add the info to the detils.
                    match = (matchWord + matchLength) / 2;
                    cr.AddMatch(match);
                    cr.DetailsData.Add(new string[]{paragraphs[0], paragraphs[1]});                

                    //TODO: ADD A NEW DETAILS LEVEL FOR EACH WORD, ETC.
                    //cr.DetailsData.Add(new string[]{paragraphs[0], paragraphs[1], leftLengt.ToString(), rightLength.ToString(), word, countLeft.ToString(), countRight.ToString(), string.Format("{0}%", MathF.Round(match, 2))});                
                }
            }                                    
            
            return cr;
        }        
    }   
}
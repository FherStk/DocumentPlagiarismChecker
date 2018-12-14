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
    /// El Word Counter Comparator llegeix  a pair of files and counts how many words and how many times appear on each paragraph within a file, and 
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
            ExcludeSampleMatches(this.Left);
            ExcludeSampleMatches(this.Right);    
            return ComputeMatching(CompareParagraphs(this.Left, this.Right));                                                        
        }

        /// <summary>
        /// Compares the sample with the given file and exclude the paragraphs that produces a false positive match between the sample an the document.
        /// </summary>
        /// <param name="doc">The document that will be compared with the sample.</param>
        private void ExcludeSampleMatches(Document doc){
             if(this.Sample != null){                
                //In order to improve the performance, all the sample paragraphs will be excluded first from both documents (exact match only).
                foreach(string paragraph in this.Sample.Paragraphs.Select(x => x.Key))
                    doc.Paragraphs.Remove(paragraph);
                                
                int leftLength, rightLength = 0;
                float totalMatch, lengthMatch, wordMath = 0f;                
                ComparatorMatchingScore sampleScore = ComputeMatching(CompareParagraphs(this.Sample, doc));

                for(int i = 0; i < sampleScore.DetailsData.Count; i++){                    
                    leftLength = (int)sampleScore.DetailsData[i][2];
                    rightLength = (int)sampleScore.DetailsData[i][3];
                    lengthMatch = (float)sampleScore.DetailsData[i][4];
                    wordMath = (float)sampleScore.DetailsData[i][5];
                    totalMatch = sampleScore.DetailsMatch[i];    //same as (float)sampleScore.DetailsData[i][6];
                    
                    //TODO: allowing to use totalMatch value or the length + word matches (used to compute the total match).
                    //TODO: testing and tweaking necessary, also config loading from a settings file.                   
                    if(totalMatch >= 0.70f)  doc.Paragraphs.Remove((string)sampleScore.DetailsData[i][1]);                    
                }
             }
        }

        /// <summary>
        /// Counts how many words and how many times appears within each paragraph, comparing them between each other in order to score a matching percentage.
        /// </summary>
        /// <param name="paragraphsLeft">A left-side set of paragraphs as a collection of pair-values following the schema (text, (word, count)).</param>
        /// <param name="paragraphsRight">A right-side set of paragraphs as a collection of pair-values following the schema (text, (word, count)).</param>
        /// <returns>The result of the comparisson as a collection of pair-values following the schema (text[left, right], (word, [countLeft, countRight])</returns>
        private Dictionary<string[], Dictionary<string, int[]>> CompareParagraphs(Document leftDoc, Document rightDoc){
            Dictionary<string, int[]> wordCounter = null;   
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    //Comptant les paraues amb un dels paràgrafs del document esquerre
                    wordCounter = new Dictionary<string, int[]>();
                    Dictionary<string, int> pLeft = leftDoc.Paragraphs[plKey];

                    foreach(string wLeft in pLeft.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wLeft)) wordCounter.Add(wLeft, new int[]{0, 0});
                        wordCounter[wLeft][0] += pLeft[wLeft];
                    }
                    //Comptant les paraules amb un dels documents del paragraf dret
                    Dictionary<string, int> pRight = rightDoc.Paragraphs[prKey];
                    foreach(string wRight in pRight.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wRight)) wordCounter.Add(wRight, new int[]{0, 0});
                        wordCounter[wRight][1] += pRight[wRight];
                    }

                   //  Addició del recompte de paraules a la comparació de paràgrafs globals (la clau és un subconjunt del paràgraf per mostrar-lo
                    paragraphCounter.Add(new string[]{ plKey, prKey }, wordCounter);
                }
            }

            return paragraphCounter;
        }   

        private  ComparatorMatchingScore ComputeMatching(Dictionary<string[], Dictionary<string, int[]>> paragraphCounter){
            //Definició dels encapçalaments de resultats
            ComparatorMatchingScore cr = new ComparatorMatchingScore("Paragraph Word Counter");            
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Left legth", "Right length", "Length match", "Word match", "Total match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0}", "{0}", "{0:P2}", "{0:P2}", "{0:P2}"};
            
            //Calculem la concordança per a cada paraula individual dins de cada paràgraf
            float match, matchWord, matchLength = 0;
            int leftLengt, rightLength, countLeft, countRight = 0;
            Dictionary<string, int[]> wordCounter = null;                      
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                wordCounter = paragraphCounter[paragraphs];                

                //Coincidència amb la longitud del paràgraf
                leftLengt = wordCounter.Values.Select(x => x[0]).Where(x => x > 0).Count();
                rightLength = wordCounter.Values.Select(x => x[1]).Where(x => x > 0).Count();

                if(leftLengt == 0 || rightLength == 0)  matchLength = 0;
                else matchLength = (leftLengt < rightLength ? (float)leftLengt / (float)rightLength : (float)rightLength / (float)leftLengt);                

                //Comptant per a cada paraula dins d'un paràgraf específic
                cr.Child = new DetailsMatchingScore();
                cr.Child.DetailsCaption = new string[]{"Word", "Left count", "Right count", "Match"};
                cr.Child.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    countLeft = wordCounter[word][0];
                    countRight = wordCounter[word][1];                

                    //Matemàtica amb aplicacions de paraules
                    if(countLeft == 0 || countRight == 0)  matchWord = 0;
                    else matchWord = (countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft);                                        

                    //Afegint els detalls de cada paraula      
                    cr.Child.AddMatch(matchWord);                                        
                    cr.Child.DetailsData.Add(new object[]{word, countLeft, countRight, matchWord});                
                }

                //Si s'afegeixen els detalls de cada paràgraf, la coincidència total és: 75% per a les paraules - 25% per a la durada (s'ha de provar per tal de modificar) i afegir la informació als detalls.                    
                match = (cr.Child.Matching*0.75f + matchLength*0.25f);
                cr.AddMatch(match);                
                cr.DetailsData.Add(new object[]{paragraphs[0], paragraphs[1], leftLengt, rightLength, matchLength, cr.Child.Matching, match});
            }

            return cr; 
        }
    }   
}
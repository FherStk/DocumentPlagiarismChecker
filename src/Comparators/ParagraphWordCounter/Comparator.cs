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
    /// The Word Counter Comparator llegeix un parell de fitxers i compte quantes paraules i quantes vegades apareixen a cada paràgraf d'un fitxer i
    ///  a continuació, calcula quants d'aquests appearences coincideixen entre documents. Per tant, dos documents amb la mateixa quantitat dels mateixos paràgrafs i
    ///  Les paraules poden ser una còpia amb un alt nivell de probabilitat.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Crea una nova instancia per al comparador.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <returns></returns>
        public Comparator(string fileLeftPath, string fileRightPath, string sampleFilePath=null): base(fileLeftPath, fileRightPath, sampleFilePath){
        }  
        
        /// <summary>
        /// 
        /// Compta quantes paraules i quantes vegades apareix a cada paràgraf d'un document i comprova el percentatge que coincideix.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){      
            ExcludeSampleMatches(this.Left);
            ExcludeSampleMatches(this.Right);    
            return ComputeMatching(CompareParagraphs(this.Left, this.Right));                                                        
        }

        /// <summary>
        /// Compara la mostra amb el fitxer especificat i exclou els paràgrafs que produeixen una coincidència falsa positiva entre la mostra i el document.
        /// </summary>
        /// <param name="doc">The document that will be compared with the sample.</param>
        private void ExcludeSampleMatches(Document doc){
             if(this.Sample != null){                
                // Per millorar el rendiment, tots els paràgrafs d'exemple s'exclouran en primer lloc dels dos documents (només coincidència exacta).
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
                    totalMatch = sampleScore.DetailsMatch[i];    //igual que (float)sampleScore.DetailsData[i][6];
                    
                    // TODO: permetent utilitzar el valor de total de missatges o la longitud + coincidències de paraules (que es fan servir per calcular la coincidència total)
                   // TODO: es necessiten proves i ajustaments, també es carrega la configuració des d'un fitxer de configuració.                  
                    if(totalMatch >= 0.70f)  doc.Paragraphs.Remove((string)sampleScore.DetailsData[i][1]);                    
                }
             }
        }

        /// <summary>
       /// Compta quantes paraules i quantes vegades apareix dins de cada paràgraf, comparant-les entre elles per tal d'obtenir un percentatge de coincidència.
        /// </summary>
        /// <param name="paragraphsLeft">A left-side set of paragraphs as a collection of pair-values following the schema (text, (word, count)).</param>
        /// <param name="paragraphsRight">A right-side set of paragraphs as a collection of pair-values following the schema (text, (word, count)).</param>
        /// <returns>The result of the comparisson as a collection of pair-values following the schema (text[left, right], (word, [countLeft, countRight])</returns>
        private Dictionary<string[], Dictionary<string, int[]>> CompareParagraphs(Document leftDoc, Document rightDoc){
            Dictionary<string, int[]> wordCounter = null;   
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    //Counting the words withing one of the left document's paragraph
                    wordCounter = new Dictionary<string, int[]>();
                    Dictionary<string, int> pLeft = leftDoc.Paragraphs[plKey];

                    foreach(string wLeft in pLeft.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wLeft)) wordCounter.Add(wLeft, new int[]{0, 0});
                        wordCounter[wLeft][0] += pLeft[wLeft];
                    }

                    //Counting the words withing one of the right document's paragraph
                    Dictionary<string, int> pRight = rightDoc.Paragraphs[prKey];
                    foreach(string wRight in pRight.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wRight)) wordCounter.Add(wRight, new int[]{0, 0});
                        wordCounter[wRight][1] += pRight[wRight];
                    }

                    //Adding the word count to the global paragapg comparisson (the key are a subset of the paragraph in order to show it 
                    //at the input).
                    paragraphCounter.Add(new string[]{ plKey, prKey }, wordCounter);
                }
            }

            return paragraphCounter;
        }   

        private  ComparatorMatchingScore ComputeMatching(Dictionary<string[], Dictionary<string, int[]>> paragraphCounter){
            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore("Paragraph Word Counter");            
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Left legth", "Right length", "Length match", "Word match", "Total match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0}", "{0}", "{0:P2}", "{0:P2}", "{0:P2}"};
            
            //Calculate the matching for each individual word within each paragraph.
            float match, matchWord, matchLength = 0;
            int leftLengt, rightLength, countLeft, countRight = 0;
            Dictionary<string, int[]> wordCounter = null;                      
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                wordCounter = paragraphCounter[paragraphs];                

                //Matching with paragraph length
                leftLengt = wordCounter.Values.Select(x => x[0]).Where(x => x > 0).Count();
                rightLength = wordCounter.Values.Select(x => x[1]).Where(x => x > 0).Count();

                if(leftLengt == 0 || rightLength == 0)  matchLength = 0;
                else matchLength = (leftLengt < rightLength ? (float)leftLengt / (float)rightLength : (float)rightLength / (float)leftLengt);                

                //Counting for each word inside an especific paragraph
                cr.Child = new DetailsMatchingScore();
                cr.Child.DetailsCaption = new string[]{"Word", "Left count", "Right count", "Match"};
                cr.Child.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    countLeft = wordCounter[word][0];
                    countRight = wordCounter[word][1];                

                    //Mathing with word appearences
                    if(countLeft == 0 || countRight == 0)  matchWord = 0;
                    else matchWord = (countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft);                                        

                    //Adding the details for each word                    
                    cr.Child.AddMatch(matchWord);                                        
                    cr.Child.DetailsData.Add(new object[]{word, countLeft, countRight, matchWord});                
                }

                //Adding the details for each paragraph, the total match is: 75% for words - 25% for length (must be tested in order to tweak) and add the info to the detils.                    
                match = (cr.Child.Matching*0.75f + matchLength*0.25f);
                cr.AddMatch(match);                
                cr.DetailsData.Add(new object[]{paragraphs[0], paragraphs[1], leftLengt, rightLength, matchLength, cr.Child.Matching, match});
            }

            return cr; 
        }
    }   
}
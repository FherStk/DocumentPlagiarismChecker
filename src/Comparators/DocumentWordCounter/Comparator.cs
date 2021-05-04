/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DocumentPlagiarismChecker.Core;
using DocumentPlagiarismChecker.Scores;

namespace DocumentPlagiarismChecker.Comparators.ParagraphWordCounter
{
    /// <summary>
    /// The Paragraph Word Counter Comparator reads a pair of files and counts how many words and how many times appear on each paragraph within a file, and 
    /// then calculates how many of those appearences matches between documents. So, two documents with the same amount of the same paragraphs and 
    /// words can be a copy with a high level of provability.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Crea una nueva instancia para Comparator.
        /// </summary>
        /// <param name="fileLeftPath">La ruta del archivo del lado izquierdo.</param>
        /// <param name="fileRightPath">La ruta del archivo del lado derecho.</param>
        /// <param name="settings">La instancia de configuración que utilizará el comparador.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Cuenta cuántas palabras y cuántas veces aparecen en cada párrafo de un documento y verifica el porcentaje de coincidencia.
        /// </summary>
        /// <returns>Los resultados que coincidan.</returns>
        public override ComparatorMatchingScore Run(){     
<<<<<<< HEAD
            //Este pedido está destinado a mejorar el rendimiento.
            ExcludeSampleExactMatches(); 
            ExcludeSamplePartialMatches(this.Left, 0.70f);  //TODO: el valor de umbral debe obtenerse de la configuración; comprobar si se puede quitar
            ExcludeSamplePartialMatches(this.Right, 0.70f);  //TODO: el valor de umbral debe obtenerse de la configuración; comprobar si se puede quitar
=======
            //Esta 
            ExcludeSampleExactMatches(); 
            ExcludeSamplePartialMatches(this.Left, 0.70f);  //TODO: El valor de limite debe recibirse de la configuración, comprobar si se puede eliminar.
            ExcludeSamplePartialMatches(this.Right, 0.70f);  //TODO: El valor de limite debe recibirse de la configuración, comprobar si se puede eliminar.
>>>>>>> 8d0ac61bb202ff934bc6c0f54b95980ed7883a4d
            ExcludeExclussionListMatches();
            
            return ComputeMatching(CompareParagraphs(this.Left, this.Right));                                                        
        }

        private void ExcludeExclussionListMatches(){
            if(this.Settings.Exclusion == null) return;

            foreach(string pattern in this.Settings.Exclusion){
                foreach(string paragraph in this.Left.Paragraphs.Select(x => x.Key).ToList()){
                    if(Regex.IsMatch(paragraph, pattern)) 
                        this.Left.Paragraphs.Remove(paragraph);   
                }

                foreach(string paragraph in this.Right.Paragraphs.Select(x => x.Key).ToList()){
                    if(Regex.IsMatch(paragraph, pattern)) 
                        this.Right.Paragraphs.Remove(paragraph);   
                }                    
            }
        }

        /// <summary>
        /// Compara la muestra con el archivo y excluye los parrafos que producen un falso positivo.
        /// </summary>
        private void ExcludeSampleExactMatches(){
            if(this.Sample == null) return;

            foreach(string paragraph in this.Sample.Paragraphs.Select(x => x.Key)){
                this.Left.Paragraphs.Remove(paragraph);   
                this.Right.Paragraphs.Remove(paragraph);   
            }   
        }

        private void ExcludeSamplePartialMatches(Document doc, float threshold){
            if(this.Sample == null) return;

            ComparatorMatchingScore sampleScore = ComputeMatching(CompareParagraphs(this.Sample, doc));
            for(int i = 0; i < sampleScore.DetailsData.Count; i++){                                                            
                if(sampleScore.DetailsMatch[i] >= threshold){
                    doc.Paragraphs.Remove((string)sampleScore.DetailsData[i][1]);                    
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
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    //Counting the words withing one of the left document's paragraph
                    Dictionary<string, int[]> wordCounter = new Dictionary<string, int[]>();
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
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Paragraph Word Counter", DisplayLevel.DETAILED);
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0:P2}"};
            
            //Calculate the matching for each individual word within each paragraph.
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                Dictionary<string, int[]> wordCounter = paragraphCounter[paragraphs];                                

                //Counting for each word inside an especific paragraph
                cr.Child = new DetailsMatchingScore();
                cr.Child.DetailsCaption = new string[]{"Word", "Left count", "Right count", "Match"};
                cr.Child.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    int countLeft = wordCounter[word][0];
                    int countRight = wordCounter[word][1];

                    //Mathing with word appearences
                    float match = (countLeft == 0 || countRight == 0 ? 0 :(countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft));                    

                    //Adding the details for each word                         
                    cr.Child.AddMatch(match);
                    cr.Child.DetailsData.Add(new object[]{word, countLeft, countRight, match});
                }
                
                //Adding the details for each paragraph
                cr.AddMatch(cr.Child.Matching);
                cr.DetailsData.Add(new object[]{paragraphs[0], paragraphs[1], cr.Child.Matching});
            }

            return cr; 
        }
    }   
}
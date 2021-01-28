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
    /// El comparador de contador de palabras de párrafo lee un par de archivos y cuenta cuántas palabras y cuántas veces aparecen en cada párrafo dentro de un archivo, y
    /// luego calcula cuántas de esas apariciones coinciden entre documentos. Entonces, dos documentos con la misma cantidad de los mismos párrafos y
    /// las palabras pueden ser una copia con un alto nivel de demostrabilidad.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Crea una nueva instancia para el comparador.
        /// </summary>
        /// <param name="fileLeftPath">La ruta del archivo del lado izquierdo</param>
        /// <param name="fileRightPath">La ruta del archivo del lado derecho</param>
        /// <param name="settings">La instancia de configuración que utilizará el comparador.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Compta quantes paraules i quantes vegades apareixen dins de cada paràgraf en un document i comprova el percentatge de coincidència.
        /// </summary>
        /// <returns>Els resultats de la coincidència.</returns>
        public override ComparatorMatchingScore Run(){     
            //This order is meant to improving performance
            ExcludeSampleExactMatches(); 
            ExcludeSamplePartialMatches(this.Left, 0.70f);  //TODO: threshold value must be get from settings; check if can be removed
            ExcludeSamplePartialMatches(this.Right, 0.70f);  //TODO: threshold value must be get from settings; check if can be removed
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
        /// Compares the sample with the given file and exclude the paragraphs that produces a false positive match between the sample an the document.
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
        /// Compta quantes paraules i quantes vegades apareixen a cada paràgraf, comparant-les entre si per obtenir un percentatge coincident.
        /// </summary>
        /// <param name="paragraphsLeft">Un conjunt de paràgrafs de l’esquerra com a col·lecció de parells de valors que segueixen l’esquema (text, (paraula, recompte)).</param>
        /// <param name="paragraphsRight">Un conjunt de paràgrafs de la dreta com a col·lecció de parells de valors que segueixen l’esquema (text, (paraula, recompte)).</param>
        /// <returns>El resultat de la comparació com a col·lecció de parells de valors després de l’esquema (text [esquerra, dreta], (paraula, [countLeft, countRight])</returns>
        private Dictionary<string[], Dictionary<string, int[]>> CompareParagraphs(Document leftDoc, Document rightDoc){
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    //Comptant les paraules d'un dels paràgrafs del document esquerre
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
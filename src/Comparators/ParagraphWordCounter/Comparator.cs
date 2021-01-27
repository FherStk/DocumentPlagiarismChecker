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
    /// El comparador de paràmetres de comptador de paraules llegeix un parell de fitxers i compta quantes paraules i quantes vegades apareixen a cada paràgraf dins d’un fitxer i
    /// llavors calcula quantes d'aquestes aparicions coincideixen entre documents. Per tant, dos documents amb la mateixa quantitat dels mateixos paràgrafs i
    /// les paraules poden ser una còpia amb un alt nivell de probabilitat.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// 
        /// Crea una nova instància per al comparador.
        /// </summary>
        /// <param name="fileLeftPath">El camí del fitxer lateral esquerre</param>
        /// <param name="fileRightPath">El camí del fitxer del costat dret</param>
        /// <param name="settings">La instància de configuració que utilizarà el comparador</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Compta quantes paraules i quantes vegades apareixen dins de cada paràgraf en un document i comprova el percentatge de coincidència.
        /// </summary>
        /// <returns>Els resultats de la coincidència</returns>
        public override ComparatorMatchingScore Run(){     
            // Aquest ordre està destinat a millorar el rendiment
            ExcludeSampleExactMatches(); 
            ExcludeSamplePartialMatches(this.Left, 0.70f);   // TOT: el valor del llindar ha de ser obtenir de la configuració; comproveu si es pot eliminar
            ExcludeSamplePartialMatches(this.Right, 0.70f);  // TOT: el valor del llindar ha de ser obtenir de la configuració; comproveu si es pot eliminar
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
        /// Compara la mostra amb el fitxer donat i exclou els paràgrafs que produeixen una falsa coincidència positiva entre la mostra i el document.
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
        /// Compta quantes paraules i quantes vegades apareixen dins de cada paràgraf, comparant-les entre si per obtenir un percentatge coincident.
        /// </summary>
        /// <param name="paragraphsLeft">Un conjunt de paràgrafs de l’esquerra com a col·lecció de parells de valors que segueixen l’esquema (text, (paraula, recompte)).</param>
        /// <param name="paragraphsRight">Un conjunt de paràgrafs de la dreta com a col·lecció de parells de valors que segueixen l’esquema (text, (paraula, recompte)).</param>
        /// <returns>El resultat de la comparació com a col·lecció de parells de valors després de l’esquema (text [esquerra, dreta], (paraula, [countLeft, countRight])</returns>
        private Dictionary<string[], Dictionary<string, int[]>> CompareParagraphs(Document leftDoc, Document rightDoc){
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    // Comptar les paraules que contenen un dels paràgrafs del document esquerre
                    Dictionary<string, int[]> wordCounter = new Dictionary<string, int[]>();
                    Dictionary<string, int> pLeft = leftDoc.Paragraphs[plKey];
                    foreach(string wLeft in pLeft.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wLeft)) wordCounter.Add(wLeft, new int[]{0, 0});
                        wordCounter[wLeft][0] += pLeft[wLeft];
                    }

                    // Comptar les paraules que contenen un dels paràgrafs del document adequat
                    Dictionary<string, int> pRight = rightDoc.Paragraphs[prKey];
                    foreach(string wRight in pRight.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wRight)) wordCounter.Add(wRight, new int[]{0, 0});
                        wordCounter[wRight][1] += pRight[wRight];
                    }

                    // Afegir el recompte de paraules a la comparació global de paragapg (les claus són un subconjunt del paràgraf per mostrar-lo 
                    // a l'entrada).
                    paragraphCounter.Add(new string[]{ plKey, prKey }, wordCounter);
                }
            }

            return paragraphCounter;
        }   

        private  ComparatorMatchingScore ComputeMatching(Dictionary<string[], Dictionary<string, int[]>> paragraphCounter){
            // Definició de les capçaleres de resultats
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Paragraph Word Counter", DisplayLevel.DETAILED);
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0:P2}"};
            
            // Calculeu la coincidència de cada paraula dins de cada paràgraf.
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                Dictionary<string, int[]> wordCounter = paragraphCounter[paragraphs];                                

                // Comptar per a cada paraula dins d’un paràgraf específic
                cr.Child = new DetailsMatchingScore();
                cr.Child.DetailsCaption = new string[]{"Word", "Left count", "Right count", "Match"};
                cr.Child.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    int countLeft = wordCounter[word][0];
                    int countRight = wordCounter[word][1];

                    // Trencar amb les aparences de paraules
                    float match = (countLeft == 0 || countRight == 0 ? 0 :(countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft));                    

                    // Afegir els detalls de cada paraula                        
                    cr.Child.AddMatch(match);
                    cr.Child.DetailsData.Add(new object[]{word, countLeft, countRight, match});
                }
                
                // Afegir els detalls de cada paràgraf
                cr.AddMatch(cr.Child.Matching);
                cr.DetailsData.Add(new object[]{paragraphs[0], paragraphs[1], cr.Child.Matching});
            }

            return cr; 
        }
    }   
}
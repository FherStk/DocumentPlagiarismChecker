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

namespace DocumentPlagiarismChecker.Comparators.DocumentWordCounter
{
    /// <summary>
    /// The Word Counter Comparator reads a pair of files and counts how many words and how many times appear on each file, and then calculates
    /// how many of those appearences matches between documents. So, two documents with the same amount of the same words can be a copy with
    /// a high level of provability.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Crea una nova instància per al comparador.
        /// </summary>
        /// <param name="fileLeftPath">La ruta del fitxer de la esquerre.</param>
        /// <param name="fileRightPath">La ruta del fitxer de la dreta.</param>
        /// <param name="settings">La instància de configuració que utilitzarà el comparador.</param>
        /// <returns></returns>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Counts how many words and how many times appears within each document, and checks the matching percentage.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){
            
            //compta les vegades que apareixen les paraules per cada document (esquerra i dreta)
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.WordAppearances[word];
            }

            foreach(string word in this.Right.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Right.WordAppearances[word];
            }

            //Compta les paraules dels documents d'exemple, pe tal d'ignorar aquells dels documents anteriors
            if(this.Sample != null){
                 foreach(string word in this.Sample.WordAppearances.Select(x => x.Key)){
                    if(counter.ContainsKey(word)){
                        counter[word][0] = Math.Max(0, counter[word][0] - Sample.WordAppearances[word]);
                        counter[word][1] = Math.Max(0, counter[word][1] - Sample.WordAppearances[word]);
                        
                        if(counter[word][0] == 0 && counter[word][1] == 0)
                            counter.Remove(word);
                    }                    
                }
            }

            
            //Defineix les capceleres resultants
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Document Word Counter", DisplayLevel.FULL);            
            cr.DetailsCaption = new string[] { "Word", "Left count", "Right count", "Match" };
            cr.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

            
            //Calcula la parella per cada paraula individual       
            foreach(string word in counter.Select(x => x.Key)){                
                int left = counter[word][0];
                int right = counter[word][1];                
                float match = (left == 0 || right == 0 ? 0 : (left < right ? (float)left / (float)right : (float)right / (float)left));

                cr.AddMatch(match);
                cr.DetailsData.Add(new object[]{word, left, right, match});                
            }                                    
            
            return cr;
        }        
    }   
}
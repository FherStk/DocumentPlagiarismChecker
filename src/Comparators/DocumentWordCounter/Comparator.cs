/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    Este software está bajo los términos de la licencia pública general de Affero de GNU versión 3.
    Consulte (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) para obtener más información sobre las licencias.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparators.DocumentWordCounter
{
    /// <summary>
    /// El contador de palabras compara un par de archivos y cuenta cuántas palabras y cuántas veces aparecen en cada archivo, y luego calcula
    /// cuántas de esas apariencias coinciden entre documentos. Entonces, dos documentos con la misma cantidad de las mismas palabras pueden ser una copia con
    /// un alto nivel de probabilidad .
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// 
        //Crea una nueva instancia para el comparador.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <returns></returns>
        public Comparator(string fileLeftPath, string fileRightPath, string sampleFilePath=null): base(fileLeftPath, fileRightPath, sampleFilePath){
        }  
        
        /// <summary>
        
/// Cuenta cuántas palabras y cuántas veces aparecen dentro de cada documento, y verifica el porcentaje correspondiente.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){
            // Contando las palabras que aparecen para cada documento (izquierda y derecha)
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.WordAppearances[word];
            }

            foreach(string word in this.Right.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Right.WordAppearances[word];
            }

           
// Contando las apariciones de la palabra del archivo de muestra, para ignorar las de los archivos anteriores
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

            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore("Document Word Counter", DisplayLevel.FULL);            
            cr.DetailsCaption = new string[] { "Word", "Count left", "Count right", "Matching" };
            cr.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

           
            // Definiendo los encabezados de resultados
            float match = 0;
            int left, right = 0;
            foreach(string word in counter.Select(x => x.Key)){                
                left = counter[word][0];
                right = counter[word][1];                

                if(left == 0 || right == 0) match = 0;
                else match = (left < right ? (float)left / (float)right : (float)right / (float)left);

                cr.AddMatch(match);
                cr.DetailsData.Add(new object[]{word, left, right, match});                
            }                                    
            
            return cr;
        }        
    }   
}
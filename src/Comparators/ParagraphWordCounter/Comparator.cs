/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    Este softaware se rige bajo los términos de la versión 3 de la licencia pública GNU Affero General 
	Por favor referirse a (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) para más detalles de la licencia.
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
    ///El Paragraph Word Counter Comparator lee un par de filas y cuanta cuantas palabras hay y cuantas veces aparecen en cada párrafo de un documento 
    /// y luego calcula cuantas de estas apariciones se coinciden entre documetos. Así, dos documentos con la misma cantidad de párrafos y
    /// palabras pueden tener una gran probabilidad de ser una copia.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Crea una nueva instancia para el Comparator.
        /// </summary>
        /// <param name="fileLeftPath">TLa parte izquierda de la ruta del archivo</param>
        /// <param name="fileRightPath">La parte derecha de la ruta del archivo</param>
        /// <param name="settings">La configuración que usará el comparador.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Cuenta cuantas palabras y cuantas veces aparecen en cada párrafo del documento, y comprueba el porcentaje de coincidencia.
        /// </summary>
        /// <returns>Los resultados de la coincidencia.</returns>
        public override ComparatorMatchingScore Run(){     
            //Esta orden sirve para mejorar el rendimiento
            ExcludeSampleExactMatches(); 
            ExcludeSamplePartialMatches(this.Left, 0.70f);  //TODO: el umbral de valores se debe obtener de la configuracion; mirar si se puede eliminar
            ExcludeSamplePartialMatches(this.Right, 0.70f);  //TODO: el umbral de valores se debe obtener de la configuracion; mirar si se puede eliminar
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
        /// Compara el ejemplo con el archivo dado y excluye el parágrafo que produce una coincidencia de falso positivo entre el ejemplo y el documento.
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
        /// Cuenta cuantas palabras y cuantas veces aparecen en cada párrafo, comparandolas entre ellas para obtener un porcentaje que los relacione.
        /// </summary>
        /// <param name="paragraphsLeft">Un set de parágrafos en la parte izquierda como colecciones de valores pares siguendo el esquema(text, (word, count)).</param>
        /// <param name="paragraphsRight">Un set de parágrafos en la parte derecha como colecciones de valores pares siguendo el esquema(text, (word, count)).</param>
        /// <returns>El resultado de la comparativa como colección de los valores pares siguiendo el esquema(text[left, right], (word, [countLeft, countRight])</returns>
        private Dictionary<string[], Dictionary<string, int[]>> CompareParagraphs(Document leftDoc, Document rightDoc){
            Dictionary<string[], Dictionary<string, int[]>> paragraphCounter = new Dictionary<string[], Dictionary<string, int[]>>();            
            foreach(string plKey in leftDoc.Paragraphs.Select(x => x.Key)){                
                foreach(string prKey in rightDoc.Paragraphs.Select(x => x.Key)){                                        

                    //Contando las palabras en uno de los párrafos izquierdos del documentos
                    Dictionary<string, int[]> wordCounter = new Dictionary<string, int[]>();
                    Dictionary<string, int> pLeft = leftDoc.Paragraphs[plKey];
                    foreach(string wLeft in pLeft.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wLeft)) wordCounter.Add(wLeft, new int[]{0, 0});
                        wordCounter[wLeft][0] += pLeft[wLeft];
                    }

                    //Contando las palabras en uno de los párrafos derechos del documentos
                    Dictionary<string, int> pRight = rightDoc.Paragraphs[prKey];
                    foreach(string wRight in pRight.Select(x => x.Key)){
                        if(!wordCounter.ContainsKey(wRight)) wordCounter.Add(wRight, new int[]{0, 0});
                        wordCounter[wRight][1] += pRight[wRight];
                    }

                    //Añadiendo el conteo de palabras a la comparación del paragrafo global(la clave es un subset del parágrafo pata mostrarlo
                    //en el input).
                    paragraphCounter.Add(new string[]{ plKey, prKey }, wordCounter);
                }
            }

            return paragraphCounter;
        }   

        private  ComparatorMatchingScore ComputeMatching(Dictionary<string[], Dictionary<string, int[]>> paragraphCounter){
            //Definiendo los resultados de la cabecera
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Paragraph Word Counter", DisplayLevel.DETAILED);
            cr.DetailsCaption = new string[] { "Left paragraph", "Right paragraph", "Match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0:P2}"};
            
            //Calcular las igualdades para cada palabra individual de cada paragrafo
            foreach(string[] paragraphs in paragraphCounter.Select(x => x.Key)){    
                Dictionary<string, int[]> wordCounter = paragraphCounter[paragraphs];                                

                //Contando cada palabra dentro de un parágrafo específico
                cr.Child = new DetailsMatchingScore();
                cr.Child.DetailsCaption = new string[]{"Word", "Left count", "Right count", "Match"};
                cr.Child.DetailsFormat = new string[]{"{0}", "{0}", "{0}", "{0:P2}"};

                foreach(string word in wordCounter.Select(x => x.Key)){                                
                    int countLeft = wordCounter[word][0];
                    int countRight = wordCounter[word][1];

                    //Coincidencia con las apariencias de las palabras
                    float match = (countLeft == 0 || countRight == 0 ? 0 :(countLeft < countRight ? (float)countLeft / (float)countRight : (float)countRight / (float)countLeft));                    

                    //Añadiendo los detalles para cada palabra                         
                    cr.Child.AddMatch(match);
                    cr.Child.DetailsData.Add(new object[]{word, countLeft, countRight, match});
                }
                
                //Añadiendo los detalles para cada párrafo
                cr.AddMatch(cr.Child.Matching);
                cr.DetailsData.Add(new object[]{paragraphs[0], paragraphs[1], cr.Child.Matching});
            }

            return cr; 
        }
    }   
}
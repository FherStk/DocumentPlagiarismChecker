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

namespace DocumentPlagiarismChecker.Comparators.SentenceWordMatch
{
    /// <summary>
    /// The Paragraph Word Counter Comparator reads a pair of files and counts how many words and how many times appear on each paragraph within a file, and 
    /// then calculates how many of those appearences matches between documents. So, two documents with the same amount of the same paragraphs and 
    /// words can be a copy with a high level of provability.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparator: Core.BaseComparator<Document>
    { 
        private class Sentence{
            public string Left {get; set;}
            public string Right {get; set;}
            public List<Word> Words {get; set;}

            public int Length {
                get{
                    return this.Words.Count;
                }
            }

            public float Match{
                get{
                    return (float)this.Words.Count() / (float)this.Left.Split(" ").Count();
                }
            }

            public bool Exact{
                get{
                    return this.Match == 1 && this.Right == this.Left;
                }                
            }
        }
        private class Word{
            public string Left {get; set;}
            public string Right {get; set;}
        } 
        /// <summary>
        /// Creates a new instance for the Comparator.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <param name="settings">The settings instance that will use the comparator.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
        }  
        
        /// <summary>
        /// Counts how many sentences appears on both documents over a matching threshold percentage.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparatorMatchingScore Run(){     
            //Excluding sample data; this order is meant to improving performance
            ExcludeSampleExactMatches();             
            ExcludeExclussionListMatches();
            
            foreach(Sentence s in CompareDocuments(this.Sample, this.Left))
                if(s.Exact || (s.Length > 5 && s.Match > 0.75f)) this.Left.Sentences.Remove(s.Left);
                        
            foreach(Sentence s in CompareDocuments(this.Sample, this.Right))
                if(s.Exact || (s.Length > 5 && s.Match > 0.75f)) this.Right.Sentences.Remove(s.Right);
            
            return ComputeMatching(CompareDocuments(this.Left, this.Right));                
        }

        private void ExcludeExclussionListMatches(){
            if(this.Settings.Exclusion == null) return;

            foreach(string pattern in this.Settings.Exclusion){
                foreach(string paragraph in this.Left.Sentences.Select(x => x.Key).ToList()){
                    if(Regex.IsMatch(paragraph, pattern)) 
                        this.Left.Sentences.Remove(paragraph);   
                }

                foreach(string paragraph in this.Right.Sentences.Select(x => x.Key).ToList()){
                    if(Regex.IsMatch(paragraph, pattern)) 
                        this.Right.Sentences.Remove(paragraph);   
                }                    
            }
        }

        /// <summary>
        /// Compares the sample with the given file and exclude the sentences that produces a false positive match between the sample an the document.
        /// </summary>
        private void ExcludeSampleExactMatches(){
            if(this.Sample == null) return;

            foreach(string sentence in this.Sample.Sentences.Select(x => x.Key)){
                this.Left.Sentences.Remove(sentence);   
                this.Right.Sentences.Remove(sentence);   
            }   
        }       
       
        private List<Sentence> CompareDocuments(Document leftDoc, Document rightDoc){
            List<Sentence> sentences = new List<Sentence>();
            foreach(string lKey in leftDoc.Sentences.Select(x => x.Key)){     
                List<Sentence> current = new List<Sentence>();           
                foreach(string rKey in rightDoc.Sentences.Select(x => x.Key)){                                        
                    
                    //Comparing both sentences (left and right)                                                       
                    Document.TextLine ls = leftDoc.Sentences[lKey];
                    for(int k = 0; k < ls.Words.Count; k++){
                        string wLeft = ls.Words[k];

                        Document.TextLine rs = rightDoc.Sentences[rKey];
                        if(!rs.ContainsWord(wLeft)){
                            //This will never match
                            current.Add(new Sentence(){
                                Left = ls.Text,
                                Right = rs.Text,
                                Words = new List<Word>()
                            });
                        }
                        else{
                            //The right side contains the word on the left one (at least one time) so some match will be produced
                            foreach(int idx in rs.GetIndex(wLeft)){
                                
                                int i = k; 
                                List<Word> word = new List<Word>();                                                                      
                                for(int j = idx; j < rs.Words.Count && i < ls.Words.Count; j++){       
                                    if(ls.Words[i].Equals(rs.Words[j], StringComparison.CurrentCultureIgnoreCase)){
                                        word.Add(new Word(){
                                            Left = ls.Words[i],
                                            Right = rs.Words[j]
                                        });
                                    }

                                    k++;
                                    i++;
                                }

                                current.Add(new Sentence(){
                                    Left = ls.Text,
                                    Right = rs.Text,
                                    Words = word
                                });
                            }
                        }    
                    }
                }

                //On current we have the results of all the comparissons between the current left sentence and all the right ones.
                //The higher match will be selected.
                float max = current.Max(x => x.Match);
                sentences.Add(current.Where(x => x.Match == max).FirstOrDefault());
            }

            return sentences;
        }   

        private  ComparatorMatchingScore ComputeMatching(List<Sentence> sentences){            
            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Sentence Word Match", DisplayLevel.DETAILED);
            cr.DetailsCaption = new string[] { "Left sentence (subset)", "Right sentence (original)", "Match"};
            cr.DetailsFormat = new string[]{"{0:L75}", "{0:L75}", "{0:P2}"}; //Note: use {0:L50} in order to show a substring of max length = 50
            
            //Calculate the matching for each individual word within each paragraph.
            foreach(Sentence sentence in sentences){                    
                cr.AddMatch(sentence.Match);
                cr.DetailsData.Add(new object[]{sentence.Left, sentence.Right, sentence.Match});
            }

            return cr; 
        }
    }   
}
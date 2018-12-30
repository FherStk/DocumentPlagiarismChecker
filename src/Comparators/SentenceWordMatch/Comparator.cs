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
        private class Item{
            public string LeftWord {get; set;}
            public string RightWord {get; set;}
            public bool Match {get; set;}
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
            //This order is meant to improving performance
            ExcludeSampleExactMatches(); 
            ExcludeExclussionListMatches();
                        
            return ComputeMatching(CompareParagraphs(this.Left, this.Right));                
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

            foreach(string paragraph in this.Sample.Sentences.Select(x => x.Key)){
                this.Left.Sentences.Remove(paragraph);   
                this.Right.Sentences.Remove(paragraph);   
            }   
        }       
       
        private List<List<Item>> CompareParagraphs(Document leftDoc, Document rightDoc){
            List<List<Item>> sentences = new List<List<Item>>();                              
            foreach(string lKey in leftDoc.Sentences.Select(x => x.Key)){                
                foreach(string rKey in rightDoc.Sentences.Select(x => x.Key)){                                        
                    
                    //Comparing both sentences (left and right)                                                       
                    Document.Sentence ls = leftDoc.Sentences[lKey];
                    for(int k = 0; k < ls.Words.Count; k++){
                        string wLeft = ls.Words[k];

                        Document.Sentence rs = rightDoc.Sentences[rKey];
                        if(rs.ContainsWord(wLeft)){

                            //The right side contains the word on the left one (at least one time)
                            foreach(int idx in rs.GetIndex(wLeft)){
                                
                                int i = k;                                
                                List<Item> current = new List<Item>();                                                                      
                                for(int j = idx; j < rs.Words.Count && i < ls.Words.Count; j++){                                    
                                    current.Add(new Item(){
                                        LeftWord = ls.Words[i],
                                        RightWord = rs.Words[j],
                                        Match = ls.Words[i].Equals(rs.Words[j], StringComparison.CurrentCultureIgnoreCase)
                                    });
                                    i++;
                                }

                                //sentence compared
                                sentences.Add(current);
                            }
                        }    
                    }
                }
            }

            return sentences;
        }   

        private  ComparatorMatchingScore ComputeMatching(List<List<Item>> sentences){
            //Defining the results headers
            ComparatorMatchingScore cr = new ComparatorMatchingScore(this.Left.Name, this.Right.Name, "Sentence Word Match", DisplayLevel.DETAILED);
            cr.DetailsCaption = new string[] { "Left sentence", "Right sentence", "Match"};
            cr.DetailsFormat = new string[]{"{0:L50}", "{0:L50}", "{0:P2}"};
            
            //Calculate the matching for each individual word within each paragraph.
            foreach(List<Item> sentence in sentences){                    
                float match = (float)sentence.Where(x => x.Match).Count() / (float)sentence.Count;

                //TODO: use the settings threshold
                if(match > 0.5f && sentence.Count > 5){
                    string leftSentence = string.Join(" ", sentence.Select(x => x.LeftWord));
                    string rightSentence = string.Join(" ", sentence.Select(x => x.RightWord));                               

                    //Adding the details for each paragraph
                    cr.AddMatch(match);
                    cr.DetailsData.Add(new object[]{leftSentence, rightSentence, match});
                }                
            }

            return cr; 
        }
    }   
}
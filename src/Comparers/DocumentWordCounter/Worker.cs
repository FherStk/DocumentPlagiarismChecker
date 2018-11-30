using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparers.WordCounter
{
    internal class Worker: Core.BaseWorker<Document>
    {  
        internal Worker(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
        

        /// <summary>
        /// Counts how many words and how many times appears within each document, and checks the matching percentage.
        /// </summary>
        /// <returns></returns>
        public override void Run(){            
            this.ResultComparer.DetailsCaption = new string[] { "Word", "Count left", "Count right", "Matching" };
            
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.WordAppearances[word];
            }

            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Left.WordAppearances[word];
            }

            //Calculate the matching for each individual word.
            float match = 0;
            foreach(string word in counter.Select(x => x.Key)){                
                int left = counter[word][0];
                int right = counter[word][1];                

                if(left != 0 && right != 0){
                    match = (left < right ? (float)left / (float)right : (float)right / (float)left);
                }

                this.ResultComparer.Matching = match;
                this.ResultComparer.DetailsData.Add(new string[]{word, left.ToString(), right.ToString(), string.Format("{0}%", MathF.Round(match, 2))});
            }
            

            
            //TODO: each comparer must own its own ResultHeader and add it to a global Result that will be sent to print.
            //this is just a dirty hard-code patch for transition and test.            
            /*foreach(var wLeft in this.Left.Words)
                this.ResultHeader.AddLeft(wLeft.Text, wLeft.Count);

            foreach(var wRight in this.Right.Words)
                this.ResultHeader.AddRight(wRight.Text, wRight.Count);
                */
        }        
    }   
}
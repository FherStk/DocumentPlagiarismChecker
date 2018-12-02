/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Outputs
{
    /// <summary>
    /// This output base object sends the results to the terminal.
    /// </summary>
    internal class Terminal: Core.BaseOutput{
        /// <summary>
        /// Writes the given set of results into the terminal.
        /// </summary>
        /// <param name="results">A set of results regarding each compared pair of files.</param>
        /// <param name="level">The output details level.</param>
        public override void Write(List<FileMatchingScore> results, OutputLevel level = OutputLevel.BASIC){            
            foreach(FileMatchingScore fpr in results){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Left file: {0}", fpr.LeftFileName);
                System.Console.WriteLine("Right file: {0}", fpr.RightFileName);
                System.Console.WriteLine("Matching: {0}%", System.Math.Round(fpr.Matching*100, 2));

                foreach(ComparatorMatchingScore rc in fpr.ComparatorResults){
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");                    
                    System.Console.WriteLine("Comparator: {0}", rc.Comparator);
                    System.Console.WriteLine("Matching: {0}%", System.Math.Round(rc.Matching*100, 2));                                                        

                    if(level >= OutputLevel.DETAILED){                        
                        System.Console.WriteLine("******************************************************************************");
                        foreach(string c in rc.DetailsCaption)                          
                            System.Console.Write("{0}\t\t", c);

                        System.Console.WriteLine("");

                        foreach(string[] dh in rc.DetailsData){
                            foreach(string dl in dh){
                                System.Console.Write("{0}\t\t", dl.Replace("\t", ""));
                            }
                            System.Console.WriteLine("");
                        }
                            
                    }
                }                
                
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("");
            }
        }
    }
}
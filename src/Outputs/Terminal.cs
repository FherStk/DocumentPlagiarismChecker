/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
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
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("");
                Console.WriteLine("##############################################################################");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("   Left file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fpr.LeftFileName);
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("   Right file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fpr.RightFileName);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("   Matching: ");                
                Console.ForegroundColor = (fpr.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                Console.WriteLine("{0}%", Math.Round(fpr.Matching*100, 2));

                Console.ForegroundColor = ConsoleColor.DarkGray;
                if(level >= OutputLevel.MATCHING){
                    foreach(ComparatorMatchingScore rc in fpr.ComparatorResults){
                        Console.WriteLine("------------------------------------------------------------------------------");
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("   Comparator: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(rc.Comparator);

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("   Matching: ");
                        Console.ForegroundColor = (rc.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                        Console.WriteLine("{0}%", Math.Round(rc.Matching*100, 2));
                        
                        Console.ForegroundColor = ConsoleColor.Gray;
                        if(level >= OutputLevel.DETAILED){                        
                            Console.WriteLine("******************************************************************************");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            foreach(string c in rc.DetailsCaption)                          
                                Console.Write("      {0}\t\t", c);

                            Console.WriteLine("");
                            
                            Console.ForegroundColor = ConsoleColor.White;
                            foreach(string[] dh in rc.DetailsData){
                                foreach(string dl in dh){
                                    Console.Write("      {0}\t\t", dl.Replace("\t", ""));
                                }
                                Console.WriteLine("");
                            }                                                            
                        }
                    }  
                }
                              
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("##############################################################################");
                Console.WriteLine("");
            }
        }
    }
}
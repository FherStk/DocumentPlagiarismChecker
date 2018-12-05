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
        /// <param name="level">The output details level.</param>DisplayDisplay
        public override void Write(List<FileMatchingScore> results, DisplayLevel level = DisplayLevel.GLOBAL){            
            foreach(FileMatchingScore fms in results){
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("");
                Console.WriteLine("##############################################################################");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\tLeft file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fms.LeftFileName);
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\tDisplayt file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fms.RightFileName);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\tMatching: ");                
                Console.ForegroundColor = (fms.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                Console.WriteLine("{0}%", Math.Round(fms.Matching*100, 2));

                Console.ForegroundColor = ConsoleColor.DarkGray;
                if(level >= DisplayLevel.COMPARATOR){
                    foreach(ComparatorMatchingScore cms in fms.ComparatorResults){
                        Console.WriteLine("------------------------------------------------------------------------------");
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\t\tComparator: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(cms.Comparator);

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("\t\tMatching: ");
                        Console.ForegroundColor = (cms.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                        Console.WriteLine("{0}%", Math.Round(cms.Matching*100, 2));                        
                        Console.ForegroundColor = ConsoleColor.Gray;
                        
                        //Looping over the detials
                        string indent = "\t\t\t";
                        DetailsMatchingScore dms = cms.Child;
                        if(dms != null){
                            Console.WriteLine("******************************************************************************");
                            
                        while(dms != null){
                            if(level >= dms.DisplayLevel){
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                ///TODO: looping
                                for(int i = 0; i < <string c in cms.DetailsCaption){
                                    Console.Write("      {0}\t\t", c);
                                }                          
                                    

                                Console.WriteLine("");
                            }

                            dms = dms.Child;
                            indent += "\t";
                        }

                        //TODO: LOOK FOR DETAILS AND ITS OUTPUT LEVEL, ITERATE THROUGH THEM INCREASING THE INDENT
                        if(level >= DisplayLevel.FULL){                        
                            Console.WriteLine("******************************************************************************");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            foreach(string c in cms.DetailsCaption)                          
                                Console.Write("      {0}\t\t", c);

                            Console.WriteLine("");
                            
                            Console.ForegroundColor = ConsoleColor.White;
                            foreach(string[] dh in cms.DetailsData){
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
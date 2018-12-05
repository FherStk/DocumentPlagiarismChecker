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
                Console.Write("  Left file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fms.LeftFileName);
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("  Displayt file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fms.RightFileName);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("  Matching: ");                
                Console.ForegroundColor = (fms.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                Console.WriteLine("{0}%", Math.Round(fms.Matching*100, 2));
                
                if(level >= DisplayLevel.COMPARATOR){
                    foreach(ComparatorMatchingScore cms in fms.ComparatorResults){
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("------------------------------------------------------------------------------");                        

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("    Comparator: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(cms.Comparator);

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("    Matching: ");
                        Console.ForegroundColor = (cms.Matching <0.5f ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed); //TODO: use config. threshold
                        Console.WriteLine("{0}%", Math.Round(cms.Matching*100, 2));                        
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        
                        //Looping over the detials
                        string indent = "      ";
                        DetailsMatchingScore dms = cms.Child;
                        if(dms != null){
                            Console.WriteLine("··············································································");
                            
                            while(dms != null){
                                if(level >= dms.DisplayLevel){                 
                                    //TODO: move to cols + rows               
                                    for(int i = 0; i < cms.DetailsCaption.Length; i++){
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        Console.Write("{0}{1}: " ,indent, cms.DetailsCaption[i]);
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.WriteLine(cms.DetailsData[i]); 
                                    }                                                              
                                }

                                dms = dms.Child;
                                indent += "  ";
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
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.Linq;
using ConsoleTables;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;
using DocumentPlagiarismChecker.Scores;
using DocumentPlagiarismChecker.Settings;

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
        public override void Write(List<ComparatorMatchingScore> results){              
            DisplayLevel dl = Enum.Parse<DisplayLevel>(AppSettings.Instance.General.Display.ToUpper());     //TODO: try with enum inside settings       
            Console.OutputEncoding = System.Text.Encoding.UTF8;            
            WriteSeparator('#', ConsoleColor.DarkGray);                

            //The list of CMS must be grouped and sorted in order to display.
            foreach(IGrouping<string, ComparatorMatchingScore> grpLeft in results.GroupBy(x => x.LeftFileName)){            
                //Displays the left file info with its total match                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("  ⬩ Left file [");
                    
                float match = grpLeft.Sum(x => x.Matching) / grpLeft.Count();
                Console.ForegroundColor = (match < GetThreshold(DisplayLevel.BASIC) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                Console.Write("{0:P2}", match);
                
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("]: ");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(grpLeft.Key);                                                        

                foreach(IGrouping<string, ComparatorMatchingScore> grpRight in grpLeft.GroupBy(x => x.RightFileName)){
                    //Displays the right file info with its total match
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("     ⤷ Right file [");
                        
                    match = grpRight.Sum(x => x.Matching) / grpRight.Count();
                    Console.ForegroundColor = (match < GetThreshold(DisplayLevel.BASIC) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                    Console.Write("{0:P2}", match);
                    
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("]: ");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(grpRight.Key);                                                        

                    if(dl >= DisplayLevel.COMPARATOR){
                        foreach(ComparatorMatchingScore comp in grpRight.Select(x => x).ToList()){
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("        ⤷ Comparator [");
                                
                            Console.ForegroundColor = (comp.Matching < GetThreshold(DisplayLevel.BASIC) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                            Console.Write("{0:P2}", comp.Matching);
                            
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("]: ");

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(comp.Comparator);

                            //Looping over the detials
                            DetailsMatchingScore dms = (DetailsMatchingScore)comp;
                            while(dms != null){
                                if(dl >= dms.DisplayLevel){      
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine();
                                    WriteSeparator('·', ConsoleColor.DarkRed);          
                                    Console.WriteLine();                          
                                    Console.WriteLine("  {1}: displaying details for matching values over {0:P2}", GetThreshold(dms.DisplayLevel), comp.Comparator);
                                    Console.WriteLine();

                                    var table = new ConsoleTable(dms.DetailsCaption);
                                    for(int i = 0; i < dms.DetailsData.Count; i++){
                                        if(dms.DetailsMatch[i] > GetThreshold(dms.DisplayLevel)){                                        
                                            List<string> formatedData = new List<string>();
                                            for(int j = 0; j < dms.DetailsFormat.Length; j++){                                            
                                                if(dms.DetailsFormat[j].Contains(":L")){
                                                //Custom string length formatting output
                                                    string sl = dms.DetailsFormat[j].Substring(dms.DetailsFormat[j].IndexOf(":L")+2);
                                                    sl = sl.Substring(0, sl.IndexOf("}"));
                                                    
                                                    int length = int.Parse(sl);
                                                    string pText = dms.DetailsData[i][j].ToString();
                                                    if(pText.Length <= length) formatedData.Add(pText);
                                                    else formatedData.Add(string.Format("{0}...", pText.Substring(0, length - 3)));                                                       
                                                }
                                                else{
                                                    //Native string formatting output
                                                    formatedData.Add(String.Format(dms.DetailsFormat[j], dms.DetailsData[i][j]));
                                                }                                            
                                            }                                            
                                            
                                            table.AddRow(formatedData.ToArray());
                                        }
                                    }
                                                                    
                                    table.Write(); 
                                    Console.WriteLine();                                                                                                                                                         
                                }
                                dms = dms.Child;
                            }
                        }
                    }
                }    

                Console.WriteLine();            
                //WriteSeparator('.', ConsoleColor.Gray);

            /*            
            foreach(ComparatorMatchingScore cms in results){
                //Displays the left file info and total match
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("");
                Console.WriteLine("##############################################################################");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("  Left file: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(cms.FileName);                                

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("  Matching: ");                
                Console.ForegroundColor = (cms.Matching < GetThreshold(DisplayLevel.BASIC) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                Console.WriteLine("{0:P2}", cms.Matching);

                //Displays the right file info and total match (related with the previous left file).
                foreach(IGrouping<string, ComparatorMatchingScore> group in cms.ComparatorResults.GroupBy(x => x.RightFileName)){
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("    Right file: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(group.Key);                                
                }                

                foreach(ComparatorMatchingScore cms in fms.ComparatorResults.Where(x => x.LeftFileName == fms.FileName)){

                }
                
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
                        Console.ForegroundColor = (cms.Matching < GetThreshold(DisplayLevel.COMPARATOR) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                        Console.WriteLine("{0:P2}", cms.Matching);                        
                        
                        //Looping over the detials
                        DetailsMatchingScore dms = (DetailsMatchingScore)cms;
                        while(dms != null){
                            if(level >= dms.DisplayLevel){      
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("··············································································");
                                Console.WriteLine();
                                
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine(string.Format("  Displaying details with a match value > {0:P2}", GetThreshold(dms.DisplayLevel)));
                                Console.WriteLine();

                                var table = new ConsoleTable(dms.DetailsCaption);
                                for(int i = 0; i < dms.DetailsData.Count; i++){
                                    if(dms.DetailsMatch[i] > GetThreshold(dms.DisplayLevel)){                                        
                                        List<string> formatedData = new List<string>();
                                        for(int j = 0; j < dms.DetailsFormat.Length; j++){                                            
                                            if(dms.DetailsFormat[j].Contains(":L")){
                                               //Custom string length formatting output
                                                string sl = dms.DetailsFormat[j].Substring(dms.DetailsFormat[j].IndexOf(":L")+2);
                                                sl = sl.Substring(0, sl.IndexOf("}"));
                                                
                                                int length = int.Parse(sl);
                                                string pText = dms.DetailsData[i][j].ToString();
                                                if(pText.Length <= length) formatedData.Add(pText);
                                                else formatedData.Add(string.Format("{0}...", pText.Substring(0, length - 3)));                                                       
                                            }
                                            else{
                                                 //Native string formatting output
                                                formatedData.Add(String.Format(dms.DetailsFormat[j], dms.DetailsData[i][j]));
                                            }                                            
                                        }                                            
                                        
                                        table.AddRow(formatedData.ToArray());
                                    }
                                }
                                                                
                                table.Write(); 
                                Console.WriteLine();                                                                                                                                                         
                            }
                            dms = dms.Child;
                        }
                    }                       
                }
                */                                           
            }

            WriteSeparator('#', ConsoleColor.DarkGray);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private float GetThreshold(DisplayLevel level){
            switch(level){
                case DisplayLevel.BASIC:
                    return AppSettings.Instance.General.Threshold.Basic;

                case DisplayLevel.COMPARATOR:
                    return AppSettings.Instance.General.Threshold.Comparator;

                case DisplayLevel.DETAILED:
                    return AppSettings.Instance.General.Threshold.Detailed;

                case DisplayLevel.FULL:
                    return AppSettings.Instance.General.Threshold.Full;

                default:
                    throw new Exceptions.DisplayLevelNotFound();
            }
        }

        private void WriteSeparator(char symbol, ConsoleColor color = ConsoleColor.White){            
            Console.ForegroundColor = color;

            for(int i = 0; i < Console.WindowWidth; i++)
                Console.Write(symbol);

            Console.WriteLine();
        }
    }
}
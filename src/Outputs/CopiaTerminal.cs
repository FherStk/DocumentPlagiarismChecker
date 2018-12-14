/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using ConsoleTables;
using System.Collections;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;


namespace DocumentPlagiarismChecker.Outputs
{
    /// <summary>
    /// This output base object sends the results to the terminal.
    /// </summary>
    internal class CopiaTerminal: Core.BaseOutput{
        /// <summary>
        /// Writes the given set of results into the terminal.
        /// </summary>
        /// <param name="results">A set of results regarding each compared pair of files.</param>
        /// <param name="level">The output details level.</param>DisplayDisplay
        public override void Write(List<FileMatchingScore> results, DisplayLevel level = DisplayLevel.BASIC){   
            ArrayList myAL = new ArrayList();
         
            foreach(FileMatchingScore fms in results){

                myAL.Add("##############################################################################");
                myAL.Add("  Left file: ");
                myAL.Add(fms.LeftFileName);
                
                myAL.Add("  Right file: ");
                myAL.Add(fms.RightFileName);

                myAL.Add("  Matching: ");                
                myAL.Add(fms.Matching);
                
                if(level >= DisplayLevel.COMPARATOR){
                    foreach(ComparatorMatchingScore cms in fms.ComparatorResults){
                        myAL.Add("------------------------------------------------------------------------------");                        

                        myAL.Add("    Comparator: ");
                        myAL.Add(cms.Comparator);

                        myAL.Add("    Matching: ");
                        Console.ForegroundColor = (cms.Matching < GetThreshold(DisplayLevel.COMPARATOR) ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
                        Console.WriteLine("{0:P2}", cms.Matching);                        
                        
                        //Looping over the detials
                        DetailsMatchingScore dms = (DetailsMatchingScore)cms;
                        while(dms != null){
                            if(level >= dms.DisplayLevel){      
                                myAL.Add("··············································································");
                                myAL.Add("");
                                
                                myAL.Add(string.Format("  Displaying details with a match value > {0:P2}", GetThreshold(dms.DisplayLevel)));
                                myAL.Add("");

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
                                                                
                                myAL.Add(table);
                                myAL.Add("");                                                                                                                                                         
                            }
                            dms = dms.Child;
                        }
                    }  
                }
                              
                myAL.Add("##############################################################################");
                myAL.Add("");
            }

            using (System.IO.StreamWriter file = 
            new System.IO.StreamWriter(@"C:\Users\David\Desktop\WriteLines2.txt"))
        {
            foreach (string line in myAL)
            {
                file.WriteLine(line);
            }
        }
        }

        private float GetThreshold(DisplayLevel level){
            return float.Parse(Settings.Instance.Get(string.Format("threshold:{0}", level.ToString().ToLower())),  System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
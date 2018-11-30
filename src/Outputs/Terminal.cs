using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Outputs
{
    internal class Terminal: Core.BaseOutput{
        public override void Write(List<FileMatchingScore> results, OutputLevel level = OutputLevel.BASIC){            
            foreach(FileMatchingScore fpr in results){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Left file: {0}", fpr.LeftFileName);
                System.Console.WriteLine("Right file: {0}", fpr.RightFileName);
                System.Console.WriteLine("Matching: {0}%", System.Math.Round(fpr.Matching*100, 2));

                foreach(ComparerMatchingScore rc in fpr.ComparerResults){
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");                    
                    System.Console.WriteLine("Comparer: {0}", rc.Comparer);
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
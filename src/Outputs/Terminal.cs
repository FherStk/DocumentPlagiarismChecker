using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Outputs
{
    internal class Terminal: Core.BaseOutput{
        public override void Write(int detail){            
            foreach(FilePairResults fpr in Results.Instance.ResultFiles){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Left file: {0}", fpr.LeftFileName);
                System.Console.WriteLine("Right file: {0}", fpr.RightFileName);
                System.Console.WriteLine("Matching: {0}%", System.Math.Round(fpr.Matching*100, 2));

                foreach(ComparerResults rc in fpr.ComparerResults){
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    System.Console.WriteLine("Comparer: {0}", rc.Comparer);
                    System.Console.WriteLine("Matching: {0}%", System.Math.Round(rc.Matching*100, 2));                                    
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");                                        

                    //TODO: print details
                }                
                
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("");
            }
        }
    }
}
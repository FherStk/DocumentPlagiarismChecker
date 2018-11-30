using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Outputs
{
    internal class Terminal: Core.BaseOutput{
        public override void Write(List<Result> results, int detail){
            foreach(Result r in results){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Global matching: {0}%",  System.Math.Round(r.Matching*100, 2));    

                foreach(ResultHeader rh in r.Headers){
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    System.Console.WriteLine("Comparer: {0}", rh.Comparer);
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");                    
                    System.Console.WriteLine("{0}", rh.LeftCaption);
                    System.Console.WriteLine("{0}", rh.RightCaption);                                                    
                    System.Console.WriteLine("Matching: {0}%", System.Math.Round(r.Matching*100, 2));
                 
                    if(detail == 5){    //TODO: implement details level as an ENUM and details info.
                        System.Console.WriteLine("------------------------------------------------------------------------------");
                        foreach(ResultLine rl in rh.Lines){
                            System.Console.WriteLine("Word: {0} | Left: {1} | Right: {2} | Matching: {3}%", rl.Item, rl.LeftValue, rl.RightValue, System.Math.Round(rl.Matching*100, 2));
                        }
                    }
                }                
                
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("");
            }
        }
    }
}
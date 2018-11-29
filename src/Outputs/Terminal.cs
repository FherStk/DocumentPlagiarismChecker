using System.Collections.Generic;
using PdfPlagiarismChecker.Core;

namespace PdfPlagiarismChecker.Outputs
{
    internal class Terminal: Core.BaseOutput{
        protected override void Write(List<ResultHeader> results, int level){
            foreach(ResultHeader r in results){
                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("Left file: {0}", r.LeftCaption);
                System.Console.WriteLine("Right file: {0}", r.RightCaption);
                System.Console.WriteLine("Matching: {0}%", System.Math.Round(r.Matching*100, 2));                

                if(false){
                    System.Console.WriteLine("::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                    foreach(ResultLine rl in r.Lines){
                        System.Console.WriteLine("Word: {0} | Left: {1} | Right: {2} | Matching: {3}%", rl.Item, rl.LeftValue, rl.RightValue, System.Math.Round(rl.Matching*100, 2));
                    }
                }

                System.Console.WriteLine("##############################################################################");
                System.Console.WriteLine("");
            }
        }
    }
}
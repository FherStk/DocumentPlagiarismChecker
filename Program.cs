namespace PdfPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {            
            Comparers.WordCounter.Worker wc = new Comparers.WordCounter.Worker();
            wc.Run("C:\\test", "pdf");
            //WordCounter.Document d = new WordCounter.Document("");
            //d.words.Add(null);
            
            /*    
                PENDING:            
                    The main program will run all the comparers one after each other and the matching % will be recalculated.
                    It will be possible to setup some configuration inside an XML file (for example, kind of output and number of check plugins to use (all by default)).
                    It will be possible to configure the detail of the output:
                        - basic (left; right; % mathing total)
                        - detailed (previous + individual % matching for each comparer)
                        - full (previous + registry data used in order to calculate the individual % matching for each comparer, for example, % of match on each individual word.)   
             */
        }
    }
}

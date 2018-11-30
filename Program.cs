namespace DocumentPlagiarismChecker
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
                CURRENT:
                    The comparisson will be made following a one-to-many pattern so the result will be more ore less as follows (including details level):
                        [Level-1: basic] For each file: File name; Total file % matching.
                            [Level-2: specific] For each file compared with the previous one (one-to-many): File name; Comprarer used; File % matching.
                                [Level-3: full] For each compared used: Left side caption (header) + left side values (lines);  Right side caption (header) + right side values (lines);  Check caption (header) + check values (lines); 

                PENDING:            
                    The main program will run all the comparers one after each other and the matching % will be recalculated.
                    It will be possible to setup some configuration inside an XML file (for example, kind of output and number of check plugins to use (all by default)).                    
             */
        }
    }
}

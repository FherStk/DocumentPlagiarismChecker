namespace PdfPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {            
            Comparers.WordCounter.Counter wc = new Comparers.WordCounter.Counter();
            wc.Run("C:\\test");
            //WordCounter.Document d = new WordCounter.Document("");
            //d.words.Add(null);
            
            /*    
                IDEAS:            
                    It will be possible to add new comparers inside its own folder at /WordCounter, inheriting from some key objects and programming the key methods.
                    The main program will run all the comparers one after each other and the matching % will be recalculated.
                    It will be possible to configure the detail of the output:
                        - basic (left; right; % mathing total)
                        - detailed (previous + individual % matching for each comparer)
                        - full (previous + registry data used in order to calculate the individual % matching for each comparer, for example, % of match on each individual word.)   

                TO BE PROPOSED:
                    How to store internally the data in order to output it, allowing the addition of new comparers easailly.

             */
        }
    }
}

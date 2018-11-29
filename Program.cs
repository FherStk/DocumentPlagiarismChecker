namespace PdfPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            WordCounter.Worker.Run("C:\\test");
            //WordCounter.Document d = new WordCounter.Document("");
            //d.words.Add(null);
        }
    }
}

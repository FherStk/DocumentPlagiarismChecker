using System;
using PdfPlagiarismChecker;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace PdfPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
        }

        private static void Test(){
            var pdfFile = createSamplePdfFile();
            var reader = new PdfReader(pdfFile);

            var streamBytes = reader.GetPageContent(1);
            var tokenizer = new PrTokeniser(new RandomAccessFileOrArray(streamBytes));

            var stringsList = new List<string>();
            while (tokenizer.NextToken())
            {
                if (tokenizer.TokenType == PrTokeniser.TK_STRING)
                {
                    stringsList.Add(tokenizer.StringValue);
                }
            }

            reader.Close();
        }

         private static byte[] createSamplePdfFile()
        {
            using (var stream = new MemoryStream())
            {
                var document = new Document();

                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor("Anonimous");
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello DNT!"));

                document.NewPage();
                // we don't add anything to this page: newPage() will be ignored
                document.Add(new Phrase(""));

                document.NewPage();
                writer.PageEmpty = false;

                document.Close();
                return stream.ToArray();
            }
        }

    }
}

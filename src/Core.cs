using System.IO;
using System.Linq;
using iTextSharp.text.pdf;

namespace PdfPlagiarismChecker
{
    class Core
    {
        private void Extract(string path){
            if(!Directory.Exists(path)) 
                throw new FolderNotFoundException();

            PdfReader reader = null;
            foreach(string filePath in Directory.GetFiles(path).Where(x => Path.GetExtension(x).ToLower().Equals("pdf"))){
                reader = new PdfReader(filePath);
                reader.Close();
            }
        }
    }   
}
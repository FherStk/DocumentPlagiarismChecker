using System.IO;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseDocument{
        public string Name {get; protected set;}        

        protected BaseDocument(string path){
            //Check pre-conditions
            if(!File.Exists(path)) 
                throw new FolderNotFoundException();

            this.Name = System.IO.Path.GetFileName(path);
        }       
    }
}
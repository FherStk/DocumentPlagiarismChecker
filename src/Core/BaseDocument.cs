using System.IO;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseDocument{
        private string _name;
        public string Name {
            get{
                return _name;
            } 
            protected set{
                _name = value;
            }
        }

        protected BaseDocument(string path){
            //Check pre-conditions
            if(!File.Exists(path)) 
                throw new FolderNotFoundException();

            this.Name = System.IO.Path.GetFileName(path);
        }       
    }
}
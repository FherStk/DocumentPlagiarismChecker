using System.IO;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the document schema that every docuemnt object must inherit in order to work as expected.
    /// </summary>
    internal abstract class BaseDocument{
        /// <summary>
        /// The document name (matches with the loaded file name).
        /// </summary>
        public string Name {get; protected set;}        

        /// <summary>
        /// Instantiates a new document object.
        /// </summary>
        /// <param name="filePath"></param>
        protected BaseDocument(string filePath){
            //Check pre-conditions
            if(!File.Exists(filePath)) 
                throw new FolderNotFoundException();

            this.Name = System.IO.Path.GetFileName(filePath);
        }       
    }
}
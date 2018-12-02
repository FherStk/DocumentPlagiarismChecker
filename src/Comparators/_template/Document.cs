namespace DocumentPlagiarismChecker.Comparators._template
{
    internal class Document: Core.BaseDocument
    {                
        
        /// <summary>
        /// The document constructor is used in order to load and parse the given file path into whatever you need.
        /// </summary>
        /// <param name="filePath">The location of the file.</param>
        public Document(string filePath): base(filePath){
        }
    }
}
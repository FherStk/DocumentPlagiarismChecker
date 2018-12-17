namespace DocumentPlagiarismChecker.Settings
{ 
    
        public class GeneralSettings{
            public string Folder{get; set;}
            public string Sample{get; set;}
            public string Extension{get; set;}
            public string Display{get; set;}
            public bool Recursive{get; set;}
            public string[] Exclusion {get; set;}
            public ThresholdSettings Threshold {get; set;}            
        }
}
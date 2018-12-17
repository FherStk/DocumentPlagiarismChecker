namespace DocumentPlagiarismChecker.Core
{
    public class Temp{
        public Global global {get; set;}
        public Threshold threshold {get; set;}
        public string[] exclusion {get; set;}
    }

    public class Global{
        public string folder{get;set;}
        public string sample{get;set;}
        public string extension{get;set;}
        public string display{get;set;}
        public bool recursive{get;set;}

    }

    public class Threshold{
        public float basic{get;set;}
        public float comparator{get;set;}
        public float detailed{get;set;}
        public float full{get;set;}
    }
}
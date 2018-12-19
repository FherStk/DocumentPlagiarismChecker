
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.

    Singleton pattern by: http://csharpindepth.com/articles/general/singleton.aspx
 */

using System.IO;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DocumentPlagiarismChecker
{
    
    public class Settings
    {       
        public string Folder{get; set;}
        public string Sample{get; set;}
        public string Extension{get; set;}
        public string Display{get; set;}
        public bool Recursive{get; set;}
        public string[] Exclusion {get; set;}
        public ThresholdSettings Threshold {get; set;}    

        public class ThresholdSettings{
            public float Basic{get; set;}
            public float Comparator{get; set;}
            public float Detailed{get; set;}
            public float Full{get; set;}
        }   

        public Settings(){}
        public Settings(string path){
            if(!System.IO.File.Exists(path)) throw new Exceptions.FileNotFoundException();

            Deserializer deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            using (StreamReader sr = new StreamReader(path)) {
                Parser parser = new Parser(sr);
                parser.Expect<StreamStart>();

                while (parser.Accept<DocumentStart>())
                {            
                    Settings s = (Settings)deserializer.Deserialize<Settings>(parser);  
                    foreach (PropertyInfo pi in s.GetType().GetProperties()){
                        pi.SetValue(this, pi.GetValue(s));
                    }
                }
            }
        }

        public void Set(string name, object value){
            string[] item = name.Split("-");
            object current = this;
            bool found = false;

            for (int i=0; i<item.Length; i++){                
                foreach (PropertyInfo pi in current.GetType().GetProperties()){
                    if(pi.Name.ToLower() == item[i].ToLower()){
                        if(i+1 <  item.Length) current = pi.GetValue(current);
                        else{
                            pi.SetValue(current, System.Convert.ChangeType(value, pi.PropertyType));
                            found = true;
                        }

                        break;
                    }
                }   
            }

            if(!found) throw new Exceptions.AppSettingNotFound();
        }
    }

}
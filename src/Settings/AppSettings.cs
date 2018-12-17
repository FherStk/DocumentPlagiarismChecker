
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.

    Singleton pattern by: http://csharpindepth.com/articles/general/singleton.aspx
 */

using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DocumentPlagiarismChecker.Settings
{    
    public sealed class AppSettings
    {      
        public GeneralSettings General {get; set;}
        private bool _firstLoad = true;
        private static readonly AppSettings instance = new AppSettings();        

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppSettings()
        {
        }

        private AppSettings()
        {
        }

        public static AppSettings Instance
        {
            get
            {   
                if(instance._firstLoad){
                    instance._firstLoad = false;
                    instance.Load();
                } 

                return instance;
            }
        }

        public void Load(string path = "settings.yaml"){
            if(!System.IO.File.Exists(path)) return;

            Deserializer deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
            using (StreamReader sr = new StreamReader(path)) {
                Parser parser = new Parser(sr);
                parser.Expect<StreamStart>();

                while (parser.Accept<DocumentStart>())
                {
                    this.General = (GeneralSettings)deserializer.Deserialize<GeneralSettings>(parser);                    
                }
            }
        }

        public void Set(string name, object value){
            string[] item = name.Split("-");
            object current = General;
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
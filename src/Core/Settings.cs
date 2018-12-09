
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.

    Singleton pattern by: http://csharpindepth.com/articles/general/singleton.aspx
 */

using System.IO;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;

namespace DocumentPlagiarismChecker.Core
{
    public enum Setting{
        GLOBAL_FOLDER,
        GLOBAL_SAMPLE,
        GLOBAL_EXTENSION,
        GLOBAL_DISPLAY,
        GLOBAL_RECURSIVE,
        THRESHOLD_BASIC,
        THRESHOLD_COMPARATOR,
        THRESHOLD_DETAILS,
        THRESHOLD_FULL
    }

    public sealed class Settings
    {
        private  YamlMappingNode _settings;
        private bool _firstLoad = true;
        private static readonly Settings instance = new Settings();
        /// <summary>
        /// The absolute path to the folder containing the documents that must be compared.
        /// </summary>
        private string Folder {get; set;}

        /// <summary>
        /// Files with other extensions inside the folder will be omited.
        /// </summary>
        private string Extension {get; set;}

        /// <summary>
        /// The absolute path to the sample file, results matching the content of this file will be ommited (like parts of homework statements).
        /// </summary>
        private string Sample {get; set;}

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Settings()
        {
        }

        private Settings()
        {
        }

        public static Settings Instance
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

            using (var reader = new StreamReader(path)) {
                YamlStream yaml = new YamlStream();
                yaml.Load(reader);

                _settings = (YamlMappingNode)yaml.Documents[0].RootNode;            
            }
        }

        public string Get(Setting setting){
            return Get(setting.ToString().ToLower().Replace("_", ":"));
        }
        public void Set(Setting setting, string value){
            Set(setting.ToString().ToLower().Replace("_", ":"), value);
        }
        public string Get(string setting){       
            return Find(setting).Value;           
        }
        public void Set(string setting, string value){
            YamlScalarNode s = Find(setting);
            s.Value = value;
        }

        private YamlScalarNode Find(string setting){
            if(_settings == null) throw new SettingsFileNotFoundException();

            string[] levels = setting.Split(":");
            YamlMappingNode current = _settings;

            foreach(string l in levels.Take(levels.Length - 1))
                current = (YamlMappingNode)current.Children.Where(x => ((YamlScalarNode)x.Key).Value == l).SingleOrDefault().Value;

            KeyValuePair<YamlNode, YamlNode> val = current.Children.Where(x => ((YamlScalarNode)x.Key).Value == levels.LastOrDefault()).SingleOrDefault();
            return (YamlScalarNode)val.Value;
        }
    }
}
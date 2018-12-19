/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
namespace DocumentPlagiarismChecker.Exceptions
{
    public class FolderNotSpecifiedException: System.Exception{}
    public class SettingsFileNotFoundException: System.Exception{}
    public class FileExtensionNotSpecifiedException: System.Exception{}
    public class FolderNotFoundException: System.Exception {}
    public class FileNotFoundException: System.Exception{}
    public class MatchValueNotValid: System.Exception {}
    public class DisplayLevelNotAllowed: System.Exception {}
    public class FileExtensionNotAllowed: System.Exception {}   
    public class DisplayLevelNotFound: System.Exception {}   
    public class AppSettingNotFound: System.Exception {}   
}
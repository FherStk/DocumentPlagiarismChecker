/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
namespace DocumentPlagiarismChecker.Exceptions
{
    internal class FolderNotSpecifiedException: System.Exception{}
    internal class SettingsFileNotFoundException: System.Exception{}
    internal class FileExtensionNotSpecifiedException: System.Exception{}
    internal class FolderNotFoundException: System.Exception {}
    internal class MatchValueNotValid: System.Exception {}
    internal class DisplayLevelNotAllowed: System.Exception {}
    internal class FileExtensionNotAllowed: System.Exception {}   
    internal class DisplayLevelNotFound: System.Exception {}   
    internal class AppSettingNotFound: System.Exception {}   
}
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
namespace DocumentPlagiarismChecker.Core
{
    internal class FolderNotSpecifiedException: System.Exception{}
    internal class FileExtensionNotSpecifiedException: System.Exception{}
    internal class FolderNotFoundException: System.Exception {}
    internal class MatchValueNotValid: System.Exception {}
}
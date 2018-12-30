# Document Plagiarism Checker
Document Plagiarism Checker is an Open-Source C# project over .NET Core 2.1 that has been developed for academic purposes only, which implements a really simple way to compare a set of documents between each other in order to check if some of them are copies.

Feel free to use, copy, fork or modify this project; but please refer a mention to this project and its author respecting also the licenses of the included third party software.

### Third party software and licenses:
Please notice than this project could not be possible without the help of:
* The [iTextSharp](https://developers.itextpdf.com/downloads) library property of [iText](https://itextpdf.com/): under the AGPL v3 license, so no commercial use of this program can be afforded without paying the corresponding fee to the original authors (further details about the license can be found at [http://itextpdf.com/terms-of-use/](http://itextpdf.com/terms-of-use/)).
* The [ConsoleTables](https://github.com/khalidabuhakmeh/ConsoleTables) library property of [Khalid Abuhakmeh](https://github.com/khalidabuhakmeh): under the MIT license, (further details about the license can be found at [https://github.com/khalidabuhakmeh/ConsoleTables/blob/master/LICENSE](https://github.com/khalidabuhakmeh/ConsoleTables/blob/master/LICENSE)).    
* The [YamlDotNet](https://github.com/aaubry/YamlDotNet) library property of [Antoine Aubry](https://github.com/aaubry): under the MIT license, (further details about the license can be found at [https://github.com/aaubry/YamlDotNet/blob/master/LICENSE](https://github.com/aaubry/YamlDotNet/blob/master/LICENSE)).

## WARNING: still in an early development stage.
### How to use it:
#### As an stand-alone console app:
Clone the repository to your local working directory, restore the dependencies with `dotnet restore`, build it with `dotnet build` and, finally, run the project with `dotnet run`. 

If there is no *settings.yaml* file in the same folder as the program, it will be mandatory to specify where to find it with `dotnet run --setting="FILE_PATH"`; for further details explore the *settings.yaml* file that comes within this project.
#### As a library:
Do the same as with the stand-alone app but import the compiled **DocumentPlagiarismChecker.dll** file to your project. Then invoke the **CompareFiles** method inside the **API** object to get the results. You can also send them to an output with the **WriteOutput** method inside the same **API** object:

**Synchronous example**
```
API api = new API();
api.CompareFiles();
api.WriteOutput();
```

**Asynchronous example**
```
API api = new API();
Task compare = Task.Run(() => 
    api.CompareFiles()
).ContinueWith((x) => {
    api.WriteOutput();
});

//SOME CODE
```

**Asynchronous example (with progress indicator)**
```
API api = new API();
Task compare = Task.Run(() => 
    api.CompareFiles()
);

Task progress = Task.Run(() => {
    while(api.Progress < 1){
        Console.Write("\r{0:P2}", api.Progress);
        System.Threading.Thread.Sleep(1000);
    }

    Console.Write("\rLoading... {0:P2}", api.Progress);
    Console.WriteLine();
    Console.WriteLine("Done! Printing results:");
});

progress.Wait();
```

Please, notice that all configuration is performed through the *settings.yaml* file under the same path as the program, so if there is no file a new one will must be established with `Settings.Instance.Load(path);` in order to proceed.

### How to add new comparator:
New comparators will be added as long as the tool became improved with new capabilities but, if anyone wants to contribute or just code their own comparator, feel free to enjoy following those steps:
 1. Copy the **_tamplate** folder with all its content inside the **Comparators** folder.
 2. Rename the new folder with the name of your comparator.
 2. Correct the namespace of the copied folders and replace **_template** with the name of your comparator (must match the name of the folder).
 3. Code both files following the indications, but you can use the current comparators as a guide.
 ### List of comparators (marked ones are avaliables, the other ones are under development):
- [x] Sentence Word Match: compares all the sentences between each other in order to get total or partial matches along its words.
- [x] ~~Document Word Counter: compares two PDF files and check how many words and how many times appears within each document, useful for checking if two documents are almost equals.~~ [DEPRECTAED]
- [X] ~~Paragraph Word Counter: compares two PDF files and check how many paragraphs contains similar sentences, useful for checking which if two paragraphs are almost equals.~~ [DEPRECTAED]
- [X] ~~Paragraph Length Counter: compares two PDF files and check how many paragraph has the same length, useful for checking which parts of two documents could have been replaced by synonyms.~~ [DEPRECTAED]
 ### Roadmap:
The full list of ideas and improvements can be found at [issues section (with the enhancements tag).](https://github.com/FherStk/DocumentPlagiarismChecker/issues?utf8=%E2%9C%93&q=is%3Aissue+is%3Aopen+is%3Aenhancement)
### Changelog:
* v0.7.0.0-alpha (XX/12/2018):
    * The console app does not accept parameters anymore (but the settings one) so settings file is the only way to set the configuration for a new app instance.
    * The old comparators have been removed and replaced by the new Sentence Word Match, which compares all the sentences between each other in order to get total or partial matches along its words.

* v0.6.0.0-alpha (20/12/2018):
    * New settings in order to allow regular expressions as exclussion list items, so paragraphs matching those expressions will be ignored by the Paragraph Word Counter comparator.
    * A new output format for the terminal output (Left file [matching %] -> Right file [matching %] -> Comparator [matching %]).
    * The Paragraph Word Counter has been splitted into a new comparator: the Paragraph Length Counter.
    * For further information see [the full list of changes](https://github.com/FherStk/DocumentPlagiarismChecker/projects/3).

* v0.5.0.0-alpha (09/12/2018):
    * A progress indicator has been added when running the app through the terminal.
    * A new parameter has been added to the settings (recursive) in order to set the file search method inside the given folder.
    * New parameters has been added to the settings in order to set the threshold values that will be used in order to determine if there is a match between two comparisons.
    * The Document and FileMatchingScore objects stores the full path for a file instead of its single name.
    * For further information see [the full list of changes](https://github.com/FherStk/DocumentPlagiarismChecker/projects/1).

* v0.4.0.0-alpha (06/12/2018):
    * A settings file has been added, so the input arguments (console) can be omited if the mandatory settings are defined inside the yaml file or Settings.Instance.Set(setting, value) method (API) can be used. Notice that settings file data will be overwriten if new information is sent throught the arguments console or API.
    * The output console has been improved, adding multi-level options, output colors and indentation.

* v0.3.0.0-alpha (06/12/2018):
    * The "Paragraph Word Counter" has been added, and can be used in order to count how many words and how many times appears on each paragraph, having also in count the paragraph's length when calculing the matching percentage.
    * The output console has been improved, adding multi-level options, output colors and indentation.

* v0.2.0.0-alpha (03/12/2018):
    * A sample file can be used in order to exclude some data from the comparisson.

* v0.1.0.0-alpha (02/12/2018):
    * Initial release.

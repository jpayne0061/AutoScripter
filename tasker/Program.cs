using System;
using System.Diagnostics;
using System.IO;

namespace tasker
{
    class Program
    {
        static string _pathForGenInserts;
        static string _object;
        static string _tempObjectName;
        static string _tempTableCreatePath;
        static string _pathToMergeScript;
        static string _fullScriptPath;
        static string _objProtected;

        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "sections") {
                _pathForGenInserts = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeSectionData\generatedInserts.sql";
                _object = "dbo.Sections";
                _objProtected = "[dbo].[Sections]";
                _tempObjectName = "#newSections";
                _tempTableCreatePath = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeSectionData\createTempSection.sql";
                _pathToMergeScript = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeSectionData\mergeSections.sql";
                _fullScriptPath = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeSectionData\generatedMerges\";
            }
            else {
                _pathForGenInserts = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeChallengeData\generatedInserts.sql";
                _object = "dbo.Challenges";
                _objProtected = "[dbo].[Challenges]";
                _tempObjectName = "#newChallenges";
                _tempTableCreatePath = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeChallengeData\createTempChallenges.sql";
                _pathToMergeScript = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeChallengeData\mergeChallenges.sql";
                _fullScriptPath = @"C:\Users\Evan\Source\Repos\sqlStuffCli\sqlScripts\Data\mergeChallengeData\generatedMerges\";
            }

            string strCmdText;
            strCmdText = @"/C mssql-scripter -S DESKTOP-IA957IQ\SQLEXPRESS -d AppDb -U sa --data-only" +
                @" --include-objects " + _object + " > "+ _pathForGenInserts;
            try
            {
                Process cmd = Process.Start("CMD.exe", strCmdText);
                cmd.WaitForExit();
            }
            catch(Exception e)
            {
                Console.WriteLine("something went wrong: " + e.Message);
                return;
            }


            string insertStatements = System.IO.File.ReadAllText(_pathForGenInserts);
            string modStatements = insertStatements.Replace("SET IDENTITY_INSERT " + _objProtected + " ON", "");
            modStatements = modStatements.Replace("SET IDENTITY_INSERT " + _objProtected + " OFF", "");
            modStatements = modStatements.Replace(_objProtected, _tempObjectName);

            string createTempTableCommandText = System.IO.File.ReadAllText(_tempTableCreatePath);

            string mergeText = System.IO.File.ReadAllText(_pathToMergeScript);

            string fullStatement = createTempTableCommandText + modStatements + mergeText;

            string fileName = "fullMerge_GEN_" + DateTime.Now.ToFileTime() + ".sql";

            File.WriteAllText(_fullScriptPath + fileName, fullStatement);

            System.IO.File.Delete(_pathForGenInserts);
        }

    }
}

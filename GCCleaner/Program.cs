using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GCCleaner
{
    internal class Program
    {
        [RequiresUnreferencedCode("Calls Microsoft.Extensions.Configuration.ConfigurationBinder.Get<T>()")]
        [RequiresDynamicCode("Calls Microsoft.Extensions.Configuration.ConfigurationBinder.Get<T>()")]
        static int Main(string[] args)
        {
            GCCSettings? gccSettings = null;
            try
            {

                try
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("You must provide a path to a file as parameter.");
                        return 0;
                    }
                    string strSourceFileName = args[0];
                    if (!File.Exists(strSourceFileName))
                    {
                        Console.WriteLine($"File: {Environment.NewLine}{strSourceFileName}{Environment.NewLine} not found.");
                        return 0;
                    }

                    var codeBaseUrl = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                    var filePathToCodeBase = new Uri(codeBaseUrl).LocalPath;
                    string theDirectory = Path.GetDirectoryName(filePathToCodeBase);
                    IConfigurationBuilder cfgBuilder = new ConfigurationBuilder()
                         .SetBasePath(theDirectory)
                        .AddJsonFile("appsettings.json", optional: false);
                    IConfigurationRoot iCFGRoot = cfgBuilder.Build();
                    gccSettings = iCFGRoot.Get<GCCSettings>();
                    if (gccSettings == null || gccSettings.LineDescriptions?.Count < 1)
                    {
                        Console.WriteLine($"There is a problem with the content of appsettings.config {Environment.NewLine}Use this as an example:");
                        PrintDefaultSettings();
                        return (0);
                    }
                    foreach (LineDescription lD in gccSettings.LineDescriptions)
                    {
                        string strErg = lD.CheckDescription();
                        if (strErg != "OK")
                        {
                            Console.WriteLine($"There is a problem with at least one LineDescription in appsettings.config{Environment.NewLine}{strErg}{Environment.NewLine}Use this as an example:");
                            PrintDefaultSettings();
                            return (0);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(gccSettings.FileNamePostFix?.Trim()))
                    {
                        gccSettings.FileNamePostFix = "_GC";
                    }
                    if (string.IsNullOrEmpty(gccSettings.LineEnding?.Trim()))
                    {
                        gccSettings.LineEnding = "\n";
                    }
                    string strExtension = Path.GetExtension(strSourceFileName);
                    string strDestFilname = strSourceFileName.Replace(strExtension, $"{gccSettings.FileNamePostFix}{strExtension}");
                    Stopwatch sWatch = new Stopwatch();
                    try
                    {
                        Console.WriteLine("Processing File");
                        int nLines = 0;
                        int nMatches = 0;
                        sWatch.Start();
                        using (StreamReader sR = File.OpenText(strSourceFileName))
                        {
                            using (StreamWriter sW = File.CreateText(strDestFilname))
                            {
                                string? strLine = "";
                                (bool hasResult, string? strErg) vErg = new(false, null);
                                while ((strLine = sR.ReadLine()) != null)
                                {
                                    nLines++;
                                    foreach (LineDescription lD in gccSettings.LineDescriptions)
                                    {

                                        vErg = lD.CheckLine(strLine);
                                        if (vErg.hasResult)
                                        {
                                            break;
                                        }
                                    }
                                    if (!vErg.hasResult)    //no match
                                    {
                                        sW.Write($"{strLine}{gccSettings.LineEnding}");
                                    }
                                    else
                                    {
                                        Console.Write(".");
                                        nMatches++;
                                        if (vErg.strErg == null) //remove line
                                        {
                                            continue;
                                        }
                                        sW.Write($"{vErg.strErg}{gccSettings.LineEnding}");
                                    }
                                }
                            }
                        }
                        sWatch.Stop();
                        Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
                        Console.WriteLine($"File {Path.GetFileName(strSourceFileName)} processed:");
                        Console.WriteLine($"Number of Lines: {nLines}");
                        Console.WriteLine($"Number of Matches: {nMatches}");
                        Console.WriteLine($"Duration: {sWatch.Elapsed}");
                        Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
                    }
                    catch (LDException eX)
                    {
                        Console.WriteLine($"There is a problem with at least one LineDescription in appsettings.config{Environment.NewLine}{eX.Message}{Environment.NewLine}Use this as an example:");
                        PrintDefaultSettings();
                        return (0);
                    }
                    catch (Exception eX)
                    {
                        Console.WriteLine($"An error occured:{Environment.NewLine}{eX.Message}");
                        return (0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"There is a problem with appsettings.config {Environment.NewLine}{e.Message}");
                    return (0);
                }
                return (1);
            }
            finally
            {
                if (gccSettings?.WaitForKey ?? true)
                {
                    Console.WriteLine("Hit a key to continue");
                    Console.ReadKey();
                }
            }
        }
        private static void PrintDefaultSettings()
        {
            Console.WriteLine("{");
            Console.WriteLine(" \"FileNamePostFix\": \"_GC\",");
            Console.WriteLine(" \"LineEnding\": \"\\n\",");
            Console.WriteLine(" \"WaitForKey\": true,");
            Console.WriteLine(" \"LineDescriptions\": [");
            Console.WriteLine("    {");
            Console.WriteLine("      \"StartsWith\": \"M73 P\"");
            Console.WriteLine("      \"Contains\": \" R\",");
            Console.WriteLine("      \"EndsWith\": \"\",");
            Console.WriteLine("      \"Matches\": \"\",");
            Console.WriteLine("      \"RemoveLine\": false,");
            Console.WriteLine("      \"Comment\": \"commented by GCCLeaner\"");
            Console.WriteLine("     }");
            Console.WriteLine("  ]");
            Console.WriteLine("}");
        }
    }
}
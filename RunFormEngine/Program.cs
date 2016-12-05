using FormEngine;
using FormEngine.FileSystem;
using FormEngine.Interfaces;
using FormEngine.PdfFormBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FormEngine.CsvValues;
using NDesk.Options;

namespace RunFormEngine
{
    class Program
    {
        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: RunFormEngine [OPTIONS]");
            Console.WriteLine("Produce a pdf by combining plugins for data, layout and resources.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void Main(string[] args)
        {
            string formName = "";
            string outputFile = "";
            string valuesProviderDll = "";
            string valuesProviderClass = "";
            string valueKey = "";
            string formBuilderDll = "PdfFormBuilder";
            string resourcesDll = "FileSystem";
            string resourcesArgument = ".";

            bool invokePdf = false;
            bool runAsDeamon = false;

            bool ShowParamDescr = false;

            var p = new OptionSet() {
                { "f|form=", "The form {<name>} of the layout description.",
                  v => formName = v },
                { "d|valuesDll=", "Dll {<name>} of a binary with at least one implementation of IValuesProvider. If no value provider is given, the form will be generated with test data from the form definition.",
                  v => valuesProviderDll = v },
                { "c|valuesClass=", "The {<name>} of a class implementing IValuesProvider.",
                  v => valuesProviderClass = v },
                { "k|valueKey=", "The {<key>} value for the IValuesProvider to determine what data to provide.",
                  v => valueKey = v },
                { "r|resources=", "The {<name>} of a binary with an implementation of IResources. Default \"" + resourcesDll + "\".",
                  v => resourcesDll = v },
                { "a|resourcesArgument=", "An {<argument>} for the IResources implementation. Default \"" + resourcesArgument + "\".",
                  v => resourcesArgument = v },
                { "b|builder=", "Form builder dll {<name>}. Implementation of IFormBuilder. Default \"" + formBuilderDll + "\".",
                  v => formBuilderDll = v },
                { "o|output=", "the output file {<name>} for the output form. Default is form name.",
                  v => outputFile = v },
                { "i|invoke",  "Invoke the output file when finished.",
                  v => invokePdf = v != null },
                { "D|deamon",  "Keep on producing output whenever a file is touched in the current directory.",
                  v => runAsDeamon = v != null },
                { "h|help",  "show this message and exit.",
                  v => ShowParamDescr = v != null }
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("RunFormEngine: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `RunFormEngine --help' for more information.");
                return;
            }

            if (ShowParamDescr)
            {
                ShowHelp(p);
                return;
            }

            if (string.IsNullOrEmpty(formName) || string.IsNullOrEmpty(valueKey))
            {
                Console.Write("RunFormEngine: ");
                if (string.IsNullOrEmpty(formName))
                    Console.WriteLine("parameter --form is mandatory!");
                if (string.IsNullOrEmpty(valueKey))
                    Console.WriteLine("parameter --valueKey is mandatory!");
                Console.WriteLine();
                ShowHelp(p);
                return;
            }

            string outFileName;
            if (string.IsNullOrEmpty(outputFile))
                outFileName = Path.ChangeExtension(formName, "pdf");
            else
                outFileName = outputFile;

            Deamon deamon;

            MakePdf(formName, outFileName, valueKey, valuesProviderDll, valuesProviderClass, formBuilderDll, resourcesDll, resourcesArgument);

            if (invokePdf)
                Process.Start(outFileName);

            if (runAsDeamon)
            {
                deamon = new Deamon(formName, outFileName, valueKey, valuesProviderDll, valuesProviderClass, formBuilderDll, resourcesDll, resourcesArgument);
            }

            if(runAsDeamon)
                Console.ReadKey();
        }

        private class Deamon
        {
            private string formBuilderDll;
            private string formName;
            private string outFileName;
            private string resourcesArgument;
            private string resourcesDll;
            private string valueKey;
            private string valuesProviderClass;
            private string valuesProviderDll;
            private FileSystemWatcher watcher = new FileSystemWatcher();

            public Deamon(string formName, string outFileName, string valueKey, string valuesProviderDll, string valuesProviderClass, string formBuilderDll, string resourcesDll, string resourcesArgument)
            {
                this.formName = formName;
                this.outFileName = outFileName;
                this.valueKey = valueKey;
                this.valuesProviderDll = valuesProviderDll;
                this.valuesProviderClass = valuesProviderClass;
                this.formBuilderDll = formBuilderDll;
                this.resourcesDll = resourcesDll;
                this.resourcesArgument = resourcesArgument;

                watcher.Path = ".";
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.*";
                watcher.Changed += new FileSystemEventHandler(OnFileAccess);
                watcher.EnableRaisingEvents = true;
            }

            private void OnFileAccess(object sender, FileSystemEventArgs e)
            {
                watcher.EnableRaisingEvents = false;
                Thread.Sleep(300);
                Program.MakePdf(formName, outFileName, valueKey, valuesProviderDll, valuesProviderClass, formBuilderDll, resourcesDll, resourcesArgument);
                watcher.EnableRaisingEvents = true;
            }
        }

        private static void MakePdf(string formName, string outFileName, string valueKey, string valuesProviderDll, string valuesProviderClass, string formBuilderDll, string resourcesDll, string resourcesArgument)
        {
            try
            {
                IResources files = ClassFactory<IResources>.Instanciate(resourcesDll, "", resourcesArgument);
                Console.WriteLine("IResources: {0} ({1})", files.GetType().ToString(), resourcesArgument);

                IEnumerable<IValues> values = null;
                if (!string.IsNullOrWhiteSpace(valuesProviderDll) || !string.IsNullOrWhiteSpace(valuesProviderClass))
                {
                    IValuesProvider provider = ClassFactory<IValuesProvider>.Instanciate(valuesProviderDll, valuesProviderClass);
                    if (provider != null)
                    {
                        Console.WriteLine("IValuesProvider: {0} ({1})", provider.GetType().ToString(), valueKey);
                        values = provider.GetValues(files, valueKey);
                    }
                    else
                    {
                        Console.WriteLine("Error: values dll: \"{0}\" / values class: \"{1}\" could not be found!", valuesProviderDll, valuesProviderClass);
                        return;
                    }
                }

                bool ok = false;
                using (FileStream OutStream = new FileStream(outFileName, FileMode.Create))
                {
                    IFormBuilder builder = ClassFactory<IFormBuilder>.Instanciate(formBuilderDll, "", OutStream, files);
                    MakeForm form = new MakeForm(builder);
                    Console.WriteLine("IFormBuilder: {0}", builder.GetType().ToString());
                    ok = form.Execute(files, values, formName);
                    OutStream.Close();
                }
                Console.WriteLine("Produced " + outFileName + (ok ? " successfully!" : " with errors!"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }
    }
}

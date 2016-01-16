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

            Console.WriteLine();
            Console.WriteLine(@"RunFormEngine [-i] [-d] FormName=<form name> [ValueKey=<data key>] [OutputFile=<output file name>]
    IValuesProvider=<dll navn>
    IFiles=<dll navn>
    IFormBuilder=<dll navn>");

        }

        static void Main(string[] args)
        {
            string formName = "";
            string outputFile = "";

            bool invokePdf = false;
            bool runAsDeamon = false;

            bool ShowParamDescr = false;

            var p = new OptionSet() {
                { "d|deamon",  "keep on producing output whenever a file is touched in the resources.",
                  v => invokePdf = v != null },
                { "i|invoke",  "invoke the output file when finished.",
                  v => invokePdf = v != null },
                { "f|formName=", "the {FormName} of the layout description.",
                  v => formName = v },
                { "o|outputFile=",
                    "the output {FileName} for the produced .\n" +
                        "this must be an integer.",
                  v => outputFile = v },
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

            string outFileName;
            if (string.IsNullOrEmpty(outputFile))
                outFileName = Path.ChangeExtension(formName, "pdf");
            else
                outFileName = outputFile;

            Deamon deamon;

            MakePdf(formName, outFileName);

            if (invokePdf)
                Process.Start(outFileName);

            if (runAsDeamon)
            {
                deamon = new Deamon(formName, outFileName);
            }

            if(runAsDeamon)
                Console.ReadKey();
        }

        private class Deamon
        {
            private string formName;
            private string outFileName;
            private FileSystemWatcher watcher = new FileSystemWatcher();

            public Deamon(string formName, string outFileName)
            {
                this.formName = formName;
                this.outFileName = outFileName;
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
                Program.MakePdf(formName, outFileName);
                watcher.EnableRaisingEvents = true;
            }
        }

        private static void MakePdf(string formName, string outFileName)
        {
            try
            {
                IFiles files = new Folder(".");
                IEnumerable<IValues> values = new CsvFile().GetValues(files, "values.csv");
                bool ok = false;
                using (FileStream OutStream = new FileStream(outFileName, FileMode.Create))
                {
                    Document builder = new Document(OutStream, files);
                    MakeForm form = new MakeForm(files, values);
                    ok = form.Execute(formName, builder);
                    OutStream.Close();
                }
                Console.WriteLine("Produced " + outFileName + (ok ? " successfully!" : " with errors!"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}

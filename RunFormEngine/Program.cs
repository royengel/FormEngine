using FormEngine;
using FormEngine.FileSystem;
using FormEngine.Interfaces;
using FormEngine.XmlSpecification;
using FormEngine.PdfFormBuilder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RunFormEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            string formName = "";
            string outputFile = "";

            bool invokePdf = false;
            bool runAsDeamon = false;

            bool ShowParamDescr = false;
            bool ParamError = false;
            try
            {
                for (int i = 0; i <= args.GetUpperBound(0); i++)
                {
                    if (args[i].Length > 9 && args[i].Substring(0, 9).ToLower() == "formname=")
                        formName = args[i].Substring(9);
                    else if (args[i].Length > 11 && args[i].Substring(0, 11).ToLower() == "outputfile=")
                        outputFile = args[i].Substring(11);
                    else if (args[i].Length == 2 && args[i].Substring(0, 2).ToLower() == "-d")
                        runAsDeamon = true;
                    else if (args[i].Length == 2 && args[i].Substring(0, 2).ToLower() == "-i")
                        invokePdf = true;
                    else if (args[i].ToLower() == "-h")
                        ShowParamDescr = true;
                    else
                        ParamError = true;
                }
            }
            catch (Exception)
            {
                ParamError = true;
            }

            if (string.IsNullOrEmpty(formName))
                ParamError = true;

            if (ShowParamDescr || ParamError)
            {
                if (ParamError)
                    Console.WriteLine("Error: Invalid parameter!");

                Console.WriteLine("Usage:");
                Console.WriteLine("RunFormEngine [-i] [-d] FormName=<form name> [OutputFile=<output file name>]");

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
                Program.MakePdf(formName, outFileName);
            }
        }

        private static void MakePdf(string formName, string outFileName)
        {
            try
            {
                IFiles Files = new Folder(".");
                using (FileStream OutStream = new FileStream(outFileName, FileMode.Create))
                {
                    Document builder = new Document(OutStream, Files);
                    MakeForm form = new MakeForm(Files);
                    form.Execute(formName, builder);
                    OutStream.Close();
                }
                }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}

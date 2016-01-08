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

namespace RunFormEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            string formName = "";
            string outputFile = "";

            bool invokePdf = false;

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
                    else if (args[i].Length == 10 && args[i].Substring(0, 10).ToLower() == "/invokepdf")
                        invokePdf = true;
                    else if (args[i].ToLower() == "/h")
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
                Console.WriteLine("RunFormEngine [/InvokePdf] FormName=<form name> [OutputFile=<output file name>]");

                return;
            }

            try
            {
                IFiles Files = new Folder(".");

                string outFileName;
                if (string.IsNullOrEmpty(outputFile))
                    outFileName = Path.ChangeExtension(formName, "pdf");
                else
                    outFileName = outputFile;

                FileStream OutStream = new FileStream(outFileName, FileMode.Create);
                Document builder = new Document(OutStream, Files);

                MakeForm form = new MakeForm(Files);

                form.Execute(formName, builder);

                if (invokePdf)
                    Process.Start(outFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}

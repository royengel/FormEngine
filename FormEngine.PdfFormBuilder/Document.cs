using FormEngine.Interfaces;
using System.Collections.Generic;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace FormEngine.PdfFormBuilder
{
    public class Document : IFormBuilder
    {
        private Stream outStream;
        private PdfDocument document;
        private IResources files;
        private Dictionary<string, XImage> xImages = new Dictionary<string, XImage>();

        public Document(Stream stream, IResources files)
        {
            this.files = files;
            outStream = stream;
            document = new PdfDocument(stream);
        }

        public IFormPage AddPage(Interfaces.PageSize pageSize)
        {
            return new Page(this, document, pageSize);
        }

        public bool Finalize()
        {
            document.Close();
            return true;
        }
    }
}

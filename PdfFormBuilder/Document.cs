using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PdfSharp.Drawing;
using System.Drawing;
using PdfSharp.Pdf;
using PdfSharp;

namespace FormEngine.PdfFormBuilder
{
    public class Document : IFormBuilder
    {
        private Stream outStream;
        private PdfDocument document;
        private IFiles files;
        private Dictionary<string, XImage> xImages = new Dictionary<string, XImage>();

        public Document(Stream stream, IFiles files)
        {
            this.files = files;
            outStream = stream;
            document = new PdfDocument(stream);
        }

        public IFormPage AddPage(Interfaces.PageSize pageSize)
        {
            return new Page(this, document, pageSize);
        }

        private XImage MakeXImage(IFiles files, string fileName)
        {
            XImage xImg;
            if (!xImages.TryGetValue(fileName, out xImg))
            {
                object file = files.Get(fileName);
                Stream memStream = new MemoryStream((byte[])file);
                Image image = Image.FromStream(memStream);
                xImg = XImage.FromGdiPlusImage(image);
                xImages[fileName] = xImg;
            }
            return xImg;
        }

    }
}

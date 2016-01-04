using System;
using FormEngine.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace FormEngine.PdfFormBuilder
{
    internal class Page : IFormPage
    {
        private Document document;
        private PdfPage page;
        XGraphics xGraphics;

        public Page(Document document, PdfDocument pdfDocument, PageSize pageSize)
        {
            this.document = document;
            page = pdfDocument.AddPage();
            page.Size = ConvertToPdfSharpPageSize(pageSize);
            xGraphics = XGraphics.FromPdfPage(page);
        }

        public void AddImage(IFiles files, string file, decimal x, decimal y, decimal width, decimal height)
        {
            throw new NotImplementedException();
        }

        public void AddText(string fieldName, string text, string alignment, string font, decimal fontSize, FontStyle fontStyle, string colour, decimal x, decimal y, decimal width, decimal height)
        {
            XFontStyle style = ConvertFontStyle(fontStyle);
            XFont xFont = new XFont(font, (double) fontSize, style);
            XStringFormat format = new XStringFormat() { Alignment = XStringAlignment.Near };
            XRect rectangle = new XRect() { X = XUnit.FromMillimeter((double)x), Y = XUnit.FromMillimeter((double)y) };

            xGraphics.DrawString(text, xFont, XBrushes.Black, rectangle, format);
        }

        public decimal GetHeight()
        {
            return (decimal) page.Height.Millimeter;
        }

        public decimal GetWidth()
        {
            return (decimal) page.Width.Millimeter;
        }

        private PdfSharp.PageSize ConvertToPdfSharpPageSize(Interfaces.PageSize pageSize)
        {
            switch (pageSize)
            {
                case Interfaces.PageSize.A4:
                    return PdfSharp.PageSize.A4;
                case Interfaces.PageSize.Letter:
                    return PdfSharp.PageSize.Letter;
                default:
                    return PdfSharp.PageSize.Undefined;
            }
        }

        private XFontStyle ConvertFontStyle(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case Interfaces.FontStyle.Regular:
                    return XFontStyle.Regular;
                case Interfaces.FontStyle.Bold:
                    return XFontStyle.Bold;
                case Interfaces.FontStyle.Italic:
                    return XFontStyle.Italic;
                case Interfaces.FontStyle.BoldItalic:
                    return XFontStyle.BoldItalic;
                case Interfaces.FontStyle.Underline:
                    return XFontStyle.Underline;
                case Interfaces.FontStyle.Strikeout:
                    return XFontStyle.Strikeout;
                default:
                    return XFontStyle.Regular;
            }
        }

    }
}
﻿using System;
using FormEngine.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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

        public void AddImage(IResources files, string file, decimal x, decimal y, decimal width, decimal height)
        {
            XImage xImg = MakeXImage(files, file);
            XRect rectangle = new XRect() { X = XUnit.FromCentimeter((double)x), Y = XUnit.FromCentimeter((double)y), Width = XUnit.FromCentimeter((double)width), Height = XUnit.FromCentimeter((double)height) };
            xGraphics.DrawImage(xImg, rectangle);
        }

        private static Dictionary<string, XImage> imageCache = new Dictionary<string, XImage>();
        private static XImage MakeXImage(IResources files, string file)
        {
            XImage xImg;
            if (!imageCache.TryGetValue(file, out xImg))
            {
                byte[] imgFile = files.Get(file);
                Stream imgStream = new MemoryStream(imgFile);
                xImg = XImage.FromStream(imgStream);
                imageCache[file] = xImg;
            }
            return xImg;
        }

        public void AddText(string fieldName, string text, string alignment, string font, decimal fontSize, Interfaces.FontStyle fontStyle, ColourName colour, decimal x, decimal y, decimal width, decimal height)
        {
            if (string.IsNullOrEmpty(text))
                return;

            XFontStyle style = ConvertFontStyle(fontStyle);
            XFont xFont = new XFont(font, (double) fontSize, style);
            XBrush brush = CreateXBrush(colour);
            XStringFormat format = new XStringFormat() { Alignment = XStringAlignment.Center };
            XSize size = xGraphics.MeasureString("I", xFont);
            double lineHeight = size.Height;
            double currentY = 0;
            double yOffset = XUnit.FromCentimeter((double)y);
            double XUnitHeight = XUnit.FromCentimeter((double)height);

            List<string> lines;
            if (width > 0)
                lines = SplitText(text, xFont, style, XUnit.FromCentimeter((double)width)).ToList();
            else
            {
                lines = new List<string>() { text };
                width = GetWidth() - x; 
            }
            foreach (string line in lines)
            {
                if (height <= 0 || currentY + lineHeight < XUnitHeight)
                {
                    XRect rectangle = new XRect() { X = XUnit.FromCentimeter((double)x), Y = yOffset + currentY };
                    //xGraphics.DrawString(line, xFont, brush, rectangle, format);

                    XTextFormatter textFormatter = new XTextFormatter(xGraphics);

                    rectangle.Width = XUnit.FromCentimeter((double)width);
                    rectangle.Height = lineHeight;
                    textFormatter.Alignment = GetXParagraphAlignment(alignment);

                    textFormatter.DrawString(line, xFont, brush, rectangle, XStringFormats.TopLeft);

                    currentY += lineHeight;
                }
            }
        }

        public decimal MeasureTextHeight(string text, string font, decimal fontSize, Interfaces.FontStyle fontStyle, decimal width, decimal height)
        {
            if (height > 0)
                return height;

            if (string.IsNullOrEmpty(text))
                return 0;

            XFontStyle style = ConvertFontStyle(fontStyle);
            XFont xFont = new XFont(font, (double)fontSize, style);
            XSize size = xGraphics.MeasureString("I", xFont);
            double lineHeight = size.Height;
            int lineCount = 1;
            if (width > 0M)
            {
                lineCount = SplitText(text, xFont, style, XUnit.FromCentimeter((double)width)).Count();
            }
            XUnit xHeight = XUnit.FromPoint(lineCount * lineHeight);
            return (decimal)xHeight.Centimeter;
        }

        private IEnumerable<string> SplitText(string text, XFont xFont, XFontStyle style, double width)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var lines = text.Split('\n');
                foreach (string line in lines)
                {
                    string rest = line;
                    while (rest.Length > 0)
                    {
                        XSize size = xGraphics.MeasureString(rest, xFont);
                        if (size.Width > width)
                        {
                            yield return LargestPossibleText(rest, xFont, style, width, out rest);
                        }
                        else
                        {
                            yield return rest;
                            rest = "";
                        }
                    }
                }
            }
        }

        private string LargestPossibleText(string line, XFont xFont, XFontStyle style, double width, out string rest)
        {
            char[] splitChars = new char[] { ' ', '-', '/' };
            int p = 0;
            int n;
            while ((n = line.IndexOfAny(splitChars, p + 1)) >= 0)
            {
                XSize size = xGraphics.MeasureString(line.Substring(0, n), xFont);
                if (size.Width > width)
                {
                    if (p == 0)
                        p = n;
                    rest = line.Substring(p).TrimStart();
                    return line.Substring(0, p);
                }
                p = n;
            }
            rest = "";
            return line;
        }

        private XBrush CreateXBrush(ColourName colour)
        {
            return new XSolidBrush(MapColourNameToXColor(colour));
        }

        private XColor MapColourNameToXColor(ColourName colourName)
        {
            switch(colourName)
            {
                case ColourName.AntiqueWhite:
                    return XColors.AntiqueWhite;
                case ColourName.Aqua:
                    return XColors.Aqua;
                case ColourName.Aquamarine:
                    return XColors.Aquamarine;
                case ColourName.Azure:
                    return XColors.Azure;
                case ColourName.Beige:
                    return XColors.Beige;
                case ColourName.Bisque:
                    return XColors.Bisque;
                case ColourName.Black:
                    return XColors.Black;
                case ColourName.BlanchedAlmond:
                    return XColors.BlanchedAlmond;
                case ColourName.Blue:
                    return XColors.Blue;
                case ColourName.BlueViolet:
                    return XColors.BlueViolet;
                case ColourName.Brown:
                    return XColors.Brown;
                case ColourName.BurlyWood:
                    return XColors.BurlyWood;
                case ColourName.CadetBlue:
                    return XColors.CadetBlue;
                case ColourName.Chartreuse:
                    return XColors.Chartreuse;
                case ColourName.Chocolate:
                    return XColors.Chocolate;
                case ColourName.Coral:
                    return XColors.Coral;
                case ColourName.CornflowerBlue:
                    return XColors.CornflowerBlue;
                case ColourName.Cornsilk:
                    return XColors.Cornsilk;
                case ColourName.Crimson:
                    return XColors.Crimson;
                case ColourName.Cyan:
                    return XColors.Cyan;
                case ColourName.DarkBlue:
                    return XColors.DarkBlue;
                case ColourName.DarkCyan:
                    return XColors.DarkCyan;
                case ColourName.DarkGoldenrod:
                    return XColors.DarkGoldenrod;
                case ColourName.DarkGray:
                    return XColors.DarkGray;
                case ColourName.DarkGreen:
                    return XColors.DarkGreen;
                case ColourName.DarkKhaki:
                    return XColors.DarkKhaki;
                case ColourName.DarkMagenta:
                    return XColors.DarkMagenta;
                case ColourName.DarkOliveGreen:
                    return XColors.DarkOliveGreen;
                case ColourName.DarkOrange:
                    return XColors.DarkOrange;
                case ColourName.DarkOrchid:
                    return XColors.DarkOrchid;
                case ColourName.DarkRed:
                    return XColors.DarkRed;
                case ColourName.DarkSalmon:
                    return XColors.DarkSalmon;
                case ColourName.DarkSeaGreen:
                    return XColors.DarkSeaGreen;
                case ColourName.DarkSlateBlue:
                    return XColors.DarkSlateBlue;
                case ColourName.DarkSlateGray:
                    return XColors.DarkSlateGray;
                case ColourName.DarkTurquoise:
                    return XColors.DarkTurquoise;
                case ColourName.DarkViolet:
                    return XColors.DarkViolet;
                case ColourName.DeepPink:
                    return XColors.DeepPink;
                case ColourName.DeepSkyBlue:
                    return XColors.DeepSkyBlue;
                case ColourName.DimGray:
                    return XColors.DimGray;
                case ColourName.DodgerBlue:
                    return XColors.DodgerBlue;
                case ColourName.Firebrick:
                    return XColors.Firebrick;
                case ColourName.FloralWhite:
                    return XColors.FloralWhite;
                case ColourName.ForestGreen:
                    return XColors.ForestGreen;
                case ColourName.Fuchsia:
                    return XColors.Fuchsia;
                case ColourName.Gainsboro:
                    return XColors.Gainsboro;
                case ColourName.GhostWhite:
                    return XColors.GhostWhite;
                case ColourName.Gold:
                    return XColors.Gold;
                case ColourName.Goldenrod:
                    return XColors.Goldenrod;
                case ColourName.Gray:
                    return XColors.Gray;
                case ColourName.Green:
                    return XColors.Green;
                case ColourName.GreenYellow:
                    return XColors.GreenYellow;
                case ColourName.Honeydew:
                    return XColors.Honeydew;
                case ColourName.HotPink:
                    return XColors.HotPink;
                case ColourName.IndianRed:
                    return XColors.IndianRed;
                case ColourName.Indigo:
                    return XColors.Indigo;
                case ColourName.Ivory:
                    return XColors.Ivory;
                case ColourName.Khaki:
                    return XColors.Khaki;
                case ColourName.Lavender:
                    return XColors.Lavender;
                case ColourName.LavenderBlush:
                    return XColors.LavenderBlush;
                case ColourName.LawnGreen:
                    return XColors.LawnGreen;
                case ColourName.LemonChiffon:
                    return XColors.LemonChiffon;
                case ColourName.LightBlue:
                    return XColors.LightBlue;
                case ColourName.LightCoral:
                    return XColors.LightCoral;
                case ColourName.LightCyan:
                    return XColors.LightCyan;
                case ColourName.LightGoldenrodYellow:
                    return XColors.LightGoldenrodYellow;
                case ColourName.LightGray:
                    return XColors.LightGray;
                case ColourName.LightGreen:
                    return XColors.LightGreen;
                case ColourName.LightPink:
                    return XColors.LightPink;
                case ColourName.LightSalmon:
                    return XColors.LightSalmon;
                case ColourName.LightSeaGreen:
                    return XColors.LightSeaGreen;
                case ColourName.LightSkyBlue:
                    return XColors.LightSkyBlue;
                case ColourName.LightSlateGray:
                    return XColors.LightSlateGray;
                case ColourName.LightSteelBlue:
                    return XColors.LightSteelBlue;
                case ColourName.LightYellow:
                    return XColors.LightYellow;
                case ColourName.Lime:
                    return XColors.Lime;
                case ColourName.LimeGreen:
                    return XColors.LimeGreen;
                case ColourName.Linen:
                    return XColors.Linen;
                case ColourName.Magenta:
                    return XColors.Magenta;
                case ColourName.Maroon:
                    return XColors.Maroon;
                case ColourName.MediumAquamarine:
                    return XColors.MediumAquamarine;
                case ColourName.MediumBlue:
                    return XColors.MediumBlue;
                case ColourName.MediumOrchid:
                    return XColors.MediumOrchid;
                case ColourName.MediumPurple:
                    return XColors.MediumPurple;
                case ColourName.MediumSeaGreen:
                    return XColors.MediumSeaGreen;
                case ColourName.MediumSlateBlue:
                    return XColors.MediumSlateBlue;
                case ColourName.MediumSpringGreen:
                    return XColors.MediumSpringGreen;
                case ColourName.MediumTurquoise:
                    return XColors.MediumTurquoise;
                case ColourName.MediumVioletRed:
                    return XColors.MediumVioletRed;
                case ColourName.MidnightBlue:
                    return XColors.MidnightBlue;
                case ColourName.MintCream:
                    return XColors.MintCream;
                case ColourName.MistyRose:
                    return XColors.MistyRose;
                case ColourName.Moccasin:
                    return XColors.Moccasin;
                case ColourName.NavajoWhite:
                    return XColors.NavajoWhite;
                case ColourName.Navy:
                    return XColors.Navy;
                case ColourName.OldLace:
                    return XColors.OldLace;
                case ColourName.Olive:
                    return XColors.Olive;
                case ColourName.OliveDrab:
                    return XColors.OliveDrab;
                case ColourName.Orange:
                    return XColors.Orange;
                case ColourName.OrangeRed:
                    return XColors.OrangeRed;
                case ColourName.Orchid:
                    return XColors.Orchid;
                case ColourName.PaleGoldenrod:
                    return XColors.PaleGoldenrod;
                case ColourName.PaleGreen:
                    return XColors.PaleGreen;
                case ColourName.PaleTurquoise:
                    return XColors.PaleTurquoise;
                case ColourName.PaleVioletRed:
                    return XColors.PaleVioletRed;
                case ColourName.PapayaWhip:
                    return XColors.PapayaWhip;
                case ColourName.PeachPuff:
                    return XColors.PeachPuff;
                case ColourName.Peru:
                    return XColors.Peru;
                case ColourName.Pink:
                    return XColors.Pink;
                case ColourName.Plum:
                    return XColors.Plum;
                case ColourName.PowderBlue:
                    return XColors.PowderBlue;
                case ColourName.Purple:
                    return XColors.Purple;
                case ColourName.Red:
                    return XColors.Red;
                case ColourName.RosyBrown:
                    return XColors.RosyBrown;
                case ColourName.RoyalBlue:
                    return XColors.RoyalBlue;
                case ColourName.SaddleBrown:
                    return XColors.SaddleBrown;
                case ColourName.Salmon:
                    return XColors.Salmon;
                case ColourName.SandyBrown:
                    return XColors.SandyBrown;
                case ColourName.SeaGreen:
                    return XColors.SeaGreen;
                case ColourName.SeaShell:
                    return XColors.SeaShell;
                case ColourName.Sienna:
                    return XColors.Sienna;
                case ColourName.Silver:
                    return XColors.Silver;
                case ColourName.SkyBlue:
                    return XColors.SkyBlue;
                case ColourName.SlateBlue:
                    return XColors.SlateBlue;
                case ColourName.SlateGray:
                    return XColors.SlateGray;
                case ColourName.Snow:
                    return XColors.Snow;
                case ColourName.SpringGreen:
                    return XColors.SpringGreen;
                case ColourName.SteelBlue:
                    return XColors.SteelBlue;
                case ColourName.Tan:
                    return XColors.Tan;
                case ColourName.Teal:
                    return XColors.Teal;
                case ColourName.Thistle:
                    return XColors.Thistle;
                case ColourName.Tomato:
                    return XColors.Tomato;
                case ColourName.Transparent:
                    return XColors.Transparent;
                case ColourName.Turquoise:
                    return XColors.Turquoise;
                case ColourName.Violet:
                    return XColors.Violet;
                case ColourName.Wheat:
                    return XColors.Wheat;
                case ColourName.White:
                    return XColors.White;
                case ColourName.WhiteSmoke:
                    return XColors.WhiteSmoke;
                case ColourName.Yellow:
                    return XColors.Yellow;
                case ColourName.YellowGreen:
                    return XColors.YellowGreen;
                case ColourName.AliceBlue:
                    return XColors.AliceBlue;
                default:
                    return XColors.Black;
            }
        }

        private XParagraphAlignment GetXParagraphAlignment(string alignment)
        {
            switch (alignment.ToLower())
            {
                case "center":
                    return XParagraphAlignment.Center;
                case "right":
                    return XParagraphAlignment.Right;
                case "default":
                    return XParagraphAlignment.Default;
                case "justify":
                    return XParagraphAlignment.Justify;
                default:
                    return XParagraphAlignment.Left;
            }
        }

        public decimal GetHeight()
        {
            return (decimal) page.Height.Centimeter;
        }

        public decimal GetWidth()
        {
            return (decimal) page.Width.Centimeter;
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

        private XFontStyle ConvertFontStyle(Interfaces.FontStyle fontStyle)
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
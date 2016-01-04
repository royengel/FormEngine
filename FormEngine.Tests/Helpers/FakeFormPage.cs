using FormEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Tests.Helpers
{
    public class FormImage
    {
        public string file;
        public decimal x;
        public decimal y;
        public decimal width;
        public decimal height;

        public FormImage(string file, decimal x, decimal y, decimal width, decimal height)
        {
            this.file = file;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public class FormText
    {
        public string fieldName;
        public string alignment;
        public string font;
        public decimal fontSize;
        public string fontStyle;
        public string colour;
        public decimal height;
        public string text;
        public decimal width;
        public decimal x;
        public decimal y;

        public FormText(string fieldName, string text, string alignment, string font, decimal fontSize, string fontStyle, string colour, decimal x, decimal y, decimal width, decimal height)
        {
            this.fieldName = fieldName;
            this.text = text;
            this.alignment = alignment;
            this.font = font;
            this.fontSize = fontSize;
            this.fontStyle = fontStyle;
            this.colour = colour;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public class FakeFormPage : IFormPage
    {
        public string pageSize;
        public List<FormImage> images = new List<FormImage>();
        public List<FormText> texts = new List<FormText>();

        public FakeFormPage(string pageSize)
        {
            this.pageSize = pageSize;
        }

        public void AddImage(IFiles files, string file, decimal x, decimal y, decimal width, decimal height)
        {
            images.Add(new FormImage(file, x, y, width, height));
        }

        public void AddText(string fieldName, string text, string alignment, string font, decimal FontSize, FontStyle fontStyle, string colour, decimal x, decimal y, decimal width, decimal height)
        {
            texts.Add(new FormText(fieldName, text, alignment, font, FontSize, fontStyle.ToString(), colour, x, y, width, height));
        }

        public decimal GetWidth()
        {
            return 21;
        }

        public decimal GetHeight()
        {
            return 27;
        }
    }
}

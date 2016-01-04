using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine.Interfaces
{
    public interface IFormPage
    {
        void AddImage(IFiles files, string file, decimal x, decimal y, decimal width, decimal height);
        void AddText(string fieldName, string text, string alignment, string font, decimal fontSize, FontStyle fontStyle, string colour, decimal x, decimal y, decimal width, decimal height);
        decimal GetWidth();
        decimal GetHeight();
    }
}

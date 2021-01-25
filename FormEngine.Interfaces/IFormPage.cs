namespace FormEngine.Interfaces
{
    public interface IFormPage
    {
        void AddImage(IResources files, string file, decimal x, decimal y, decimal width, decimal height);
        void AddText(string fieldName, string text, string alignment, string font, decimal fontSize, FontStyle fontStyle, ColourName colour, decimal x, decimal y, decimal width, decimal height);
        decimal MeasureTextHeight(string text, string font, decimal fontSize, FontStyle fontStyle, decimal width, decimal height);
        decimal GetWidth();
        decimal GetHeight();
    }
}

using System.Collections.Generic;

namespace FormEngine.Interfaces
{
    public class Section : FieldProperties
    {
        public List<string> breakColumns;
        public List<Field> fields;
        public List<Image> images;
        public decimal shiftX;
        public decimal shiftY;
    }
}
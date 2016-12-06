using System;

namespace FormEngine.Interfaces
{
    public class Field : FieldProperties
    {
        public string name;
        public Func<dynamic, string> value;
        public decimal width;
        public decimal height;
        public string testValue;
    }
}
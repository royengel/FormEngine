using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace FormEngine.Interfaces
{
    public enum SectionType { FormHeader, FormFooter, PageHeader, PageFooter, GroupHeader, GroupFooter, DetailHeader, Detail }

    public class Section : FieldProperties
    {
        private SectionType _sectionType = SectionType.FormHeader;
        [JsonConverter(typeof(StringEnumConverter))]
        public SectionType sectionType { get { return _sectionType; } set { _sectionType = value; } }
        public bool isHeader = true;
        public bool pageBreak = false;
        public List<Func<dynamic, object>> breakColumns;
        public List<Field> fields;
        public List<Image> images;
        public decimal width;
        public decimal? height;
    }
}
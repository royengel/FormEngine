using System;
using System.Collections.Generic;

namespace FormEngine.Interfaces
{
    public enum SectionType { FormHeader, FormFooter, PageHeader, PageFooter, GroupHeader, GroupFooter, DetailHeader, Detail }

    public class Section : FieldProperties
    {
        public SectionType sectionType = SectionType.FormHeader;
        public bool isHeader = true;
        public bool pageBreak = false;
        public List<Func<dynamic, object>> breakColumns;
        public List<Field> fields;
        public List<Image> images;
        public decimal width;
        public decimal? height;
    }
}
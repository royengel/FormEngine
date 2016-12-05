using FormEngine.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormEngine
{
    public class MakeForm
    {
        private IResources files;
        private IEnumerable<IValues> values;

        private IFormBuilder builder = null;
        private IFormPage currentPage = null;
        private decimal currentVerticalPosition = 0;
        private ValueIterator valueIterator = null;

        public MakeForm(IFormBuilder builder)
        {
            this.builder = builder;
        }

        public bool Execute(IResources files, IEnumerable<IValues> values, string form)
        {
            Form formSpec;
            try
            {
                string json = files.GetText(form + ".json");
                formSpec = JsonConvert.DeserializeObject<Form>(json);
            }
            catch(Exception ex)
            {
                ExceptionPdf(ex);

                return false;
            }

            try
            {
                Execute(files, values, formSpec);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionPdf(ex);

                return false;
            }
        }

        public void Execute(IResources files, IEnumerable<IValues> values, Form formSpec, bool finalize = true)
        {
            this.files = files;
            this.values = values;
            //string test = JsonConvert.SerializeObject(formSpec);
            if (this.values == null)
                this.values = new List<IValues>() { new TestValues(formSpec) };

            foreach (Report r in formSpec.reports)
            {
                ExecuteReport(formSpec, r);
            }
            if(finalize)
                builder.Finalize();
        }

        private void ExceptionPdf(Exception ex)
        {
            Form formSpec = new Form()
            {
                formTitle = "Internal error",
                pageSize = PageSize.A4,
                reports = new List<Report>() {
                        new Report() { 
                            sections = new List<Section>() {
                                new Section() {
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 2, y = 2, width=18, name = "exception", colour = ColourName.Red, font = "Arial", fontSize = 10 }
                                    }
                                }
                            }
                        }
                    }
            };
            IEnumerable<IValues> values = new List<IValues> { new Values(new Dictionary<string, object> { { "exception", ex.ToString() } }) };
            Execute(files, values, formSpec);
        }

        private void ExecuteReport(Form formSpec, Report report)
        {
            valueIterator = new ValueIterator();
            foreach (IValues v in values)
            {
                valueIterator.IterateTo(v);

                ExecuteReport(formSpec, report, v);
            }
        }

        private SectionType lastExecutedSectionType = SectionType.FormHeader;
        private IFormPage lastPageWithDetailHeader = null;
        private void ExecuteReport(Form formSpec, Report report, IValues v)
        {
            if (valueIterator.IsFirst())
                foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.FormHeader))
                    HandleSection(formSpec, report, s, v);

            foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.GroupHeader))
                HandleGroupHeaderSection(formSpec, report, s, v);

            foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.Detail))
                HandleSection(formSpec, report, s, v);

            foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.GroupFooter))
                HandleGroupFooterSection(formSpec, report, s, v);

            if (valueIterator.IsLast())
                foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.FormFooter))
                    HandleSection(formSpec, report, s, valueIterator.PreviousValues());
        }

        private void HandleGroupHeaderSection(Form formSpec, Report report, Section section, IValues v)
        {
            if (valueIterator.IsBreak(section.breakColumns, true))
            {
                HandlePageBreaks(formSpec, report, section, v);
                ExecuteSection(formSpec, report, section, v);
            }
        }

        private void HandleSection(Form formSpec, Report report, Section section, IValues v)
        {
            HandlePageBreaks(formSpec, report, section, v);

            if (lastPageWithDetailHeader != currentPage || lastExecutedSectionType != SectionType.Detail)
            {
                foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.DetailHeader))
                    ExecuteSection(formSpec, report, s, v);
                lastPageWithDetailHeader = currentPage;
            }

            ExecuteSection(formSpec, report, section, v);
        }

        private void HandleGroupFooterSection(Form formSpec, Report report, Section section, IValues v)
        {
            if (valueIterator.IsBreak(section.breakColumns, false))
            {
                HandlePageBreaks(formSpec, report, section, v);
                ExecuteSection(formSpec, report, section, v);
            }
        }

        private void HandlePageBreaks(Form formSpec, Report report, Section section, IValues values)
        {
            if (currentPage == null 
                || section.pageBreak
                || currentVerticalPosition + CalculateSectionHeight(formSpec, report, section, values) > currentPage.GetHeight())
            {
                if(currentPage != null)
                    foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.PageFooter))
                        ExecuteSection(formSpec, report, s, valueIterator.PreviousValues());

                currentPage = builder.AddPage(formSpec.pageSize);
                currentVerticalPosition = formSpec.y + report.y;

                foreach (Section s in report.sections.Where(s => s.sectionType == SectionType.PageHeader))
                    ExecuteSection(formSpec, report, s, values);
            }
        }

        private void ExecuteSection(Form formSpec, Report report, Section section, IValues values)
        {
            if (section.images != null)
                foreach (Image image in section.images)
                    ExecuteImage(formSpec, report, section, image, values);

            if (section.fields != null)
                foreach (Field field in section.fields)
                    ExecuteField(formSpec, report, section, field, values);

            lastExecutedSectionType = section.sectionType;
            currentVerticalPosition += CalculateSectionHeight(formSpec, report, section, values);
        }

        private decimal CalculateSectionHeight(Form formSpec, Report report, Section section, IValues values)
        {
            if (section.height != null)
                return section.height ?? 0;

            decimal maxHeight = 0;
            if (section.images != null)
                foreach (Image image in section.images)
                    maxHeight = Max(maxHeight, image.y + image.height);
            if (section.fields != null)
                foreach (Field field in section.fields)
                    maxHeight = Max(maxHeight, field.y + MeasureFieldTextHeight(formSpec, report, section, field, values));

            return maxHeight;
        }

        private T Max<T>(T v1, T v2) where T : IComparable
        {
            if (v1.CompareTo(v2) >= 0)
                return v1;
            return v2;
        }

        private T Min<T>(T v1, T v2) where T : IComparable
        {
            if (v1.CompareTo(v2) <= 0)
                return v1;
            return v2;
        }

        private void ExecuteImage(Form formSpec, Report report, Section section, Image image, IValues v)
        {
            try
            {
                decimal x = CalculateXCoordinate(formSpec.x, report.x, section.x, image.x);
                decimal y = CalculateYCoordinate(formSpec.y, report.y, section.y, image.y, currentVerticalPosition);
                decimal width = Min(image.width, currentPage.GetWidth() - x);
                decimal height = Min(image.height, currentPage.GetHeight() - y);
                currentPage.AddImage(files, image.name, x, y, width, height);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when processing image: " + image.name, ex);
            }
        }

        private decimal MeasureFieldTextHeight(Form formSpec, Report page, Section section, Field field, IValues v)
        {
            try
            {
                string text = FieldText(field, v);
                FieldProperties p = GetFieldDefaults(formSpec, page, section, field);
                return currentPage.MeasureTextHeight(text, p.font, p.fontSize, p.fontStyle, field.width, field.height);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when processing field: " + field.name, ex);
            }
        }

        private void ExecuteField(Form formSpec, Report page, Section section, Field field, IValues v)
        {
            try
            {
                string text = FieldText(field, v);
                FieldProperties p = GetFieldDefaults(formSpec, page, section, field);
                currentPage.AddText(field.name, text, p.alignment, p.font, p.fontSize, p.fontStyle, p.colour, p.x, p.y, field.width, field.height);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when processing field: " + field.name, ex);
            }
        }

        private static string FieldText(Field field, IValues v)
        {
            string text = field.value;
            if (string.IsNullOrEmpty(text))
                text = v.Get(field.name, field.format);
            return text;
        }

        private FieldProperties GetFieldDefaults(Form formSpec, Report report, Section section, Field field)
        {
            FieldProperties p = new FieldProperties();
            p.alignment = field.alignment ?? section.alignment ?? report.alignment ?? formSpec.alignment ?? "Left";

            p.colour = field.colour != ColourName.Undefined ? field.colour
                    : (section.colour != ColourName.Undefined ? section.colour
                    : (report.colour != ColourName.Undefined ? report.colour
                    : (formSpec.colour != ColourName.Undefined ? formSpec.colour
                    : ColourName.Black)));

            p.font = field.font ?? section.font ?? report.font ?? formSpec.font ?? "Arial";
            p.fontSize = field.fontSize != 0 ? field.fontSize 
                    : (section.fontSize != 0 ? section.fontSize 
                    : (report.fontSize != 0 ? report.fontSize
                    : (formSpec.fontSize != 0 ? formSpec.fontSize
                    : 12)));
            p.fontStyle = field.fontStyle != FontStyle.Undefined ? field.fontStyle 
                    : (section.fontStyle != FontStyle.Undefined ? section.fontStyle 
                    : (report.fontStyle != FontStyle.Undefined ? report.fontStyle 
                    : (formSpec.fontStyle != FontStyle.Undefined ? formSpec.fontStyle
                    : FontStyle.Regular)));

            p.x = CalculateXCoordinate(formSpec.x, report.x, section.x, field.x);
            p.y = CalculateYCoordinate(formSpec.y, report.y, section.y, field.y, currentVerticalPosition);

            return p;
        }

        private decimal CalculateXCoordinate(decimal formOffset, decimal reportOffset, decimal sectionOffset, decimal itemCoordinate)
        {
            return formOffset + reportOffset + sectionOffset + itemCoordinate;
        }

        private decimal CalculateYCoordinate(decimal formOffset, decimal reportOffset, decimal sectionOffset, decimal itemCoordinate, decimal currentPosition)
        {
            return sectionOffset + itemCoordinate + currentPosition;
        }

        //private void WriteException(Exception ex, string message)
        //{
        //    if(currentPage == null)
        //        currentPage = builder.AddPage(PageSize.A4);

        //    currentPage.AddText("Exception", message + Environment.NewLine + ex.ToString(), "Left", "Impact", 12, FontStyle.Bold, ColourName.Black, 2, 2, currentPage.GetWidth() - 4, currentPage.GetHeight() - 4);
        //}
    }
}

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
        private IFiles files;
        private IEnumerable<IValues> values;

        private IFormBuilder builder = null;
        private IFormPage currentPage = null;
        private BreakChecker breakChecker = new BreakChecker();

        public MakeForm(IFiles files)
        {
            this.files = files;
            this.values = null;
        }


        public MakeForm(IFiles files, IEnumerable<IValues> values)
        {
            this.files = files;
            this.values = values;
        }

        public bool Execute(string form, IFormBuilder builder)
        {
            this.builder = builder;
            currentPage = null;
            Form formSpec = null;
            bool ok = true;
            try
            {
                string json = files.GetText(form + ".json");
                formSpec = JsonConvert.DeserializeObject<Form>(json);
            }
            catch(Exception ex)
            {
                formSpec = new Form()
                {
                    formTitle = "Internal error",
                    pages = new List<Page>() {
                        new Page() { pageSize = PageSize.A4,
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
                values = new List<IValues> { new Values(new Dictionary<string, object> { { "exception", ex.ToString() } }) };

                ok = false;
            }

            return Execute(formSpec, builder) && ok;
        }

        public bool Execute(Form formSpec, IFormBuilder builder)
        {
            bool ok = true;
            this.builder = builder;
            currentPage = null;
            if (values == null)
                values = new List<IValues> () { new TestValues(formSpec)};

            foreach (IValues v in values)
            {
                breakChecker.IterateTo(v);

                if (!ExecuteForm(formSpec, v))
                {
                    ok = false;
                    break;
                }
            }
            return builder.Finalize() && ok;
        }

        private bool ExecuteForm(Form formSpec, IValues v)
        {
            foreach(Page page in formSpec.pages)
                if (!ExecutePage(formSpec, page, v))
                    return false;

            return true;
        }
        long interationOnCurrentPage = 0;
        private bool ExecutePage(Form formSpec, Page page, IValues v)
        {
            if (breakChecker.IsBreak(page.breakColumns))
            {
                currentPage = builder.AddPage(page.pageSize);

                if (!string.IsNullOrEmpty(page.backgroundImage))
                    currentPage.AddImage(files, page.backgroundImage, 0, 0, currentPage.GetWidth(), currentPage.GetHeight());

                interationOnCurrentPage = 0;
            }
            else
            {
                interationOnCurrentPage++;
            }
            if (page.sections != null)
                foreach (Section section in page.sections)
                    if (!ExecuteSection(formSpec, page, section, v))
                        return false;

            return true;
        }

        private bool ExecuteSection(Form formSpec, Page page, Section section, IValues v)
        {
            if (interationOnCurrentPage == 0 || breakChecker.IsBreak(section.breakColumns))
            {
                if (section.images != null)
                    foreach (Image image in section.images)
                        if (!ExecuteImage(formSpec, page, section, image, v))
                            return false;

                if (section.fields != null)
                    foreach (Field field in section.fields)
                        if (!ExecuteField(formSpec, page, section, field, v))
                            return false;
            }
            return true;
        }

        private bool ExecuteImage(Form formSpec, Page page, Section section, Image image, IValues v)
        {
            try
            {
                decimal x = CalculateCoordinate(formSpec.x, page.x, section.x, interationOnCurrentPage, section.shiftX, image.x);
                decimal y = CalculateCoordinate(formSpec.y, page.y, section.y, interationOnCurrentPage, section.shiftY, image.y);
                currentPage.AddImage(files, image.name, x, y, image.width, image.height);
            }
            catch (Exception ex)
            {
                WriteException(ex, "Error when processing image: " + image.name);
                return false;
            }
            return true;
        }

        private bool ExecuteField(Form formSpec, Page page, Section section, Field field, IValues v)
        {
            try
            {
                string text = v.Get(field.name, field.format);
                FieldProperties p = GetFieldDefaults(formSpec, page, section, field);
                currentPage.AddText(field.name, text, p.alignment, p.font, p.fontSize, p.fontStyle, p.colour, p.x, p.y, field.width, field.height);
            }
            catch(Exception ex)
            {
                WriteException(ex, "Error when processing field: " + field.name);
                return false;
            }
            return true;
        }

        private FieldProperties GetFieldDefaults(Form formSpec, Page page, Section section, Field field)
        {
            FieldProperties p = new FieldProperties();
            p.alignment = field.alignment ?? section.alignment ?? page.alignment ?? formSpec.alignment ?? "Left";

            p.colour = field.colour != ColourName.Undefined ? field.colour
                    : (section.colour != ColourName.Undefined ? section.colour
                    : (page.colour != ColourName.Undefined ? page.colour
                    : (formSpec.colour != ColourName.Undefined ? formSpec.colour
                    : ColourName.Black)));

            p.font = field.font ?? section.font ?? page.font ?? formSpec.font ?? "Arial";
            p.fontSize = field.fontSize != 0 ? field.fontSize 
                    : (section.fontSize != 0 ? section.fontSize 
                    : (page.fontSize != 0 ? page.fontSize
                    : (formSpec.fontSize != 0 ? formSpec.fontSize
                    : 12)));
            p.fontStyle = field.fontStyle != FontStyle.Undefined ? field.fontStyle 
                    : (section.fontStyle != FontStyle.Undefined ? section.fontStyle 
                    : (page.fontStyle != FontStyle.Undefined ? page.fontStyle 
                    : (formSpec.fontStyle != FontStyle.Undefined ? formSpec.fontStyle
                    : FontStyle.Regular)));

            p.x = CalculateCoordinate(formSpec.x, page.x, section.x, interationOnCurrentPage, section.shiftX, field.x);
            p.y = CalculateCoordinate(formSpec.y, page.y, section.y, interationOnCurrentPage, section.shiftY, field.y);

            return p;
        }

        private decimal CalculateCoordinate(decimal formOffset, decimal pageOffset, decimal sectionOffset, long iteration, decimal shiftValue, decimal itemCoordinate)
        {
            return formOffset + pageOffset + sectionOffset + (iteration * shiftValue) + itemCoordinate;
        }

        private void WriteException(Exception ex, string message)
        {
            if(currentPage == null)
                currentPage = builder.AddPage(PageSize.A4);

            currentPage.AddText("Exception", message + Environment.NewLine + ex.ToString(), "Left", "Impact", 12, FontStyle.Bold, ColourName.Black, 2, 2, currentPage.GetWidth() - 4, currentPage.GetHeight() - 4);
        }
    }
}

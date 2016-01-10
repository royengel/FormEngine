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
                                        new Field() { x = 1, y = 1, width=15, name = "exception", colour = ColourName.Red, font = "Impact", fontSize = 18M }
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
                if (!ExecuteForm(formSpec, v))
                {
                    ok = false;
                    break;
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

        private bool ExecutePage(Form formSpec, Page page, IValues v)
        {
            currentPage = builder.AddPage(page.pageSize);

            foreach (Section section in page.sections)
                if (!ExecuteSection(formSpec, page, section, v))
                    return false;

            return true;
        }

        private bool ExecuteSection(Form formSpec, Page page, Section section, IValues v)
        {
            foreach (Field field in section.fields)
                if (!ExecuteField(formSpec, page, section, field, v))
                    return false;

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
            p.x = field.x + section.x + page.x + formSpec.x;
            p.y = field.y + section.y + page.y + formSpec.y;
            return p;
        }

        private void WriteException(Exception ex, string message)
        {
            if(currentPage == null)
                currentPage = builder.AddPage(PageSize.A4);

            currentPage.AddText("Exception", message + Environment.NewLine + ex.ToString(), "Left", "Impact", 18, FontStyle.Bold, ColourName.Black, 2, 2, currentPage.GetWidth() - 4, currentPage.GetHeight() - 4);
        }
    }
}

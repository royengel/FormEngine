using FormEngine.FileSystem;
using FormEngine.Interfaces;
using FormEngine.PdfFormBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FormEngine.Tests
{
    public class IntegrationTest
    {
        private readonly ITestOutputHelper output;

        public IntegrationTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]// (Skip="Integration test")]
        public void WhenCreateForm_ThenGotAPdf()
        {
            IEnumerable<dynamic> values = new List<dynamic> {
            new {
                Tekst = "Nesten alle økonomiske indikatorer har kommet siden september tyder på at rentebanen skal ned. Rapporten fra Regionalt nettverk bare bekrefter dette, sier sjeføkonom Kari Due-Andresen i Handelsbanken Capital Markets til Nettavisen. Rapporten fra Regionalt nettverk dette kvartalet tyder på at den økonomiske veksten i Norge vil fortsette å øke i tiden fremover, men er likevel svakere enn ventet.",
                Vare = "Syltetøy",
                Pris = 34.9M,
            },
            new {
                Vare = "Saks",
                Pris = 119M,
            },
        };

            Form f = new Form()
            {
                pageSize = PageSize.A4,
                fontSize = 10,
                reports = new List<Report>
                {
                    new Report()
                    {
                        sections = new List<Section>
                        {
                            new Section
                            {
                                sectionType = SectionType.FormHeader,
                                fields = new List<Field>
                                {
                                    new Field { y = 0.0M, x = .5M, value = v => "---------------------*************** Hey ***************---------------" },
                                    new Field { y = 0.6M, x = 1M, width = 8M, value = v => v.Tekst },
                                    new Field { y = 0.6M, x = 15M, value = v => DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") },
                                },
                                images = new List<Image>
                                {
                                    new Image { y = 0.6M, x = 9, width = 6.5M, height = 3, name = "eika.png" }
                                }
                            },
                            new Section
                            {
                                sectionType = SectionType.FormHeader,
                                fields = new List<Field>()
                                {
                                    new Field() { y = 0.0M, x = .5M, value = v => "------------------------------------------------------------------------------------------------------------------------------------" },
                                }
                            },
                            new Section
                            {
                                sectionType = SectionType.Detail,
                                fields = new List<Field>()
                                {
                                    new Field() { y = 0.0M, x = 1M, width = 3M, value = v => v.Vare },
                                    new Field() { y = 0.0M, x = 8M, width = 3M, alignment = "right", value = v => v.Pris.ToString("0.00") },
                                }
                            }
                        }
                    }
                }
            };

            IResources files = new Folder(".");

            bool ok = false;
            using (FileStream outStream = new FileStream("test.pdf", FileMode.Create))
            {
                IFormBuilder builder = new Document(outStream, files);
                MakeForm form = new MakeForm(builder);
                output.WriteLine("IFormBuilder: {0}", builder.GetType().ToString());
                ok = form.Execute(files, values, f);
                outStream.Close();
            }
            output.WriteLine("Produced test.pdf " + (ok ? "successfully!" : "with errors!"));
        }
    }
}


using System;
using FormEngine.Tests.Helpers;
using FormEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FormEngine.Tests
{
    public class MakeFormTests
    {
        // serialisering av lambda uttrykk er ikke helt trivielt, vent...
        //[Fact]
        //public void MakeForm_Basics()
        //{
        //    FakeFiles files = new FakeFiles() { textFiles = {
        //            { "form1.json",
        //                @"{
        //                    'pageSize':'A4', 
        //                    'reports':
        //                    [
        //                        { 
        //                            'x':'0.5', 
        //                            'y':'0.5', 
        //                            'fontStyle':'Bold',
        //                            'sections':
        //                            [
        //                                { 
        //                                    'x':'1', 
        //                                    'y':'2', 
        //                                    'fields':
        //                                    [
        //                                        { 
        //                                            'name':'v1',
        //                                            'x':'3',
        //                                            'y':'4',
        //                                            'testValue':'XXX' 
        //                                        }
        //                                    ]
        //                                }
        //                            ]
        //                        }
        //                    ]
        //                  }" } } };
        //    List<IValues> values = new List<IValues> { new Values(new Dictionary<string, object> { { "v1", "1" } }) };
        //    FakeFormBuilder builder = new FakeFormBuilder();
        //    MakeForm maker = new MakeForm(builder);

        //    Assert.IsTrue(maker.Execute(files, values, "form1"), "MakeForm.Execute failed!");
        //    Assert.Equal(1, builder.pages.Count);
        //    Assert.Equal(1, builder.pages[0].texts.Count, "Wrong number of fields");
        //    Assert.Equal("A4", builder.pages[0].pageSize);
        //    Assert.Equal("1", builder.pages[0].texts[0].text);
        //    Assert.Equal(4.5M, builder.pages[0].texts[0].x);
        //    Assert.Equal("Bold", builder.pages[0].texts[0].fontStyle);
        //}

        [Fact]
        public void MakeForm_Defaults()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                x = .2M,
                y = .4M,
                pageSize = PageSize.A4,
                reports = new List<Report>() {
                        new Report() {
                            x = -.1M,
                            y = -.2M,
                            colour = ColourName.Red, 
                            sections = new List<Section>() {
                                new Section() {
                                    x = 1,
                                    y = 2,
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 4, y = 8, colour = ColourName.Blue, name = "v1", value = v => v.v1 },
                                        new Field() { x = 1, y = 1, name = "v2", value = v => v.v2 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> { new Values( new Dictionary<string, object> { { "v1", "1" }, { "v2", "2" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);

            Assert.Equal(1, builder.pages.Count);
            Assert.Equal(2, builder.pages[0].texts.Count); //Wrong number of fields

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("1", t1.text);
            Assert.Equal(5.1M, t1.x);
            Assert.Equal(10.2M, t1.y);
            Assert.Equal(ColourName.Blue, t1.colour);

            FormText t2 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("2", t2.text);
            Assert.Equal(2.1M, t2.x);
            Assert.Equal(3.2M, t2.y);
            Assert.Equal(ColourName.Red, t2.colour);
        }

        [Fact]
        public void MakeForm_TestValues()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() { 
                                    fields = new List<Field>()
                                    {
                                        new Field() { name = "v1", value = v => v.v1, testValue = "x" }
                                    }
                                }
                            }
                        }
                    }
            };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, null, form);
            Assert.Equal(1, builder.pages.Count);
            Assert.Equal(1, builder.pages[0].texts.Count); // Wrong number of fields

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("x", t1.text);
        }

        [Fact]
        public void MakeForm_MultiplePages()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    pageBreak = true,
                                    sectionType = SectionType.GroupHeader,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 }
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { name = "v1", value = v => v.v1 },
                                        new Field() { name = "v2", value = v => v.v2 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }),
                    new Values(new Dictionary<string, object> { { "v1", "2" }, { "v2", "z" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(2, builder.pages.Count);

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("1", t1.text);

            t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("x", t1.text);

            t1 = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v2");
            Assert.Equal("y", t1.text);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("2", t1.text);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("z", t1.text);
        }

        [Fact]
        public void MakeForm_MultipleSections()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.GroupHeader,
                                    pageBreak = true,
                                    height = 0m,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 }
                                },
                                new Section() {
                                    sectionType = SectionType.GroupHeader,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1, b => b.v2 },
                                    height = 1M,
                                    images = new List<Image>()
                                    {
                                        new Image() { y = 3M, name = "i1" }
                                    },
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 },
                                        new Field() { y = 1M, name = "v2", value = v => v.v2 },
                                        new Field() { y = 1M, name = "v3", value = v => v.v3 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" }, { "v3", "a" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" }, { "v3", "b" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" }, { "v3", "c" } }),
                    new Values(new Dictionary<string, object> { { "v1", "2" }, { "v2", "y" }, { "v3", "d" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(2, builder.pages.Count);

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v3");
            Assert.Equal("a", t1.text);
            Assert.Equal(1M, t1.y);

            t1 = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v3");
            Assert.Equal("b", t1.text);
            Assert.Equal(2M, t1.y);

            Assert.False(builder.pages[0].texts.Any(t => t.fieldName == "v3" && t.text == "c"));

            FormImage i1 = builder.pages[0].images.FirstOrDefault(t => t.file == "i1");
            Assert.Equal(3M, i1.y);
            i1 = builder.pages[0].images.LastOrDefault(t => t.file == "i1");
            Assert.Equal(4M, i1.y);
        }

        [Fact]
        public void MakeForm_SectionsWithAndWitoutBreak()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.GroupHeader,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 },
                                    height = 0,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    height = 1M,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v2", value = v => v.v2 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(1, builder.pages.Count);

            Assert.Equal(1, builder.pages[0].texts.Where(t => t.fieldName == "v1").Count());
            Assert.Equal(2, builder.pages[0].texts.Where(t => t.fieldName == "v2").Count());

            FormText ft = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("x", ft.text);
            Assert.Equal(1M, ft.y);
            ft = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v2");
            Assert.Equal("y", ft.text);
            Assert.Equal(2M, ft.y);
        }

        [Fact]
        public void MakeForm_PageHeaderAndFooter()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.PageHeader,
                                    height = 1M,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    height = 6M,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v2", value = v => v.v2 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.PageFooter,
                                    height = 6M
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "a" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "b" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "c" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "d" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "e" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "f" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(2, builder.pages.Count);

            Assert.Equal(1, builder.pages[0].texts.Where(t => t.fieldName == "v1").Count());
            Assert.Equal(1, builder.pages[1].texts.Where(t => t.fieldName == "v1").Count());

            FormText[] ft = builder.pages[0].texts.Where(t => t.fieldName == "v2").ToArray();
            Assert.Equal("a", ft[0].text);
            Assert.Equal(2M, ft[0].y);
            Assert.Equal("b", ft[1].text);
            Assert.Equal(8M, ft[1].y);
            Assert.Equal("c", ft[2].text);
            Assert.Equal(14M, ft[2].y);
            ft = builder.pages[1].texts.Where(t => t.fieldName == "v2").ToArray();
            Assert.Equal("d", ft[0].text);
            Assert.Equal(2M, ft[0].y);
            Assert.Equal("e", ft[1].text);
            Assert.Equal(8M, ft[1].y);
            Assert.Equal("f", ft[2].text);
            Assert.Equal(14M, ft[2].y);
        }

        [Fact]
        public void MakeForm_VariableHeightSections()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" } }),
                    new Values(new Dictionary<string, object> { { "v1", "2" + Environment.NewLine + "2" } }),
                    new Values(new Dictionary<string, object> { { "v1", "3" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(1, builder.pages.Count);

            List<FormText> texts = builder.pages[0].texts;
            Assert.Equal(3, texts.Where(t => t.fieldName == "v1").Count());

            Assert.Equal("1", texts[0].text);
            Assert.Equal(1M, texts[0].y);
            Assert.Equal("2" + Environment.NewLine + "2", texts[1].text);
            Assert.Equal(3M, texts[1].y);
            Assert.Equal("3", texts[2].text);
            Assert.Equal(6M, texts[2].y);
        }

        [Fact]
        public void MakeForm_TwoReports()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                }
                            }
                        }
                    }
            };

            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form, false);
            maker.Execute(files, values, form);
            Assert.Equal(1, builder.pages.Count);

            List<FormText> texts = builder.pages[0].texts;
            Assert.Equal(2, texts.Where(t => t.fieldName == "v1").Count());

            Assert.Equal("1", texts[0].text);
            Assert.Equal(1M, texts[0].y);
            Assert.Equal("1", texts[1].text);
            Assert.Equal(3M, texts[1].y);
        }

        [Fact]
        public void MakeForm_DetailHeader()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    pageBreak = true,
                                    sectionType = SectionType.GroupHeader,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 }
                                },
                                new Section() {
                                    sectionType = SectionType.DetailHeader,
                                    fields = new List<Field>()
                                    {
                                        new Field() { name = "l1", value = v => "a" }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { name = "v1", value = v => v.v1 },
                                        new Field() { name = "v2", value = v => v.v2 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }),
                    new Values(new Dictionary<string, object> { { "v1", "2" }, { "v2", "z" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(2, builder.pages.Count);

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("1", t1.text);
            Assert.Equal(1, t1.y);

            t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "l1");
            Assert.Equal("a", t1.text);
            Assert.Equal(0, t1.y);

            t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("x", t1.text);
            Assert.Equal(1, t1.y);

            t1 = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v2");
            Assert.Equal("y", t1.text);
            Assert.Equal(2, t1.y);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "l1");
            Assert.Equal("a", t1.text);
            Assert.Equal(0, t1.y);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.Equal("2", t1.text);
            Assert.Equal(1, t1.y);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.Equal("z", t1.text);
            Assert.Equal(1, t1.y);
        }

        [Fact]
        public void MakeForm_BreakFooterSection()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.GroupHeader,
                                    pageBreak = true,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 },
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v2", value = v => v.v2 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.GroupFooter,
                                    breakColumns = new List<Func<dynamic, object>>() { b => b.v1 },
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }),
                    new Values(new Dictionary<string, object> { { "v1", "2" }, { "v2", "y" } }) };
            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(2, builder.pages.Count);

            FakeFormPage page = builder.pages[0];
            Assert.Equal("x", page.texts[0].text);
            Assert.Equal("1", page.texts[1].text);
            page = builder.pages[1];
            Assert.Equal("y", page.texts[0].text);
            Assert.Equal("2", page.texts[1].text);
        }

        [Fact]
        public void MakeForm_DetailHeaderAndFormHeader()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                reports = new List<Report>() {
                        new Report() {
                            sections = new List<Section>() {
                                new Section() {
                                    sectionType = SectionType.FormHeader,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v1", value = v => v.v1 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.DetailHeader,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v2", value = v => v.v2 }
                                    }
                                },
                                new Section() {
                                    sectionType = SectionType.Detail,
                                    fields = new List<Field>()
                                    {
                                        new Field() { y = 1M, name = "v3", value = v => v.v3 }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" }, { "v3", "a" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" }, { "v3", "b" } }) };

            FakeFormBuilder builder = new FakeFormBuilder();
            MakeForm maker = new MakeForm(builder);

            maker.Execute(files, values, form);
            Assert.Equal(1, builder.pages.Count);
            Assert.Equal(4, builder.pages[0].texts.Count);

            FormText t = builder.pages[0].texts.FirstOrDefault(f => f.fieldName == "v1");
            Assert.Equal("1", t.text);
            Assert.Equal(1M, t.y);

            t = builder.pages[0].texts.FirstOrDefault(f => f.fieldName == "v2");
            Assert.Equal("x", t.text);
            Assert.Equal(3M, t.y);

            t = builder.pages[0].texts.FirstOrDefault(f => f.fieldName == "v3");
            Assert.Equal("a", t.text);
            Assert.Equal(5M, t.y);

            t = builder.pages[0].texts.LastOrDefault(f => f.fieldName == "v3");
            Assert.Equal("b", t.text);
            Assert.Equal(7M, t.y);

        }


    }
}

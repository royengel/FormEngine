using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormEngine.Tests.Helpers;
using System.Text;
using FormEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace FormEngine.Tests
{
    [TestClass]
    public class MakeFormTests
    {
        [TestMethod]
        public void Basics()
        {
            FakeFiles files = new FakeFiles() { textFiles = {
                    { "form1.json", 
                        @"{
                            'pages':
                            [
                                { 
                                    'pageSize':'A4', 
                                    'x':'0.5', 
                                    'y':'0.5', 
                                    'fontStyle':'Bold',
                                    'sections':
                                    [
                                        { 
                                            'x':'1', 
                                            'y':'2', 
                                            'fields':
                                            [
                                                { 
                                                    'name':'v1',
                                                    'x':'3',
                                                    'y':'4',
                                                    'testValue':'XXX' 
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                          }" } } };
            List<IValues> values = new List<IValues> { new Values(new Dictionary<string, object> { { "v1", "1" } }) };
            MakeForm maker = new MakeForm(files, values);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute("form1", builder), "MakeForm.Execute failed!");
            Assert.AreEqual(1, builder.pages.Count);
            Assert.AreEqual(1, builder.pages[0].texts.Count, "Wrong number of fields");
            Assert.AreEqual("A4", builder.pages[0].pageSize);
            Assert.AreEqual("1", builder.pages[0].texts[0].text);
            Assert.AreEqual(4.5M, builder.pages[0].texts[0].x);
            Assert.AreEqual("Bold", builder.pages[0].texts[0].fontStyle);
        }

        [TestMethod]
        public void Defaults()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1", x = .2M, y = .4M,
                pages = new List<Page>() {
                        new Page() { pageSize = PageSize.A4, x = -.1M, y = -.2M, colour = ColourName.Red, 
                            sections = new List<Section>() {
                                new Section() { x = 1, y = 2,
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 4, y = 8, colour = ColourName.Blue, name="v1" },
                                        new Field() { x = 1, y = 1, name = "v2" }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> { new Values( new Dictionary<string, object> { { "v1", "1" }, { "v2", "2" } }) };
            MakeForm maker = new MakeForm(files, values);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute(form, builder), "MakeForm.Execute failed!");
            Assert.AreEqual(1, builder.pages.Count);
            Assert.AreEqual(2, builder.pages[0].texts.Count, "Wrong number of fields");

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.AreEqual("1", t1.text);
            Assert.AreEqual(5.1M, t1.x);
            Assert.AreEqual(10.2M, t1.y);
            Assert.AreEqual(ColourName.Blue, t1.colour);

            FormText t2 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("2", t2.text);
            Assert.AreEqual(2.1M, t2.x);
            Assert.AreEqual(3.2M, t2.y);
            Assert.AreEqual(ColourName.Red, t2.colour);
        }

        [TestMethod]
        public void TestValues()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                pages = new List<Page>() {
                        new Page() {
                            sections = new List<Section>() {
                                new Section() { 
                                    fields = new List<Field>()
                                    {
                                        new Field() { name="v1", testValue="x" }
                                    }
                                }
                            }
                        }
                    }
            };
            MakeForm maker = new MakeForm(files);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute(form, builder), "MakeForm.Execute failed!");
            Assert.AreEqual(1, builder.pages.Count);
            Assert.AreEqual(1, builder.pages[0].texts.Count, "Wrong number of fields");

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.AreEqual("x", t1.text);
        }

        [TestMethod]
        public void MultiplePages()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                pages = new List<Page>() {
                        new Page() {
                            breakColumns = new List<string>() { "v1" },
                            sections = new List<Section>() {
                                new Section() {
                                    fields = new List<Field>()
                                    {
                                        new Field() { name = "v1" },
                                        new Field() { name = "v2" }
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
            MakeForm maker = new MakeForm(files, values);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute(form, builder), "MakeForm.Execute failed!");
            Assert.AreEqual(2, builder.pages.Count);

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.AreEqual("1", t1.text);

            t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("x", t1.text);

            t1 = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("y", t1.text);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v1");
            Assert.AreEqual("2", t1.text);

            t1 = builder.pages[1].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("z", t1.text);
        }

        [TestMethod]
        public void MultipleSections()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                pages = new List<Page>() {
                        new Page() {
                            breakColumns = new List<string>() { "v1" },
                            sections = new List<Section>() {
                                new Section() {
                                    breakColumns = new List<string>() { "v1", "v2" },
                                    shiftX = 1M,
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 1M, name = "v1" },
                                        new Field() { x = 1M, name = "v2" },
                                        new Field() { x = 1M, name = "v3" }
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
            MakeForm maker = new MakeForm(files, values);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute(form, builder), "MakeForm.Execute failed!");
            Assert.AreEqual(2, builder.pages.Count);

            FormText t1 = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v3");
            Assert.AreEqual("a", t1.text);
            Assert.AreEqual(1M, t1.x);

            t1 = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v3");
            Assert.AreEqual("b", t1.text);
            Assert.AreEqual(2M, t1.x);

            Assert.IsFalse(builder.pages[0].texts.Any(t => t.fieldName == "v3" && t.text == "c"));

        }

        [TestMethod]
        public void SectionsWithAndWitoutBreak()
        {
            FakeFiles files = new FakeFiles();
            Form form = new Form()
            {
                formTitle = "title1",
                pages = new List<Page>() {
                        new Page() {
                            breakColumns = new List<string>() { "v1" },
                            sections = new List<Section>() {
                                new Section() {
                                    breakColumns = new List<string>() { "v1" },
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 1M, name = "v1" }
                                    }
                                },
                                new Section() {
                                    shiftX = 1M,
                                    fields = new List<Field>()
                                    {
                                        new Field() { x = 1M, name = "v2" }
                                    }
                                }
                            }
                        }
                    }
            };
            List<IValues> values = new List<IValues> {
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "x" } }),
                    new Values(new Dictionary<string, object> { { "v1", "1" }, { "v2", "y" } }) };
            MakeForm maker = new MakeForm(files, values);
            FakeFormBuilder builder = new FakeFormBuilder();

            Assert.IsTrue(maker.Execute(form, builder), "MakeForm.Execute failed!");
            Assert.AreEqual(1, builder.pages.Count);

            Assert.AreEqual(1, builder.pages[0].texts.Where(t => t.fieldName == "v1").Count());
            Assert.AreEqual(2, builder.pages[0].texts.Where(t => t.fieldName == "v2").Count());

            FormText ft = builder.pages[0].texts.FirstOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("x", ft.text);
            Assert.AreEqual(1M, ft.x);
            ft = builder.pages[0].texts.LastOrDefault(t => t.fieldName == "v2");
            Assert.AreEqual("y", ft.text);
            Assert.AreEqual(2M, ft.x);

            Assert.IsFalse(builder.pages[0].texts.Any(t => t.fieldName == "v3" && t.text == "c"));

        }


    }
}

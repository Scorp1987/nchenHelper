using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Attributes;
using System.IO.Types;

namespace System.IO.Tests
{
    [TestClass]
    public class TextFieldParserExtensionTest
    {
        private readonly TestObject TestObject1 = new TestObject
        {
            Text = "TestObject1",
            Date = new DateTime(2021, 11, 12),
            DateTime = new DateTime(2021, 12, 22, 15, 14, 20),
            NullableDateTime = new DateTime(2021, 12, 15, 16, 47, 16, 205),
            Number = 10,
            NullableNumber = 5.6,
            Object = new TestSubObject
            {
                Name = "TestSubObject1",
                Value = 15
            },
        };

        private readonly TestObject TestObject2 = new TestObject
        {
            Text = "TestObject2",
            Date = new DateTime(2021, 10, 12),
            DateTime = new DateTime(2020, 12, 22, 15, 14, 20),
            NullableDateTime = null,
            Number = 5,
            NullableNumber = null,
            Object = new TestSubObject
            {
                Name = "TestSubObject2",
                Value = 14
            },
        };

        private void TestTextFieldParser(string source, Action<TextFieldParser> testAction)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.Write(source);
                writer.Flush();
                stream.Position = 0;
                testAction(parser);
            }
        }

        [TestMethod]
        public void TestGetPosition()
        {
            TestTextFieldParser("Name,Value,Description\r\nName1,0,\r\nName2,\"Value,String\",DescriptionTest\r\n", parser =>
            {
                parser.ReadFields();
                var pos = parser.GetPosition();
                Assert.AreEqual(24, pos);
                parser.ReadFields();
                pos = parser.GetPosition();
                Assert.AreEqual(34, pos);
                parser.ReadFields();
                pos = parser.GetPosition();
                Assert.AreEqual(72, pos);
            });
        }

        [TestMethod]
        public void TestGetColumnInfos1()
        {
            TestTextFieldParser("TextName,Date,DateTimeName,NullableDateTime,Number,NullableNumber,ObjectName\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,,5,,TestSubObject2|14\r\n",
                parser =>
                {
                    var columnInfos = parser.GetColumnInfos();
                    for (var i = 0; i < columnInfos.Length; i++)
                        Assert.AreEqual(i, columnInfos[i].Index);
                    Assert.AreEqual("TextName", columnInfos[0].Name);
                    Assert.AreEqual("Date", columnInfos[1].Name);
                    Assert.AreEqual("DateTimeName", columnInfos[2].Name);
                    Assert.AreEqual("NullableDateTime", columnInfos[3].Name);
                    Assert.AreEqual("Number", columnInfos[4].Name);
                    Assert.AreEqual("NullableNumber", columnInfos[5].Name);
                    Assert.AreEqual("ObjectName", columnInfos[6].Name);
                });
        }

        [TestMethod]
        public void TestGetColumnInfoReadObjectUpdateObject2()
        {
            var str = "Text,Date,DateTime,OtherColumn,NullableDateTime,Number,NullableNumber,Object\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,OtherRow1,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,OtherRow2,,5,,TestSubObject2|14\r\n";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(str);
                writer.Flush();
                stream.Position = 0;

                var parser1 = stream.GetCsvTextFieldParser();
                var columnInfos = parser1.GetColumnInfos<TestObject>();
                var type = typeof(TestObject);
                for (var i = 0; i < columnInfos.Length; i++)
                    Assert.AreEqual(i, columnInfos[i].Index);
                Assert.AreEqual("Text", columnInfos[0].Name);
                Assert.AreEqual("Date", columnInfos[1].Name);
                Assert.AreEqual("DateTime", columnInfos[2].Name);
                Assert.AreEqual("OtherColumn", columnInfos[3].Name);
                Assert.AreEqual("NullableDateTime", columnInfos[4].Name);
                Assert.AreEqual("Number", columnInfos[5].Name);
                Assert.AreEqual("NullableNumber", columnInfos[6].Name);
                Assert.AreEqual("Object", columnInfos[7].Name);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.Text)), columnInfos[0].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.Date)), columnInfos[1].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.DateTime)), columnInfos[2].Property);
                Assert.AreEqual(null, columnInfos[3].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.NullableDateTime)), columnInfos[4].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.Number)), columnInfos[5].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.NullableNumber)), columnInfos[6].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.Object)), columnInfos[7].Property);

                stream.Position = 0;

                var parser2 = stream.GetCsvTextFieldParser();
                var partialColumnInfos = parser2.GetColumnInfos<TestObject, WithoutNameAttribute>();
                for (var i = 0; i < partialColumnInfos.Length; i++)
                    Assert.AreEqual(i, partialColumnInfos[i].Index);
                Assert.AreEqual("TextName", partialColumnInfos[0].Name);
                Assert.AreEqual("Date", partialColumnInfos[1].Name);
                Assert.AreEqual("DateTimeName", partialColumnInfos[2].Name);
                Assert.AreEqual("OtherColumn", partialColumnInfos[3].Name);
                Assert.AreEqual("NullableDateTime", partialColumnInfos[4].Name);
                Assert.AreEqual("Number", partialColumnInfos[5].Name);
                Assert.AreEqual("NullableNumber", partialColumnInfos[6].Name);
                Assert.AreEqual("ObjectName", partialColumnInfos[7].Name);
                Assert.AreEqual(null, partialColumnInfos[0].Property); ;
                Assert.AreEqual(type.GetProperty(nameof(TestObject.Date)), partialColumnInfos[1].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.DateTime)), partialColumnInfos[2].Property);
                Assert.AreEqual(null, partialColumnInfos[3].Property);
                Assert.AreEqual(null, partialColumnInfos[4].Property);
                Assert.AreEqual(null, partialColumnInfos[5].Property);
                Assert.AreEqual(type.GetProperty(nameof(TestObject.NullableNumber)), partialColumnInfos[6].Property);
                Assert.AreEqual(null, partialColumnInfos[7].Property);

                var obj = parser1.ReadObject<TestObject>(columnInfos);
                Assert.AreEqual("TestObject1", obj.Text);
                Assert.AreEqual(new DateTime(2021, 11, 12), obj.Date);
                Assert.AreEqual(new DateTime(2021, 12, 22, 15, 14, 20), obj.DateTime);
                Assert.AreEqual(new DateTime(2021, 12, 15, 16, 47, 16, 205), obj.NullableDateTime);
                Assert.AreEqual(10, obj.Number);
                Assert.AreEqual(5.6, obj.NullableNumber);
                Assert.AreEqual("TestSubObject1", obj.Object.Name);
                Assert.AreEqual(15, obj.Object.Value);

                parser2.UpdateObject(obj, partialColumnInfos);
                parser2.UpdateObject(obj, partialColumnInfos);
                Assert.AreEqual("TestObject1", obj.Text);
                Assert.AreEqual(new DateTime(2021, 10, 12), obj.Date);
                Assert.AreEqual(new DateTime(2020, 12, 22, 15, 14, 20), obj.DateTime);
                Assert.AreEqual(new DateTime(2021, 12, 15, 16, 47, 16, 205), obj.NullableDateTime);
                Assert.AreEqual(10, obj.Number);
                Assert.AreEqual(null, obj.NullableNumber);
                Assert.AreEqual("TestSubObject1", obj.Object.Name);
                Assert.AreEqual(15, obj.Object.Value);

                parser1.Dispose();
                parser2.Dispose();
            }
        }

        [TestMethod]
        public void TestAddToCollection1()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            var list = new List<TestObject>();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.WriteCollection(",", objects);
                writer.Flush();
                stream.Position = 0;
                var columnInfos = parser.GetColumnInfos<TestObject>();
                parser.AddToCollection(list, columnInfos);
            }
            for (var i = 0; i < objects.Length; i++)
                Assert.AreEqual(objects[i], list[i]);
        }

        [TestMethod]
        public void TestAddToCollection2()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            var list = new List<TestObject>();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.WriteCollection(",", objects);
                writer.Flush();
                stream.Position = 0;
                parser.AddToCollection(list);
            }
            for (var i = 0; i < objects.Length; i++)
                Assert.AreEqual(objects[i], list[i]);
        }

        [TestMethod]
        public void TestAddToCollection3()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            var list = new List<TestObject>();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.WriteCollection(",", objects);
                writer.Flush();
                stream.Position = 0;
                parser.AddToCollection<TestObject, WithoutNameAttribute>(list);
            }
            for (var i = 0; i < objects.Length; i++)
            {
                Assert.AreEqual(objects[i].Date, list[i].Date);
                Assert.AreEqual(objects[i].DateTime, list[i].DateTime);
                Assert.AreEqual(objects[i].NullableNumber, list[i].NullableNumber);
            }
        }

        [TestMethod]
        public void TestAddToDictionary1()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            var dictionary = new Dictionary<string, TestObject>();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.WriteCollection(",", objects);
                writer.Flush();
                stream.Position = 0;
                parser.AddToDictionary(dictionary, i => i.Text);
            }
            for (var i = 0; i < objects.Length; i++)
                Assert.AreEqual(objects[i], dictionary[objects[i].Text]);
        }

        [TestMethod]
        public void TestAddToDictionary2()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            var dictionary = new Dictionary<string, TestObject>();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            using (var parser = stream.GetCsvTextFieldParser())
            {
                writer.WriteCollection<TestObject, WithoutNameAttribute>(",", objects);
                writer.Flush();
                stream.Position = 0;
                parser.AddToDictionary<string, TestObject, WithoutNameAttribute>(dictionary, i => i.Text);
            }
            for (var i = 0; i < objects.Length; i++)
            {
                Assert.AreEqual(objects[i].Date, dictionary[objects[i].Text].Date);
                Assert.AreEqual(objects[i].DateTime, dictionary[objects[i].Text].DateTime);
                Assert.AreEqual(objects[i].NullableNumber, dictionary[objects[i].Text].NullableNumber);
            }
        }
    }
}

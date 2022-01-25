using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.IO.Attributes;
using System.IO.Extensions;
using System.IO.Types;
using System.Reflection;
using System.Threading.Tasks;

namespace System.IO.Tests
{
    [TestClass]
    public class StreamWriterExtensionTest
    {
        private void TestStreamWriter(string expected, Action<StreamWriter> action)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                action(writer);
                var str = writer.ReadStreamAndDispose();
                Assert.AreEqual(expected, str);
            }
        }
        private void TestStreamWriter(string expected, Func<StreamWriter, Task> asyncFunc)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                asyncFunc(writer).Wait();
                var str = writer.ReadStreamAndDispose();
                Assert.AreEqual(expected, str);
            }
        }
        private void TestWriteFields_Values(string expected, IEnumerable values, string delimiter = ",")
        {
            TestStreamWriter(expected, w => w.WriteFields(delimiter, values));
            TestStreamWriter(expected, async w => await w.WriteFieldsAsync(delimiter, values));
        }

        [TestMethod]
        public void TestWriteFields_Values()
        {
            IEnumerable toWrite = new object[] { "Test1", 10 };
            TestWriteFields_Values("Test1,10\r\n", toWrite);
            toWrite = new object[] { "Test,Value", "Test\r\nNewLine", "Test,Delimited", "Test\"Quote", "Test\",Combine" };
            TestWriteFields_Values("\"Test,Value\",\"Test\r\nNewLine\",\"Test,Delimited\",\"Test\"\"Quote\",\"Test\"\",Combine\"\r\n", toWrite);
            toWrite = new object[] { "TestDates", new DateTime(2021, 12, 22), new DateTime(2021, 11, 21, 20, 01, 02) };
            TestWriteFields_Values("TestDates,2021-12-22,2021-11-21 20:01:02\r\n", toWrite);
            TestWriteFields_Values("TestDates-\"2021-12-22\"-\"2021-11-21 20:01:02\"\r\n", toWrite, "-");
        }

        [TestMethod]
        public void TestWriteFields_ColumnNames()
        {
            TestStreamWriter("TextName,Date,DateTimeName,NullableDateTime,Number,NullableNumber,ObjectName\r\n", w => w.WriteFields<TestObject>(","));
            TestStreamWriter("TextName,Date,DateTimeName,NullableDateTime,Number,NullableNumber,ObjectName\r\n", w => w.WriteFieldsAsync<TestObject>(","));
            TestStreamWriter("Text,Date,DateTimeName,NullableNumber\r\n", w => w.WriteFields<TestObject, WithoutNameAttribute>(","));
            TestStreamWriter("Text,Date,DateTimeName,NullableNumber\r\n", w => w.WriteFieldsAsync<TestObject, WithoutNameAttribute>(","));
            TestStreamWriter("TextName,ObjectName\r\n", w => w.WriteFields<TestObject, WithNameAttribute>(","));
            TestStreamWriter("TextName,ObjectName\r\n", w => w.WriteFieldsAsync<TestObject, WithNameAttribute>(","));
        }


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

        [TestMethod]
        public void TestWriteFields_Object1()
        {
            var type = typeof(TestObject);
            var properties = new PropertyInfo[]
            {
                type.GetProperty(nameof(TestObject.Text)),
                type.GetProperty(nameof(TestObject.NullableNumber)),
                type.GetProperty(nameof(TestObject.NullableDateTime)),
                type.GetProperty(nameof(TestObject.DateTime)),
                type.GetProperty(nameof(TestObject.Object)),
            };
            TestStreamWriter("TestObject1,5.6,2021-12-15 16:47:16.205,2021-12-22 15:14:20,TestSubObject1|15\r\n", w => w.WriteFields(",", TestObject1, properties));
            TestStreamWriter("TestObject1,5.6,2021-12-15 16:47:16.205,2021-12-22 15:14:20,TestSubObject1|15\r\n", w => w.WriteFieldsAsync(",", TestObject1, properties));
            TestStreamWriter("TestObject2|||2020-12-22 15:14:20|\"TestSubObject2|14\"\r\n", w => w.WriteFields("|", TestObject2, properties));
            TestStreamWriter("TestObject2|||2020-12-22 15:14:20|\"TestSubObject2|14\"\r\n", w => w.WriteFieldsAsync("|", TestObject2, properties));
        }

        [TestMethod]
        public void TestWriteFields_Object2()
        {
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\n", w => w.WriteFields(",", TestObject1));
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\n", w => w.WriteFieldsAsync(",", TestObject1));
            TestStreamWriter("TestObject2|2021-10-12|2020-12-22 15:14:20||5||\"TestSubObject2|14\"\r\n", w => w.WriteFields("|", TestObject2));
            TestStreamWriter("TestObject2|2021-10-12|2020-12-22 15:14:20||5||\"TestSubObject2|14\"\r\n", w => w.WriteFieldsAsync("|", TestObject2));
        }

        [TestMethod]
        public void TestWriteFields_Object3()
        {
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\n", w => w.WriteFields<TestObject, WithoutNameAttribute>(",", TestObject1));
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\n", w => w.WriteFieldsAsync<TestObject, WithoutNameAttribute>(",", TestObject1));
            TestStreamWriter("TestObject2,TestSubObject2|14\r\n", w => w.WriteFields<TestObject, WithNameAttribute>(",", TestObject2));
            TestStreamWriter("TestObject2,TestSubObject2|14\r\n", w => w.WriteFieldsAsync<TestObject, WithNameAttribute>(",", TestObject2));
        }

        [TestMethod]
        public void TestWriteCollection_Object2()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,,5,,TestSubObject2|14\r\n", w => w.WriteCollection(",", objects, false));
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,,5,,TestSubObject2|14\r\n", w => w.WriteCollectionAsync(",", objects, false));
            TestStreamWriter("TextName,Date,DateTimeName,NullableDateTime,Number,NullableNumber,ObjectName\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,,5,,TestSubObject2|14\r\n", w => w.WriteCollection(",", objects, true));
            TestStreamWriter("TextName,Date,DateTimeName,NullableDateTime,Number,NullableNumber,ObjectName\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,2021-12-15 16:47:16.205,10,5.6,TestSubObject1|15\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,,5,,TestSubObject2|14\r\n", w => w.WriteCollectionAsync(",", objects, true));
        }

        [TestMethod]
        public void TestWriteCollection_Object3()
        {
            var objects = new TestObject[] { TestObject1, TestObject2 };
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,\r\n", w => w.WriteCollection<TestObject, WithoutNameAttribute>(",", objects, false));
            TestStreamWriter("TestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,\r\n", w => w.WriteCollectionAsync<TestObject, WithoutNameAttribute>(",", objects, false));
            TestStreamWriter("Text,Date,DateTimeName,NullableNumber\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,\r\n", w => w.WriteCollection<TestObject, WithoutNameAttribute>(",", objects, true));
            TestStreamWriter("Text,Date,DateTimeName,NullableNumber\r\nTestObject1,2021-11-12,2021-12-22 15:14:20,5.6\r\nTestObject2,2021-10-12,2020-12-22 15:14:20,\r\n", w => w.WriteCollectionAsync<TestObject, WithoutNameAttribute>(",", objects, true));
        }
    }
}

using Campus.DocumentValidator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.Reflection;
using System.IO;
using Aspose.Cells;
using System.Collections.Generic;

namespace Campus.DocumentValidator_Test
{


    /// <summary>
    ///This is a test class for DocumentValidateTest and is intended
    ///to contain all DocumentValidateTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DocumentValidateTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for ValidateRow
        ///</summary>
        [TestMethod()]
        public void ValidateFieldTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetType().Assembly.GetManifestResourceStream("Campus.DocumentValidator_Test.DocumentValidateTest.ValidateRule.xml"));

            XmlElement XmlNode = doc.DocumentElement;
            DocumentValidate target = new DocumentValidate(); // TODO: Initialize to an appropriate value
            ErrorCapturer capture = new ErrorCapturer(target);
            target.LoadRule(XmlNode);
            SheetRowStream source = GetRowStream();

            source.Reset();
            target.BeginDetecteDuplicate();
            while (source.Next())
            {
                target.Validate(source);
            }

            IList<DuplicateData> duplicate = target.EndDetecteDuplicate();

            foreach (DuplicateData each in duplicate)
            {
                TestContext.WriteLine(each.Name);
                TestContext.WriteLine("Fields:{0}", each.Fields.ToString());

                foreach (DuplicateRecord record in each)
                {
                    TestContext.WriteLine("Values:{0}", record.Values.ToString());
                    TestContext.WriteLine("Positions:{0}", record.Positions.ToString());
                }
            }
        }

        private SheetRowStream GetRowStream()
        {
            Workbook book = new Workbook();
            book.Open(GetType().Assembly.GetManifestResourceStream("Campus.DocumentValidator_Test.DocumentValidateTest.SampleData.xls"));
            SheetRowStream RowSource = new SheetRowStream(book.Worksheets[0]);

            return RowSource;
        }
    }
}

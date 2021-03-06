﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GemBox.Spreadsheet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zapos.Constructors.Razor.Constructors;
using Zapos.Constructors.Razor.Tests.TestModels;

namespace Zapos.Printers.Gembox.Tests
{
    [TestClass]
    public class GemBoxConstructorTest
    {
        [TestMethod]
        [DeploymentItem(@".\Content", "Content")]
        [DeploymentItem(@".\Content\Images", @"Content\Images")]
        public void ConstructorTest()
        {
            var rnd = new Random();

            var model = new TestReportModel
            {
                Items = Enumerable.Range(0, 50).Select(id => new TestReportItemModel
                {
                    Id = id,
                    Name = Guid.NewGuid().ToString(),
                    Value = rnd.Next(1000000, 10000000) / 1000.0
                })
            };

            Func<string, string> pathConverter = s => s.Replace("/", "\\");

            var constructor = new RazorGridConstructor();
            constructor.Init(new Dictionary<string, object> { { "RESOLVE_PATH_ACTION", pathConverter } });
            var tables = constructor.CreateTables(@"Content\SimpleReport.cshtml", model);

            try
            {
                try
                {
                    SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                }
                catch
                {
                }

                var printer = new XlsxPrinter();

                using (var stream = new FileStream("test.xlsx", FileMode.Create, FileAccess.ReadWrite))
                {
                    printer.Print(stream, tables);

                    Assert.AreNotEqual(stream.Length, 0);
                }

                Assert.IsTrue(File.Exists("test.xlsx"));
                Assert.IsTrue(File.ReadAllBytes("test.xlsx").Length > 1024);
            }
            finally
            {
                if (File.Exists("test.xlsx"))
                {
                    //File.Delete("test.xlsx");
                }
            }
        }
    }
}

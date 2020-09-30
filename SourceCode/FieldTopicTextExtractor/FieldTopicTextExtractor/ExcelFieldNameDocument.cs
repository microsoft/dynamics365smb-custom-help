using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FieldTopicTextExtractor
{
    public class ExcelFieldNameDocument : IFieldNameDocument
    {
        public List<FieldTopicInfo> FieldTopics { get; set; }

        public ExcelFieldNameDocument()
        {
            FieldTopics = new List<FieldTopicInfo>();
        }

        public void Save(string file)
        {
            XLWorkbook wb = new XLWorkbook();
            IXLWorksheet ws = wb.Worksheets.Add("FieldTopicText");

            // Column formatting
            IXLColumn col1 = ws.Column("A");
            col1.Width = 15;
            IXLColumn col2 = ws.Column("B");
            col2.Width = 170;

            // Headers
            ws.Cell("A1").Value = "ID";
            ws.Cell("B1").Value = "Text";

            var rngHeader = ws.Range("A1:B1");
            rngHeader.SetAutoFilter();
            rngHeader.Style.Font.Bold = true;
            rngHeader.Style.Font.FontColor = XLColor.DarkBlue;

            int i = 2;

            foreach (FieldTopicInfo fieldTopic in FieldTopics)
            {
                ws.Cell("A" + i).Value = fieldTopic.Id;
                ws.Cell("B" + i).Value = fieldTopic.Text;
                i++;
            }
            wb.SaveAs(file);
        }
    }
}

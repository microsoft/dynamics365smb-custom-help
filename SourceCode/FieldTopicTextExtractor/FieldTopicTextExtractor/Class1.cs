using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

namespace FieldTopicTextExtractor
{
    public class ExcelTooltipDocument : ITooltipDocument
    {
    }
}

namespace ToolTips.Cmdlets
{
    /// <summary>
    /// Utilizes https://closedxml.codeplex.com/
    /// </summary>
    public class ExcelTooltipDocument : ITooltipDocument
    {
        public Dictionary<string, Tooltip> Tooltips { get; set; }

        private const string workbookname = "Tooltips";

        public ExcelTooltipDocument()
        {
            Tooltips = new Dictionary<string, Tooltip>();
        }

        public void Load(string file)
        {
            var wb = new XLWorkbook(file);
            var ws = wb.Worksheet(workbookname);

            int columns = ws.RowsUsed().Count();

            for (int i = 2; i < columns + 1; i++)
            {
                Status status = ws.Cell("A" + i).Value.ToString() == "Updated" ? Status.Updated : Status.New;
                string tooltipId = ws.Cell("B" + i).Value.ToString();
                string pageName = ws.Cell("C" + i).Value.ToString();
                string captionML = ws.Cell("D" + i).Value.ToString();
                string applicationArea = ws.Cell("E" + i).Value.ToString();

                string tooltipML = ws.Cell("F" + i).Value.ToString();
                tooltipML = String.IsNullOrEmpty(tooltipML) ? null : tooltipML;

                bool updated;
                Boolean.TryParse(ws.Cell("G" + i).Value.ToString(), out updated);

                Tooltips.Add(tooltipId, new Tooltip { Id = tooltipId, ControlName = captionML, Status = status, TooltipML = tooltipML, PageName = pageName, ApplicationArea = applicationArea, Updated = updated });
            }
        }

        public void Save(string file)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(workbookname);

            // Freeze panes
            ws.SheetView.Freeze(1, 7);

            // Column formatting
            var col1 = ws.Column("A");
            col1.Width = 12;
            var col2 = ws.Column("B");
            col2.Width = 20;
            var col3 = ws.Column("C");
            col3.Width = 20;
            var col4 = ws.Column("D");
            col4.Width = 30;
            var col5 = ws.Column("E");
            col5.Width = 18;
            var col6 = ws.Column("F");
            col6.Width = 90;
            var col7 = ws.Column("G");
            col7.Width = 10;
            var col8 = ws.Column("H");
            // Hide this column as it contains tooltip reference text for comparison.
            col8.Hide();

            // Headers
            ws.Cell("A1").Value = "Status";
            ws.Cell("B1").Value = "ID";
            ws.Cell("C1").Value = "Page Name";
            ws.Cell("D1").Value = "Control Name";
            ws.Cell("E1").Value = "Application Area";
            ws.Cell("F1").Value = "Tooltip";
            ws.Cell("G1").Value = "Updated";
            ws.Cell("H1").Value = "HiddenReferenceColumn";

            // Formatting headers
            var rngHeader = ws.Range("A1:H1");
            rngHeader.SetAutoFilter();
            rngHeader.Style.Font.Bold = true;
            rngHeader.Style.Font.FontColor = XLColor.DarkBlue;

            int i = 2;

            foreach (var tooltip in Tooltips.Values)
            {
                ws.Cell("A" + i).Value = tooltip.Status;
                ws.Cell("B" + i).Value = tooltip.Id;
                ws.Cell("C" + i).Value = tooltip.PageName;
                ws.Cell("D" + i).Value = tooltip.ControlName;
                ws.Cell("E" + i).Value = tooltip.ApplicationArea;
                ws.Cell("F" + i).Value = String.IsNullOrEmpty(tooltip.TooltipML) ? null : tooltip.TooltipML;
                ws.Cell("G" + i).FormulaA1 = string.Format("=F{0}<>H{0}", i);
                ws.Cell("H" + i).Value = String.IsNullOrEmpty(tooltip.TooltipML) ? null : tooltip.TooltipML;

                i++;
            }

            wb.SaveAs(file);
        }
    }
}

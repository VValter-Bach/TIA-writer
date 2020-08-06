using Graus.PlantComponents;
using System;
using System.IO;
using DocumentFormat.OpenXml.Office.Excel;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Bibliography;
using Graus.Data;

namespace Graus
{
    class ExelReader
    {
        SpreadsheetDocument excelDocument;

        public ExelReader(string filename)
        {
            excelDocument = SpreadsheetDocument.Open(filename, false);
        }

        private SheetData GetSheetData(string sheetName)
        {
            if (String.IsNullOrEmpty(sheetName)) throw new ArgumentException("Argument String is Null or Empty");

            WorkbookPart workbookPart = excelDocument.WorkbookPart;
            Sheet correspondingSheet = null;
            foreach (Sheets sheets in workbookPart.Workbook.Elements<Sheets>())
            {
                foreach (Sheet sheet in sheets)
                {
                    if (sheet.Name != sheetName) continue;
                    correspondingSheet = sheet;
                    break;
                }
            }

            if (correspondingSheet == null) throw new NullReferenceException("Could not locate Sheet with name: \"" + sheetName + "\"");

            Worksheet workSheet = (workbookPart.GetPartById(correspondingSheet.Id) as WorksheetPart).Worksheet;

            foreach (var sheetData in workSheet.Elements<SheetData>()) return sheetData;

            throw new Exception("Could not locate a DataSheet related to \"" + sheetName + "\"");
        }

        private string ReadCell(Cell cell)
        {
            if (cell == null) throw new ArgumentNullException("Cell cannot be Null");
            if (cell.CellValue == null) throw new Exception("Cell has no Value");
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                return excelDocument.WorkbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(Convert.ToInt32(cell.CellValue.Text)).InnerText;
            else
                return cell.CellValue.InnerText;

        }

        private ProcessCell FindProcessCell(PLC plc, string name)
        {
            foreach(var pc in plc.ProcessCells)
            {
                if (pc.Name == name) return pc;
            }
            throw new Exception("Not found ProcessCell" + name);
        }

        private string ReadCoordinates(SheetData page, int rowIndex, char column)
        {
            foreach (var row in page.Elements<Row>())
            {
                if (row.RowIndex != rowIndex) continue;
                foreach (var cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value[0] == column) return ReadCell(cell);
                }
            }
            throw new Exception("Could not find");
        }

        public Unit Read()
        {
            string name;
            bool emptySlots;
            var page = GetSheetData("Overview");
            name = ReadCoordinates(page, 1, 'B');
            emptySlots = Convert.ToBoolean(ReadCoordinates(page, 2, 'B'));
            var unit = new Unit(name, emptySlots);
            PLC plc = null;
            page = GetSheetData("Structure");
            foreach (var row in page.Elements<Row>())
            {
                ProcessCell pc = null;
                string defaultname = null;
                foreach (var cell in row.Elements<Cell>())
                {
                    if(row.RowIndex == 1)
                    {
                        if (cell.CellReference.Value[0] == 'A')
                        {
                            var plcData = ReadCell(cell);
                            var datas = plcData.Split(';');
                            plc = unit.AddPLC(datas[1],datas[0]);
                        }
                        else
                        {
                            if(!string.IsNullOrEmpty(ReadCell(cell))) plc.AddProcessCell(ReadCell(cell));
                        }
                    }
                    else
                    {
                        if (cell.CellReference.Value[0] == 'A') 
                        {
                            var values = ReadCell(cell).Split(';');
                            pc = FindProcessCell(plc, values[0]);
                            defaultname = values[1];

                        }

                        else
                        {
                            if (!string.IsNullOrEmpty(ReadCell(cell)))
                            {
                                var data = ReadCell(cell).Split(';');
                                if (data.Length == 3) pc.AddEquipmentModule(data[2], data[1], data[0]);
                                else pc.AddEquipmentModule(String.Format("{0}_{1}", defaultname, pc.EquipmentModules.Count + 1), data[1], data[0]);
                            }
                        }
                    }
                }
            }

            EMCMF.ReadSettings();

            foreach (var pc in plc.ProcessCells)
            {
                foreach (var em in pc.EquipmentModules)
                {
                    foreach(var cm in EMCMF.emf[em.Type])
                    {
                        em.AddControllModule(cm.CM, String.Format("{0} {1} {2}",pc.Name, em.Name, cm.Function));
                    }
                }
            }
            return unit;
        }
    }
}

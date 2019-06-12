/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.DataHandler.Filter;
using Mtd.OrderMaker.Web.DataHandler.Stack;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Mtd.OrderMaker.Web.Controllers.Index
{
    [Route("api/action/index")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class ActionController : ControllerBase
    {

        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ActionController(OrderMakerContext context, UserManager<WebAppUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("excel/export")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostExportAsync()
        {
            var data = Request.Form["InputFormForExport"];
            if (data.FirstOrDefault() == null || data.FirstOrDefault().Equals(string.Empty)) return NotFound();
            string idForm = data.FirstOrDefault();

            var user = await _userManager.GetUserAsync(User);
            FilterHandler handlerFilter = new FilterHandler(_context, idForm, user);
            MtdFilter mtdFilter = await handlerFilter.GetFilterAsync();
            if (mtdFilter == null) return NotFound();
            Incomer incomer = await handlerFilter.GetIncomerDataAsync();

            TypeQuery typeQuery = await handlerFilter.GetTypeQueryAsync();
            incomer.PageSize = 1000;

            OutFlow outFlow = await handlerFilter.GetStackFlowAsync(incomer, typeQuery);
            IList<MtdStore> mtdStore = outFlow.MtdStores;

            IList<string> storeIds = mtdStore.Select(s => s.Id).ToList();
            IList<string> fieldIds = incomer.FieldForColumn.Select(x => x.Id).ToList();

            StackHandler handlerStack = new StackHandler(_context);
            IList<MtdStoreStack> mtdStoreStack = await handlerStack.GetStackAsync(storeIds, fieldIds);
            IList<MtdFormPartField> columns = incomer.FieldForColumn.Where(x => x.MtdSysType != 8 && x.MtdSysType != 7).ToList();            
            return await Export(mtdStore, columns, mtdStoreStack);
        }

        private async Task<IActionResult> Export(IList<MtdStore> mtdStores, IList<MtdFormPartField> partFields, IList<MtdStoreStack> storeStack)
        {
            string prefixFile = new Random().Next().ToString();
            string sWebRootFolder = $"{_hostingEnvironment.WebRootPath}/files";
            string sFileName = $"{prefixFile}OrderMaker.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet1 = workbook.CreateSheet("Sheet1");                
                var rowIndex = 0;
                var colIndex = 0;
                IRow rowTitle = sheet1.CreateRow(rowIndex);
                rowTitle.CreateCell(0).SetCellValue("ID");
                foreach (var field in partFields)
                {
                    sheet1.AutoSizeColumn(colIndex);
                    colIndex++;
                    rowTitle.CreateCell(colIndex).SetCellValue(field.Name);
                }

                colIndex = 0;
                rowIndex++;
                foreach (var store in mtdStores)
                {
                    IRow row = sheet1.CreateRow(rowIndex);
                    row.CreateCell(colIndex).SetCellValue(store.Sequence.ToString("D9"));
                    foreach (var field in partFields)
                    {
                        colIndex++;
                        MtdStoreStack stack = storeStack.FirstOrDefault(x => x.MtdStore == store.Id && x.MtdFormPartField == field.Id);
                        ICell cell = row.CreateCell(colIndex);
                        SetValuefoCell(stack, field, cell);                        
                    }
                    colIndex = 0;
                    rowIndex++;
                }
                
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            file.Delete();
            memory.Position = 0;            
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName.Replace(prefixFile,""));
        }

        private void SetValuefoCell(MtdStoreStack stack, MtdFormPartField field, ICell cell)
        {

            switch (field.MtdSysType)
            {
                case 2:
                    {
                        int result = 0;
                        if (stack != null && stack.MtdStoreStackInt != null)
                        {
                            result = stack.MtdStoreStackInt.Register;
                        };

                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(result);
                        break;
                    }
                case 3:
                    {
                        double result = 0.00;
                        if (stack != null && stack.MtdStoreStackDecimal != null)
                        {
                            result = (double)stack.MtdStoreStackDecimal.Register;
                        }
                        cell.SetCellType(CellType.Numeric);
                        cell.SetCellValue(result);
                        break;
                    }
                case 5:
                    {

                        // cell.SetCellType(CellType.String);
                        bool check = false;
                        if (stack != null && stack.MtdStoreStackDate != null)
                        {
                            check = true;
                            cell.SetCellValue(stack.MtdStoreStackDate.Register.Date);
                        }
                        if (!check)
                        {
                            cell.SetCellValue(0);
                        }
                        break;
                    }
                case 6:
                    {
                        bool check = false;
                        if (stack != null && stack.MtdStoreStackDate != null)
                        {
                            check = true;
                            cell.SetCellValue(stack.MtdStoreStackDate.Register);
                        }
                        if (!check)
                        {
                            cell.SetCellValue(0);
                        }
                        break;
                    }
                case 10:
                    {
                        bool check = false;
                        if (stack != null && stack.MtdStoreStackDate != null)
                        {
                            check = true;
                            cell.SetCellValue(stack.MtdStoreStackDate.Register);
                        }
                        if (!check)
                        {
                            cell.SetCellValue(0);
                        }
                        break;
                    }
                case 11:
                    {
                        string result = "";
                        if (stack != null && stack.MtdStoreLink != null)
                        {
                            result = stack.MtdStoreLink.Register;
                        }
                        cell.SetCellType(CellType.String);
                        cell.SetCellValue(result);
                        break;
                    }
                case 12:
                    {
                        int result = 0;
                        if (stack != null && stack.MtdStoreStackInt != null)
                        {
                            result = stack.MtdStoreStackInt.Register;
                        }
                        cell.SetCellType(CellType.Boolean);
                        cell.SetCellValue(result);
                        break;
                    }
                default:
                    {
                        string result = "";
                        if (stack != null && stack.MtdStoreStackText != null)
                        {
                            result = stack.MtdStoreStackText.Register;
                        }
                        cell.SetCellValue(result);
                        break;
                    }
            }

        }

    }
}
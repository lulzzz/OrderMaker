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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Controllers.Store
{
    [Route("api/store")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DataController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserHandler _userHandler;

        public DataController(OrderMakerContext context, UserHandler userHandler)
        {
            _context = context;
            _userHandler = userHandler;
        }

        // POST: api/store/save
        [HttpPost("save")]
        public async Task<IActionResult> OnPostSaveAsync()
        {
            string Id = Request.Form["idStore"];

            MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.Id == Id);
            if (mtdStore == null)
            {
                return NotFound();
            }

            WebAppUser webAppUser = await _userHandler._userManager.GetUserAsync(HttpContext.User);
            bool isRight = await _userHandler.IsRight(webAppUser, RightsType.Edit, mtdStore.MtdForm);
            if (!isRight)
            {
                return Ok(403);
            }

            OutData outData = await CreateData(Id);
            List<MtdStoreStack> stackNew = outData.MtdStoreStacks;

            IList<MtdStoreStack> stackOld = await _context.MtdStoreStack
                .Include(m => m.MtdStoreStackText)
                .Include(m => m.MtdStoreStackDecimal)
                .Include(m => m.MtdStoreStackFile)
                .Include(m => m.MtdStoreStackDate)
                .Include(m => m.MtdStoreStackInt)
                .Include(m => m.MtdStoreLink)
                .Where(x => x.MtdStore == Id).ToListAsync();

            foreach (MtdStoreStack stack in stackOld)
            {
                MtdStoreStack stackForField = stackNew.SingleOrDefault(x => x.MtdFormPartField == stack.MtdFormPartField);

                stack.MtdStoreStackText = stackForField.MtdStoreStackText;
                stack.MtdStoreLink = stackForField.MtdStoreLink;
                stack.MtdStoreStackDate = stackForField.MtdStoreStackDate;
                stack.MtdStoreStackDecimal = stackForField.MtdStoreStackDecimal;
                stack.MtdStoreStackFile = stackForField.MtdStoreStackFile;
                stack.MtdStoreStackInt = stackForField.MtdStoreStackInt;
            }


            try
            {
                if (stackNew.Count > 0)
                {
                    int count = await _context.MtdStoreLink.Where(x => x.MtdStore == Id).CountAsync();
                    if (count > 0)
                    {
                        string titleText = outData.MtdStoreStacks.FirstOrDefault(x => x.MtdFormPartField == outData.MtdFormPartField.Id).MtdStoreStackText.Register;
                        IList<MtdStoreLink> linkIds = await _context.MtdStoreLink.Where(x => x.MtdStore == Id).Select(x => new MtdStoreLink { Id = x.Id, MtdStore = x.MtdStore, Register = titleText }).ToListAsync();
                        _context.MtdStoreLink.UpdateRange(linkIds);
                    }

                    _context.MtdStoreStack.UpdateRange(stackOld);


                    List<MtdStoreStack> stackNewOnly = stackNew.Where(x => !stackOld.Select(f => f.MtdFormPartField).Contains(x.MtdFormPartField)).ToList();

                    if (stackNewOnly.Any())
                    {
                        await _context.MtdStoreStack.AddRangeAsync(stackNewOnly);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MtdStoreExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();

        }

        [HttpPost("create")]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            string idForm = Request.Form["idForm"];
            string idFormParent = Request.Form["store-parent-id"];

            WebAppUser webAppUser = await _userHandler._userManager.GetUserAsync(HttpContext.User);
            bool isRight = await _userHandler.IsRight(webAppUser, RightsType.Create, idForm);
            if (!isRight)
            {
                return Ok(403);
            }

            await _context.Database.BeginTransactionAsync();
            int sequence;
            var forms = await _context.MtdStore.Where(x => x.MtdForm == idForm).ToListAsync();
            if (forms != null && forms.Count > 0) { sequence = forms.Max(x => x.Sequence); } else sequence = 0;
            sequence++;

            MtdStore mtdStore = new MtdStore { MtdForm = idForm, Sequence = sequence, Parent = idFormParent.Length > 0 ? idFormParent : null };
            await _context.MtdStore.AddAsync(mtdStore);
            await _context.SaveChangesAsync();
            OutData outParam = await CreateData(mtdStore.Id);
            List<MtdStoreStack> stackNew = outParam.MtdStoreStacks;
            await _context.MtdStoreStack.AddRangeAsync(stackNew);
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return Ok(mtdStore);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            string idStore = Request.Form["store-delete-id"];
            MtdStore mtdStore = await _context.MtdStore.FindAsync(idStore);

            if (mtdStore == null)
            {
                return NotFound();
            }

            WebAppUser webAppUser = await _userHandler._userManager.GetUserAsync(HttpContext.User);
            bool isRight = await _userHandler.IsRight(webAppUser, RightsType.Delete, mtdStore.MtdForm);
            if (!isRight)
            {
                return Ok(403);
            }


            _context.MtdStore.Remove(mtdStore);
            await _context.SaveChangesAsync();

            return Ok();
        }



        [HttpPost("number/id")]
        public async Task<IActionResult> OnPostGetIDAsync()
        {
            string result = "";
            string idFormPart = Request.Form["idFromParant"];
            string parentNumber = Request.Form["store-parent-number"];

            if (parentNumber.Length == 0) { return Ok(result); }

            parentNumber = parentNumber.TrimStart(new char[] { '0' });
            bool isOk = int.TryParse(parentNumber, out int num);


            if (!isOk) { return Ok(result); }

            MtdStore mtdStore = await _context.MtdStore.FirstOrDefaultAsync(x => x.MtdForm == idFormPart & x.Sequence == num);

            if (mtdStore != null)
            {
                result = mtdStore.Id;
            }

            return Ok(result);
        }

        private async Task<OutData> CreateData(string Id)
        {

            var store = await _context.MtdStore
               .Include(m => m.MtdFormNavigation)
               .ThenInclude(p => p.MtdFormPart)
               .FirstOrDefaultAsync(m => m.Id == Id);

            List<string> partsIds = store.MtdFormNavigation.MtdFormPart.Select(s => s.Id).ToList();

            var fields = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                .Where(x => partsIds.Contains(x.MtdFormPartNavigation.Id))
                .OrderBy(x => x.MtdFormPartNavigation.Sequence)
                .ThenBy(x => x.Sequence)
                .ToListAsync();

            var titleField = fields.FirstOrDefault(x => x.MtdSysType == 1);

            List<MtdStoreStack> stackNew = new List<MtdStoreStack>();

            foreach (MtdFormPartField field in fields)
            {
                var data = Request.Form[field.Id];
                MtdStoreStack mtdStoreStack = new MtdStoreStack()
                {
                    MtdStore = Id,
                    MtdFormPartField = field.Id
                };


                switch (field.MtdSysType)
                {
                    case 2:
                        {
                            if (data.FirstOrDefault() != string.Empty)
                            {
                                int result = int.Parse(data.FirstOrDefault());
                                mtdStoreStack.MtdStoreStackInt = new MtdStoreStackInt { Register = result };
                            }
                            break;
                        }
                    case 3:
                        {
                            if (data.FirstOrDefault() != string.Empty)
                            {
                                decimal result;
                                try
                                {
                                    string separ = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                                    result = decimal.Parse(data.FirstOrDefault().Replace(".", separ));
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }

                                mtdStoreStack.MtdStoreStackDecimal = new MtdStoreStackDecimal { Register = result };
                            }
                            break;
                        }
                    case 5:
                    case 6:
                    case 10:
                        {
                            if (data.FirstOrDefault() != string.Empty)
                            {
                                DateTime dateTime = DateTime.Parse(data.FirstOrDefault());
                                mtdStoreStack.MtdStoreStackDate = new MtdStoreStackDate { Register = dateTime };
                            }
                            break;
                        }
                    case 7:
                    case 8:
                        {

                            var actionDelete = Request.Form[$"{field.Id}-delete"];

                            if (actionDelete.FirstOrDefault() == null || actionDelete.FirstOrDefault() == "false")
                            {

                                IFormFile file = Request.Form.Files.FirstOrDefault(x => x.Name == field.Id);

                                if (file != null)
                                {
                                    byte[] streamArray = new byte[file.Length];
                                    await file.OpenReadStream().ReadAsync(streamArray, 0, streamArray.Length);
                                    mtdStoreStack.MtdStoreStackFile = new MtdStoreStackFile()
                                    {
                                        Register = streamArray,
                                        FileName = file.FileName,
                                        FileSize = streamArray.Length,
                                        FileType = file.ContentType
                                    };

                                }

                                if (file == null)
                                {
                                    MtdStoreStack stackOld = await _context.MtdStoreStack
                                        .Include(m => m.MtdStoreStackFile)
                                        .OrderByDescending(x => x.Id)
                                        .FirstOrDefaultAsync(x => x.MtdStore == Id & x.MtdFormPartField == field.Id);

                                    if (stackOld != null && stackOld.MtdStoreStackFile != null)
                                    {

                                        mtdStoreStack.MtdStoreStackFile = new MtdStoreStackFile()
                                        {
                                            FileName = stackOld.MtdStoreStackFile.FileName,
                                            FileSize = stackOld.MtdStoreStackFile.FileSize,
                                            Register = stackOld.MtdStoreStackFile.Register,
                                            FileType = stackOld.MtdStoreStackFile.FileType,

                                        };
                                    }
                                }
                            }

                            break;
                        }

                    case 11:
                        {
                            if (data.FirstOrDefault() != string.Empty)
                            {
                                string datalink = Request.Form[$"{field.Id}-datalink"];
                                mtdStoreStack.MtdStoreLink = new MtdStoreLink { MtdStore = data.FirstOrDefault(), Register = datalink };
                            }

                            break;
                        }

                    case 12:
                        {
                            bool check = bool.Parse(data.FirstOrDefault());
                            mtdStoreStack.MtdStoreStackInt = new MtdStoreStackInt { Register = check ? 1 : 0 };
                            break;
                        }

                    default:
                        {
                            if (data.FirstOrDefault() != string.Empty)
                            {
                                mtdStoreStack.MtdStoreStackText = new MtdStoreStackText() { Register = data.FirstOrDefault() };
                            }
                            break;
                        }
                }

                stackNew.Add(mtdStoreStack);
            }

            OutData outParam = new OutData()
            {
                MtdFormPartField = titleField,
                MtdStoreStacks = stackNew,
            };

            return outParam;
        }

        private bool MtdStoreExists(string id)
        {
            return _context.MtdStore.Any(e => e.Id == id);
        }

    }

    public class OutData
    {
        public List<MtdStoreStack> MtdStoreStacks { get; set; }
        public MtdFormPartField MtdFormPartField { get; set; }
    }
}

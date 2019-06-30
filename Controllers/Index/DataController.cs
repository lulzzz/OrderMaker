/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Controllers.Index
{
    [Route("api/index")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class DataController : ControllerBase
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;


        public DataController(OrderMakerContext context, UserManager<WebAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("search/text")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostSearchTextAsync()
        {
            var form = Request.Form["indexForm"];
            var value = Request.Form["indexSearchText"];
            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == form);
            bool old = true;
            if (filter == null)
            {
                old = false;
                filter = new MtdFilter { IdUser = user.Id, MtdForm = form };
            }

            filter.SearchNumber = "";
            filter.SearchText = value;
            filter.Page = 1;

            if (old)
            {
                _context.MtdFilter.Update(filter);
            }
            else
            {
                await _context.MtdFilter.AddAsync(filter);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();
        }

        [HttpPost("search/number")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostSerarchIndexAsync()
        {

            string form = Request.Form["formId"];
            string value = Request.Form["searchNumber"];

            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == form);
            bool old = true;
            if (filter == null)
            {
                old = false;
                filter = new MtdFilter { IdUser = user.Id, MtdForm = form };
            }

            filter.SearchNumber = value;
            filter.Page = 1;
            filter.SearchText = "";

            if (old)
            {
                _context.MtdFilter.Update(filter);
            }
            else
            {
                await _context.MtdFilter.AddAsync(filter);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }
            return Ok();
        }

        [HttpPost("{idForm}/pagesize/{number}")]
        public async Task<IActionResult> PostPageSize(string idForm, int number)
        {
            int temp = number;
            if (temp > 50) temp = 50;
            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == idForm);
            if (filter == null)
            {
                filter = new MtdFilter { SearchNumber = "", SearchText = "" };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            filter.PageSize = number;
            _context.MtdFilter.Update(filter);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("pagemove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostPageMove()
        {
            /* number
             * 1 -  First Page; 2 - back; 3 - forward;
             */

            string idForm = Request.Form["formId"];
            string formValue = Request.Form["formValue"];
            int number = int.Parse(formValue);

            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == idForm);
            if (filter == null)
            {
                filter = new MtdFilter { SearchNumber = "", SearchText = "" };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            int page = filter.Page;

            switch (number)
            {
                case 2: { if (page > 1) { page--; } break; }
                case 3: { page++; break; }
                default: { page = 1; break; }
            };

            filter.Page = page < 0 ? page = 1 : page;

            _context.MtdFilter.Update(filter);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("filter/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterAddAsync()
        {

            var idForm = Request.Form["indexInputForm"];
            var valueFilter = Request.Form["indexInputFilter"];
            var valueField = Request.Form["indexInputField"];
            var valueTerm = Request.Form["indexInputTerm"];
            var valueFieldList = Request.Form[$"{valueField}-inputlist"];

            string result = "";
            if (valueFilter.FirstOrDefault() != null) { result = valueFilter; }
            if (valueFieldList.FirstOrDefault() != null) { result = valueFieldList; }

            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == idForm);

            if (filter == null)
            {
                filter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = idForm,
                    SearchNumber = "",
                    SearchText = "",
                    Page = 1,
                    PageSize = 10
                };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            var term = int.Parse(valueTerm);

            MtdFilterField field = new MtdFilterField { MtdFilter = filter.Id, MtdFormPartField = valueField, MtdTerm = term, Value = result };

            try
            {
                await _context.MtdFilterField.AddAsync(field);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();

        }

        [HttpPost("filter/remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterRemoveAsync()
        {
            string strID = Request.Form["idField"];
            long idField = long.Parse(strID);
            MtdFilterField mtdFilterField = new MtdFilterField { Id = idField };

            try
            {
                _context.MtdFilterField.Remove(mtdFilterField);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();

        }

        [HttpPost("filter/removeAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterRemoveAllAsync()
        {
            string strID = Request.Form["idFilter"];
            int idFilter = int.Parse(strID);

            IList<MtdFilterField> mtdFilterFields = await _context.MtdFilterField.Where(x => x.MtdFilter == idFilter).ToListAsync();

            try
            {
                _context.MtdFilterField.RemoveRange(mtdFilterFields);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();

        }

        [HttpPost("filter/columns/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostFilterColumnsAsync()
        {

            var idForm = Request.Form["indexDataColumnIdForm"];
            var data = Request.Form["indexDataColumnList"];

            List<string> fieldIds = new List<string>();
            if (data.FirstOrDefault() != null && data.FirstOrDefault().Length > 0) fieldIds = data.FirstOrDefault().Split(",").ToList();
            var user = await _userManager.GetUserAsync(User);
            MtdFilter filter = await _context.MtdFilter.Include(m => m.MtdFilterColumn).FirstOrDefaultAsync(x => x.IdUser == user.Id & x.MtdForm == idForm);

            if (filter == null)
            {
                filter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = idForm,
                    SearchNumber = "",
                    SearchText = "",
                    Page = 1,
                    PageSize = 10
                };
                await _context.MtdFilter.AddAsync(filter);
                await _context.SaveChangesAsync();
            }

            List<MtdFilterColumn> columns = new List<MtdFilterColumn>();
            int seq = 0;
            foreach (string field in fieldIds.Where(x => x != ""))
            {
                seq++;
                columns.Add(new MtdFilterColumn
                {
                    MtdFilter = filter.Id,
                    MtdFormPartField = field,
                    Sequence = seq
                });
            }


            try
            {
                if (filter.MtdFilterColumn != null)
                {
                    _context.MtdFilterColumn.RemoveRange(filter.MtdFilterColumn);
                    await _context.SaveChangesAsync();
                }

                await _context.MtdFilterColumn.AddRangeAsync(columns);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex.InnerException; }


            return Ok();

        }


        [HttpPost("waitlist/set")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostWaitListSetAsync()
        {
            string idForm = Request.Form["id-form-waitlist"];
            WebAppUser user = await _userManager.GetUserAsync(HttpContext.User);
            MtdFilter mtdFilter = await _context.MtdFilter.Where(x => x.IdUser == user.Id && x.MtdForm == idForm).FirstOrDefaultAsync();
            if (mtdFilter == null)
            {
                mtdFilter = new MtdFilter
                {
                    IdUser = user.Id,
                    MtdForm = idForm,
                    PageSize = 10,
                    SearchText = "",
                    SearchNumber = "",
                    Page = 1,
                    WaitList = 1,
                };
                await _context.MtdFilter.AddAsync(mtdFilter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            mtdFilter.WaitList = mtdFilter.WaitList == 0 ? 1 : 0;
            _context.MtdFilter.Update(mtdFilter);
            await _context.SaveChangesAsync();

            return Ok();

        }
    }
}

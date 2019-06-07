﻿/*
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Models.Index;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index
{
    [ViewComponent(Name = "IndexFilterColumns")]
    public class Columns : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public Columns(OrderMakerContext orderMakerContext, UserManager<WebAppUser> userManager)
        {
            _context = orderMakerContext;
            _userManager = userManager;
        }
        

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {
           

            var user = await _userManager.GetUserAsync(HttpContext.User);
            IList<MtdFilterColumn> mtdFilterColumns = await _context.MtdFilterColumn
                .Include(m => m.MtdFormPartFieldNavigation)
                .Include(x=>x.MtdFilterNavigation)
                .Where(x => x.MtdFilterNavigation.IdUser == user.Id && x.MtdFilterNavigation.MtdForm == idForm)
                .OrderBy(x=>x.Sequence)
                .ToListAsync();   

            IList<MtdFormPartField> mtdFormPartFields = await _context.MtdFormPartField
                .Include(x => x.MtdFormPartNavigation)
                .Where(x => x.MtdFormPartNavigation.MtdForm == idForm)
                .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o => o.Sequence)
                .ToListAsync();

            IList<MtdFormPart> mtdFormParts = mtdFormPartFields.GroupBy(x => x.MtdFormPartNavigation.Id)
                .Select(g => g.FirstOrDefault(x=>x.MtdFormPartNavigation.Id==g.Key).MtdFormPartNavigation)
                .OrderBy(x=>x.Sequence)
                .ToList();
                                              

            ColumnsModelView fieldsModelView = new ColumnsModelView
            {
                IdForm = idForm,
                MtdFormParts = mtdFormParts,
                MtdFilterColumns = mtdFilterColumns,
                MtdFormPartFields = mtdFormPartFields
            };            

            return View(fieldsModelView);
        }
    }
}
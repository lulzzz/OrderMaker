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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Models.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Index.Filter
{
    [ViewComponent(Name = "IndexFilterSelector")]
    public class Selector : ViewComponent
    {
        private readonly OrderMakerContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public Selector(OrderMakerContext orderMakerContext, UserManager<WebAppUser> userManager)
        {
            _context = orderMakerContext;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string idForm)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            MtdFilter filter = await _context.MtdFilter.FirstOrDefaultAsync(x => x.IdUser == user.Id && x.MtdForm == idForm);

            var query = _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                    .Where(x => x.MtdFormPartNavigation.MtdForm == idForm & x.MtdSysType != 7 & x.MtdSysType != 8 & x.Active)
                    .OrderBy(x => x.MtdFormPartNavigation.Sequence).ThenBy(x => x.Sequence);

            IList<MtdFormPartField> mtdFields;
            if (filter != null)
            {
                List<string> fieldIds = await _context.MtdFilterField.Where(x => x.MtdFilter == filter.Id)
                    .Select(x => x.MtdFormPartField).ToListAsync();
                mtdFields = await query.Where(x => !fieldIds.Contains(x.Id)).ToListAsync();

            }
            else
            {
                mtdFields = await query.ToListAsync();
            }
            
            IList<MtdSysTerm> mtdSysTerms = await _context.MtdSysTerm.ToListAsync();
            List<SelectorList> storeList = new List<SelectorList>();
            foreach (var field in mtdFields.Where(x => x.MtdSysType == 11).ToList())
            {
                string idFormForList = await _context.MtdFormList.Where(x => x.Id == field.Id).Select(x=>x.MtdForm).FirstOrDefaultAsync();
                MtdFormPartField fieldForList = await _context.MtdFormPartField.Include(m => m.MtdFormPartNavigation)
                        .Where(x => x.MtdFormPartNavigation.MtdForm == idFormForList & x.MtdSysType==1)
                        .OrderBy(o => o.MtdFormPartNavigation.Sequence).ThenBy(o=>o.Sequence).FirstOrDefaultAsync();
                               
                List<SelecorStore> selecorStores = await _context.MtdStoreStack
                                .Include(x => x.MtdStoreStackText).Where(x => x.MtdFormPartField == fieldForList.Id)
                                .Select(x => new SelecorStore { IdStore = x.MtdStore, Result = x.MtdStoreStackText.Register })
                                .OrderBy(x => x.Result)
                                .ToListAsync();

                SelectorList selectorList = new SelectorList
                {
                     FieldAim = field,
                     FieldOut = fieldForList,
                     Store = selecorStores
                };

                storeList.Add(selectorList);

            }

            SelectorModelView selector = new SelectorModelView()
            {
                IdForm = idForm,
                MtdFormPartFields = mtdFields,
                MtdSysTerms = mtdSysTerms,
                StoreList = storeList,
            };
            return View("Default", selector);
        }
    }
}

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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Models.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Components.Store
{
    public enum FormType { Create, Edit, Details, Print }

    [ViewComponent(Name = "StoreForm")]
    public class Form : ViewComponent
    {
        private readonly OrderMakerContext _context;

        public Form(OrderMakerContext orderMakerContext)
        {
            _context = orderMakerContext;
        }

        private async Task<IList<MtdFormPart>> GetPartsAsync(string idForm)
        {
            IList<MtdFormPart> result = await _context.MtdFormPart
                .Include(m => m.MtdFormPartHeader)
                .Where(x => x.MtdForm == idForm)
                .OrderBy(s => s.Sequence)
                .ToListAsync();

            return result;
        }

        private async Task<IList<MtdFormPartField>> GetFieldsAsync(IList<MtdFormPart> formParts)
        {
            IList<string> partIds = formParts.Select(x => x.Id).ToList();

            return await _context.MtdFormPartField
                .Include(x => x.MtdFormList)
                .Where(x => partIds.Contains(x.MtdFormPart))
                .OrderBy(s => s.Sequence)
                .ToListAsync();
        }

        private async Task<DataSet> CreateDataSetAsync(MtdStore store)
        {
            if (store == null) return null;

            IList<MtdFormPart> mtdFormParts = await GetPartsAsync(store.MtdForm);
            IList<MtdFormPartField> mtdFormPartFields = await GetFieldsAsync(mtdFormParts);

            var mtdStore = await _context.MtdStore
                .Include(m=>m.ParentNavigation)
                .Include(m => m.MtdFormNavigation)
                .ThenInclude(m => m.MtdFormHeader)
                .Include(m => m.MtdFormNavigation)
                .ThenInclude(m => m.ParentNavigation)
                .FirstOrDefaultAsync(m => m.Id == store.Id);


            var query = from n in _context.MtdStoreStack
                    .Where(x => x.MtdStore == mtdStore.Id)
                        group n by n.MtdFormPartField into g
                        select new { idField = g.Key, idStack = g.FirstOrDefault(x => x.Id == g.Max(m => m.Id)).Id };

            IList<long> ids = await query.Select(x => x.idStack).ToListAsync();

            IList<MtdStoreStack> stack = await _context.MtdStoreStack
                .Include(m => m.MtdStoreStackText)
                .Include(m => m.MtdStoreStackDecimal)
                .Include(m => m.MtdStoreStackFile)
                .Include(m => m.MtdStoreStackDate)
                .Include(m => m.MtdStoreStackInt)
                .Include(m => m.MtdStoreLink)
               .Where(x => ids.Contains(x.Id))
               .ToListAsync();


            DataSet result = new DataSet()
            {
                Store = mtdStore,
                Parts = mtdFormParts,
                Fields = mtdFormPartFields,
                Stack = stack,
            };

            return result;

        }

        public async Task<IViewComponentResult> InvokeAsync(MtdStore store, FormType type = FormType.Details)
        {

            if (store == null)
            {
                return View();
            }

            if (type == FormType.Create)
            {
                store.MtdFormNavigation.MtdFormHeader = await _context.MtdFormHeader.FindAsync(store.MtdForm);
                store.MtdFormNavigation.ParentNavigation = await _context.MtdForm.FindAsync(store.MtdFormNavigation.Parent);

                IList<MtdFormPart> mtdFormParts = await GetPartsAsync(store.MtdForm);
                IList<MtdFormPartField> mtdFormPartFields = await GetFieldsAsync(mtdFormParts);

                DataSet dataSetForCreate = new DataSet()
                {
                    Store = store,
                    Parts = mtdFormParts,
                    Fields = mtdFormPartFields,
                    Stack = new List<MtdStoreStack>()
                };

                return View("Create", dataSetForCreate);
            }
            
            DataContainer dataContainer = new DataContainer
            {
                Owner = await CreateDataSetAsync(store),
                Parent = await CreateDataSetAsync(store.ParentNavigation)
            };

            return View(type.ToString(), dataContainer);

        }
    }
}
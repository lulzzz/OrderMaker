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
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using Mtd.OrderMaker.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.DataHandler.Filter
{
    public partial class FilterHandler
    {
        private readonly OrderMakerContext _context;
        private MtdFilter register;
        private WebAppUser _user;
        private IQueryable<MtdStore> queryMtdStore;
        private IList<Claim> _userRights;

        public string IdForm { get; private set; }        
        
        public FilterHandler(OrderMakerContext orderMakerContext, string idForm, WebAppUser user, IList<Claim> userRights)
        {            
            _context = orderMakerContext;
            _user = user;
            _userRights = userRights.Where(x=>x.Type == idForm).ToList();
            IdForm = idForm;
            queryMtdStore = _context.MtdStore;
        }

        public async Task<MtdFilter> GetFilterAsync()
        {            

            if (register == null)
            {
                register = await _context.MtdFilter.Include(m=>m.MtdFilterColumn).FirstOrDefaultAsync(x => x.IdUser == _user.Id && x.MtdForm == IdForm);
            }
            return register;
        }

        public async Task<Incomer> GetIncomerDataAsync()
        {
            MtdFilter mtdFilter = await GetFilterAsync();

            Incomer ps = new Incomer
            {
                IdForm = IdForm,
                SearchNumber = "",
                SearchText = "",
                Page = 1,
                PageSize = 10,
                FieldForColumn = await GetFieldsAsync()
        };

            if (mtdFilter != null)
            {
                ps.IdForm = IdForm;
                ps.SearchText = mtdFilter.SearchText;
                ps.SearchNumber = mtdFilter.SearchNumber;
                ps.PageSize = mtdFilter.PageSize;
                ps.Page = mtdFilter.Page;                
                ps.FieldForFilter = await GetAdvancedAsync();
            }

            return ps;

        }

        public async Task<TypeQuery> GetTypeQueryAsync()
        {

            TypeQuery typeQuery = TypeQuery.empty;
            bool filterField = false;

            MtdFilter mtdFilter = await GetFilterAsync();
            if (mtdFilter != null)
            {
                filterField = await _context.MtdFilterField.Where(x => x.MtdFilter == mtdFilter.Id).AnyAsync();
                if (mtdFilter.SearchNumber != "") { typeQuery = TypeQuery.number; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText != "" && !filterField) { typeQuery = TypeQuery.text; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText == "" && filterField) { typeQuery = TypeQuery.field; }
                if (mtdFilter.SearchNumber == "" && mtdFilter.SearchText != "" && filterField) { typeQuery = TypeQuery.textField; }
            }

            return typeQuery;
        }

        public async Task<IList<MtdFormPartField>> GetFieldsAsync()
        {

            IList<MtdFormPartField> result;

            MtdFilter mtdFilter = await GetFilterAsync();
            
            if (mtdFilter != null && mtdFilter.MtdFilterColumn != null && mtdFilter.MtdFilterColumn.Count > 0)
            {
                List<string> fIds = mtdFilter.MtdFilterColumn.Select(x => x.MtdFormPartField).ToList();
                IList<MtdFormPartField> tempFields = await _context.MtdFormPartField.Where(x => fIds.Contains(x.Id)).ToListAsync();

                result = (from s in tempFields
                                     join sa in mtdFilter.MtdFilterColumn on s.Id equals sa.MtdFormPartField
                                     orderby sa.Sequence
                                     select s).ToList();
            }
            else
            {
                result = await _context.MtdFormPartField
                    .Include(m => m.MtdFormPartNavigation)
                    .Where(x => x.MtdFormPartNavigation.MtdForm == IdForm & x.MtdSysType == 1)
                    .OrderBy(s => s.MtdFormPartNavigation.Sequence).ThenBy(s => s.Sequence)
                    .Take(3)
                    .ToListAsync();
            }

            return result;
        }

        public async Task<IList<MtdFilterField>> GetAdvancedAsync()
        {
            IList<MtdFilterField> result = null;

            MtdFilter mtdFilter = await GetFilterAsync();

            if (mtdFilter != null)
            {
                result = await _context.MtdFilterField.Include(m => m.MtdFormPartFieldNavigation).Where(x => x.MtdFilter == mtdFilter.Id).ToListAsync();
            }

            return result;
        }


    }
}

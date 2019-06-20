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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Services
{
    public enum RightsType
    {
        View, Create, Edit, Delete, ViewOwn, EditOwn, DeleteOwn
    };

    public class UserHandler
    {
        public readonly UserManager<WebAppUser> _userManager;

        public string RightCreate => "-create";
        public string RightView => "-view";
        public string RightEdit => "-edit";
        public string RightDelete => "-delete";
        public string RightViewOwn => "-view-own";
        public string RightEditOwn => "-edit-own";
        public string RightDeleteOwn => "-delete-own";        


        private List<string> rights;
        private OrderMakerContext _context;

        public ReadOnlyCollection<string> Rights => rights.AsReadOnly();


        public UserHandler(OrderMakerContext context, UserManager<WebAppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            rights = new List<string>()
            {RightView,RightCreate,RightDelete,RightEdit };
        }

        private string RightTypeToString(RightsType rightsType)
        {
            string value = $"-{rightsType.ToString().ToLower()}";
            value = value.Replace("own", "-own");
            return value;
        }


        public async Task<List<string>> GetFormIdsAsync(WebAppUser user, params RightsType[] rightsTypes)
        {
            List<string> formIds = new List<string>();

            foreach (RightsType rightsType in rightsTypes)
            {
                string right = RightTypeToString(rightsType);
                IList<Claim> claims = await _userManager.GetClaimsAsync(user);
                List<string> fids = claims.Where(x => x.Value == right).Select(x => x.Type).ToList();
                formIds.AddRange(fids.Where(x => !formIds.Contains(x)));
            }


            return formIds;
        }



        public async Task<bool> IsRightAsync(WebAppUser user, RightsType rightsType, string idForm)
        {
            bool result;

            string right = RightTypeToString(rightsType);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            result = claims.Where(x => x.Type == idForm && x.Value == right).Any();

            return result;
        }

        public async Task<bool> IsAdmin(WebAppUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsOwner(WebAppUser user, string idStore)
        {
            return await _context.MtdStoreOwner.Where(x => x.Id == idStore && x.UserId == user.Id).AnyAsync();
        }

        private async Task<bool> IsRights(string right, WebAppUser user, string idForm, string idStore = null)
        {

            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            bool isOk = claims.Where(x => x.Type == idForm && x.Value == right).Any();
            if (isOk) return true;

            bool ownRight = claims.Where(x => x.Type == idForm && x.Value == $"{right}-own").Any();
            if (ownRight && idStore != null)
            {
                bool isOkOwner = await _context.MtdStoreOwner.Where(x => x.Id == idStore & x.UserId == user.Id).AnyAsync();
                if (isOkOwner) return true;
            }

            return false;
        }

        public async Task<bool> IsCreator(WebAppUser user, string idForm, string idStore = null)
        {
            return await IsRights("-create", user, idForm);
        }

        public async Task<bool> IsViewer(WebAppUser user, string idForm, string idStore = null)
        {
            return await IsRights("-view", user, idForm, idStore);
        }

        public async Task<bool> IsEditor(WebAppUser user, string idForm, string idStore = null)
        {
            return await IsRights("-edit", user, idForm, idStore);
        }

        public async Task<bool> IsEraser(WebAppUser user, string idForm, string idStore = null)
        {
            return await IsRights("-delete", user, idForm, idStore);
        }
        
        public async Task<bool> IsCreatorPartAsync (WebAppUser user, string idPart)
        {
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(x => x.Type == idPart && x.Value == "-part-create").Any();        
        }

        public async Task<bool> IsEditorPartAsync(WebAppUser user, string idPart)
        {
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(x => x.Type == idPart && x.Value == "-part-edit").Any();
        }

        public async Task<bool> IsViewerPartAsync(WebAppUser user, string idPart)
        {
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(x => x.Type == idPart && x.Value == "-part-view").Any();
        }

        public async Task<List<string>> GetAllowPartsForView(WebAppUser user, string idForm)
        {
            List<string> idsAll = await _context.MtdFormPart.Where(x=>x.MtdForm==idForm).Select(x=>x.Id).ToListAsync();
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(x => idsAll.Contains(x.Type) && x.Value == "-part-view").Select(x=>x.Type).ToList();
            
        }

    }
}

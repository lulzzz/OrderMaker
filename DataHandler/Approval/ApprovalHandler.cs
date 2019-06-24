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

using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Web.Areas.Identity.Data;
using Mtd.OrderMaker.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.DataHandler.Approval
{
    public class ApprovalHandler
    {
        private readonly OrderMakerContext _context;
        private readonly string idStore;
        private MtdStore storeCache;
        private MtdApproval approvalCache;

        private async Task<MtdStore> GetStoreAsync()
        {
            if (storeCache != null) return storeCache;
            storeCache = await _context.MtdStore
                .Include(x => x.MtdFormNavigation)
                .Include(x => x.MtdStoreApproval)
                .Include(x => x.MtdStoreOwner)
                .Where(x => x.Id == idStore)
                .FirstOrDefaultAsync();
            return storeCache;
        }

        private async Task<MtdApproval> GetApproval()
        {
            if (approvalCache != null) return approvalCache;
            MtdStore mtdStore = await GetStoreAsync();
            return await _context.MtdApproval.Include(x => x.MtdApprovalStage).Where(x => x.MtdForm == mtdStore.MtdForm).FirstOrDefaultAsync();
        }

        public ApprovalHandler(OrderMakerContext context, string idstore)
        {
            _context = context;
            idStore = idstore;
            storeCache = null;
            approvalCache = null;
        }

        public void ClearCache()
        {
            approvalCache = null;
            storeCache = null;
        }

        public async Task<bool> IsApprovalFormAsync()
        {
            MtdApproval mtdApproval = await GetApproval();
            return mtdApproval == null ? false : true;
        }

        public async Task<MtdApprovalStage> GetCurrentStageAsync()
        {
            MtdApproval approval = await GetApproval();
            if (approval == null) return null;

            MtdStore mtdStore = await GetStoreAsync();
            if (mtdStore.MtdStoreApproval == null) return await GetFirstStageAsync();
            return approval.MtdApprovalStage.Where(x => x.Id == mtdStore.MtdStoreApproval.MtdApproveStage).FirstOrDefault();
        }

        public async Task<bool> IsApproverAsync(WebAppUser user)
        {
            bool isApprovalForm = await IsApprovalFormAsync();
            if (!isApprovalForm) { return false; }

            MtdApprovalStage mtdApprovalStage = await GetCurrentStageAsync();
            MtdStore store = await GetStoreAsync();
            bool forOwner = false;
            if (store.MtdStoreOwner != null)
            {
                forOwner = (store.MtdStoreOwner.UserId.Equals(user.Id) && mtdApprovalStage.Equals(await GetFirstStageAsync()));
            }

            if (mtdApprovalStage.UserId.Equals(user.Id) || forOwner) { return true; }

            return false;

        }

        public async Task<IList<MtdApprovalStage>> GetStagesAsync()
        {
            MtdApproval mtdApproval = await GetApproval();
            return mtdApproval.MtdApprovalStage.OrderBy(x => x.Stage).ToList();
        }

        public async Task<MtdApprovalStage> GetFirstStageAsync()
        {
            IList<MtdApprovalStage> mtdApprovalStages = await GetStagesAsync();
            return mtdApprovalStages.FirstOrDefault();
        }

        public async Task<MtdApprovalStage> GetLastStageAsync()
        {
            IList<MtdApprovalStage> mtdApprovalStages = await GetStagesAsync();
            return mtdApprovalStages.OrderByDescending(x => x.Stage).FirstOrDefault();
        }

        public async Task<MtdApprovalStage> GetNextStage()
        {
            MtdApprovalStage current = await GetCurrentStageAsync();
            IList<MtdApprovalStage> stages = await GetStagesAsync();
            return stages.Where(x => x.Stage > current.Stage).FirstOrDefault();

        }

        public async Task<bool> ActionApprove(WebAppUser webAppUser)
        {
            MtdStore mtdStore = await GetStoreAsync();
            if (mtdStore.MtdStoreApproval.Complete == 1) { return false; }
            MtdApprovalStage mtdApprovalStage = await GetNextStage();
            sbyte complete = 0;
            if (mtdApprovalStage == null) { complete = 1; mtdApprovalStage = await GetLastStageAsync(); };
            MtdStoreApproval storeApproval = new MtdStoreApproval
            {
                Id = mtdStore.Id,
                MtdApproveStage = mtdApprovalStage.Id,
                PartsApproved = mtdApprovalStage.BlockParts,
                Approved = 1,
                Complete = complete
            };

            if (mtdStore.MtdStoreApproval == null)
            {
                await _context.MtdStoreApproval.AddAsync(storeApproval);
            }
            else
            {
                _context.MtdStoreApproval.Update(storeApproval);
            }

            MtdLogApproval mtdLogApproval = new MtdLogApproval
            {
                MtdApprovalStage = mtdApprovalStage.Id,
                Result = 1,
                Timech = DateTime.Now,
                UserId = webAppUser.Id
            };

            await _context.MtdLogApproval.AddAsync(mtdLogApproval);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ActionReject(bool complete, int idStage, WebAppUser webAppUser)
        {
            //MtdStore mtdStore = await GetStoreAsync();
            //if (mtdStore.MtdStoreApproval.Complete == 1) { return false; }

            //MtdStoreApproval storeApproval = new MtdStoreApproval
            //{
            //    Id = mtdStore.Id,
            //    MtdApproveStage = idStage,
            //    PartsApproved = mtdApprovalStage.BlockParts,
            //    Approved = 1,
            //    Complete = complete
            //};

            //if (mtdStore.MtdStoreApproval == null)
            //{
            //    await _context.MtdStoreApproval.AddAsync(storeApproval);
            //}
            //else
            //{
            //    _context.MtdStoreApproval.Update(storeApproval);
            //}

            //MtdLogApproval mtdLogApproval = new MtdLogApproval
            //{
            //    MtdApprovalStage = mtdApprovalStage.Id,
            //    Result = 1,
            //    Timech = DateTime.Now,
            //    UserId = webAppUser.Id
            //};

            //await _context.MtdLogApproval.AddAsync(mtdLogApproval);

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch
            //{
            //    return false;
            //}

            return false;
        }
    }
}

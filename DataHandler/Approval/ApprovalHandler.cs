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

        public static async Task<List<string>> GetStoreIds(OrderMakerContext context, WebAppUser user)
        {
            List<int> stagesUser = await context.MtdApprovalStage.Where(x => x.UserId == user.Id).Select(x => x.Id).ToListAsync();
            return await context.MtdStoreApproval.Where(x => stagesUser.Contains(x.MtdApproveStage)).Select(x => x.Id).ToListAsync();
        }

        private async Task<MtdStore> GetStoreAsync()
        {
            if (storeCache != null) return storeCache;
            storeCache = await _context.MtdStore.AsNoTracking()
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
            return await _context.MtdApproval.AsNoTracking().Include(x => x.MtdApprovalStage).Where(x => x.MtdForm == mtdStore.MtdForm).FirstOrDefaultAsync();
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
            bool isComplete = await IsComplete();
            bool isApprovalForm = await IsApprovalFormAsync();            
            if (!isApprovalForm || isComplete) { return false; }

            MtdApprovalStage mtdApprovalStage = await GetCurrentStageAsync();
            MtdStore store = await GetStoreAsync();
            bool forOwner = false;
            if (store.MtdStoreOwner != null)
            {
                MtdApprovalStage firstStage = await GetFirstStageAsync();
                forOwner = (store.MtdStoreOwner.UserId.Equals(user.Id) && mtdApprovalStage.Id.Equals(firstStage.Id));
            }

            if (mtdApprovalStage.UserId.Equals(user.Id) || forOwner) { return true; }

            return false;

        }

        public async Task<bool> IsFirstStageAsync()
        {
            MtdApprovalStage first = await GetCurrentStageAsync();
            MtdApprovalStage current = await GetFirstStageAsync();
            return first.Id.Equals(current.Id);
        }

        public async Task<bool> IsComplete()
        {
            bool result = false;
            MtdStore mtdStore = await GetStoreAsync();
            if (mtdStore.MtdStoreApproval != null && mtdStore.MtdStoreApproval.Approved == 1)
            {
                result = true;
            }
            return result;
        }

        public async Task<IList<MtdApprovalStage>> GetStagesAsync()
        {
            MtdApproval mtdApproval = await GetApproval();
            return mtdApproval.MtdApprovalStage.OrderBy(x => x.Stage).ToList();
        }

        public async Task<List<MtdApprovalStage>> GetStagesDownAsync()
        {
            MtdApproval mtdApproval = await GetApproval();
            MtdApprovalStage currentStage = await GetCurrentStageAsync();
            return mtdApproval.MtdApprovalStage.Where(x=>x.Stage<currentStage.Stage).OrderBy(x => x.Stage).ToList();
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

        public async Task<MtdForm> GetFormAsync()
        {
            MtdStore mtdStore = await GetStoreAsync();
            return mtdStore.MtdFormNavigation;
        }

        public async Task<bool> ActionApprove(WebAppUser webAppUser)
        {
            MtdStore mtdStore = await GetStoreAsync();
            if (mtdStore.MtdStoreApproval != null && mtdStore.MtdStoreApproval.Approved == 1) { return false; }

            MtdApprovalStage currentStage = await GetCurrentStageAsync();
            MtdApprovalStage nextStage = await GetNextStage();
            sbyte complete = 0;
            if (nextStage == null) { complete = 1; nextStage = await GetLastStageAsync(); };

            MtdStoreApproval storeApproval = new MtdStoreApproval
            {
                Id = mtdStore.Id,
                MtdApproveStage = nextStage.Id,
                PartsApproved = nextStage.BlockParts,
                Approved = complete,                
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
                MtdApprovalStage = currentStage.Id,
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
            MtdStore mtdStore = await GetStoreAsync();
            if (mtdStore.MtdStoreApproval != null &&  mtdStore.MtdStoreApproval.Approved == 1) { return false; }

            MtdApproval mtdApproval = await GetApproval();
            MtdApprovalStage currentStage = await GetCurrentStageAsync();
            MtdApprovalStage nextStage;
            if (!complete)
            {
                nextStage = mtdApproval.MtdApprovalStage.Where(x => x.Id == idStage).FirstOrDefault();
            }
            else {
                nextStage = await GetCurrentStageAsync();
            }            

            if (nextStage == null) { return false; }


            MtdStoreApproval storeApproval = new MtdStoreApproval
            {
                Id = mtdStore.Id,
                MtdApproveStage = nextStage.Id,
                PartsApproved = nextStage.BlockParts,
                Approved = complete? (sbyte)1:(sbyte)0,                
            };

            if (mtdStore.MtdStoreApproval == null)
            {
                await _context.MtdStoreApproval.AddAsync(storeApproval);
            }
            else
            {
                try
                {
                    _context.MtdStoreApproval.Update(storeApproval);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            MtdLogApproval mtdLogApproval = new MtdLogApproval
            {
                MtdApprovalStage = currentStage.Id,
                Result = 0,
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

            return false;
        }

        public async Task<IList<MtdLogApproval>> GetHistory()
        {
            IList<MtdApprovalStage> stages = await GetStagesAsync();
            List<int> stageIds = stages.Select(x => x.Id).ToList();            
            return await _context.MtdLogApproval
                .Include(x => x.MtdApprovalStageNavigation)
                .Where(x => stageIds.Contains(x.MtdApprovalStage))
                .ToListAsync();
        }

        public async Task<List<string>> GetPartsIds() {

            MtdStore mtdStore = await GetStoreAsync();
            List<string> ids = new List<string>();
            ids = mtdStore.MtdStoreApproval.PartsApproved.Split("&").ToList();
            if (ids.Any())
            {
                ids.RemoveAt(ids.Count - 1);
            }

            return ids;
        }

    }
}

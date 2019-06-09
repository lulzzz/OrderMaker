using Mtd.OrderMaker.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Services
{
    public class ConfigHandler
    {
        public int CodeImgMenu => 1;
        public int CodeImgAppBar =>2;        

        private readonly OrderMakerContext _context;

        public ConfigHandler(OrderMakerContext context)
        {
            _context = context;
        }

        public async Task<string> GetImageFromConfig(int code)
        {
            string result = string.Empty;
            MtdConfigFile configFile = await _context.MtdConfigFiles.FindAsync(code);
            if (configFile != null && configFile.FileData != null)
            {
                string base64 = Convert.ToBase64String(configFile.FileData);
                result = string.Format("data:{0};base64,{1}", configFile.FileType, base64);
            }

            return result;
        }
    }
}

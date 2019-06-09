using System;
using System.Collections.Generic;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class MtdConfigFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public byte[] FileData { get; set; }
    }
}

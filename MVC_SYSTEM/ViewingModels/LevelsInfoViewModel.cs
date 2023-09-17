using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels
{
    public class LevelsInfoViewModel
    {
        public PagedList<ViewingModels.tbl_PktUtama> tbl_PktUtama { get; set; }
        public PagedList<ViewingModels.tbl_SubPkt> tbl_SubPkt { get; set; }
    }
}
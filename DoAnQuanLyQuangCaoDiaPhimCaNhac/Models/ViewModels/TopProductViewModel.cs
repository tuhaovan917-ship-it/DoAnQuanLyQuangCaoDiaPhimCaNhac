using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Models.ViewModels
{
    public class TopProductViewModel
    {
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
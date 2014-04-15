using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineAbit2013.Models
{
    public class AdminAbitsModel
    {
        public List<AbitCountModel> List { get; set; }
    }

    public class AbitCountModel
    {
        public KeyValuePair<int, string> Faculty { get; set; }
        public KeyValuePair<int, string> Profession { get; set; }
        public KeyValuePair<int, string> ObrazProgram { get; set; }
        public KeyValuePair<int, string> Profile { get; set; }
        public string CntBudzh { get; set; }
        public string CntPlatn { get; set; }
    }
}
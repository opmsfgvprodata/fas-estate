using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class Constans
    {
        public enum Month
        {
            None,
            [Description("Januari")]
            Januari,

            [Description("Februari")]
            Februari,

            [Description("Mac")]
            Mac,

            [Description("April")]
            April,

            [Description("Mei")]
            Mei,

            [Description("Jun")]
            Jun,

            [Description("Julai")]
            Julai,

            [Description("Ogos")]
            Ogos,

            [Description("Setember")]
            September,

            [Description("Oktober")]
            Oktober,

            [Description("November")]
            November,

            [Description("Disember")]
            Disember
        }

        public enum YaTidak
        {
            [Description("Tidak")]
            Tidak,

            [Description("Ya")]
            Ya
        }
    }
}
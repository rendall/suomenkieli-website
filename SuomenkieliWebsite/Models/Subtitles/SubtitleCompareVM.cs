using SuomenkieliWebsite.Models.Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuomenkieliWebsite.Models.Subtitles
{
    public class SubtitleCompareVM
    {
        public SubtitleList leftList { get; set; }
        public SubtitleList rightList { get; set; }
    }
}
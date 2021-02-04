using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuomenkieliWebsite.Models.Subtitles
{
    public class SubtitleEntry
    {
        public int Index { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Text { get; set; }
        public int OtherIndex { get; internal set; }
    }
}
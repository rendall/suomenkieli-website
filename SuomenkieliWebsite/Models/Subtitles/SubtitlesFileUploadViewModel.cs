using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuomenkieliWebsite.Models.Subtitles
{
    public class SubtitlesFileUploadViewModel
    {
        public HttpPostedFileBase FinnishSubtitleFile { get; set; }
        public HttpPostedFileBase EnglishSubtitleFile { get; set; }
    }
}
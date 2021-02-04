using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SuomenkieliWebsite.Models.Subtitles
{
    
    
    public class SubtitleList:List<SubtitleEntry>
    {

        public SubtitleList()
        {




        }

        public string languageCode { get; set; }
        public string movie { get; set; }

        public void CompareTo(SubtitleList that)
        {
            // This compares 'that' list's entries.
            // If two sets begin at the same time,
            // it adds the index of the other list
            // to the corresponding entry of this list.

            int aIndex = 0;
            int bIndex = 0;



            while (aIndex < this.Count && bIndex < that.Count)
            {
                SubtitleEntry aEntry = this[aIndex];
                SubtitleEntry bEntry = that[bIndex];

                if (sameStart(aEntry, bEntry))
                {
                    aEntry.OtherIndex = bEntry.Index;
                    aIndex++;
                    bIndex++;
                    continue;
                }

                if (aEntry.StartTime < bEntry.StartTime) aIndex++;
                else bIndex++;


            }




        }

        private bool sameStart(SubtitleEntry a, SubtitleEntry b)
        {
            bool bDuringA = b.StartTime >= a.StartTime && b.StartTime < a.EndTime;
            bool aDuringB = a.StartTime >= b.StartTime && a.StartTime < b.EndTime;

            return aDuringB || bDuringA;
        }
    }
}
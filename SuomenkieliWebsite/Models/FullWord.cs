using Suomenkieli;
using System;
using System.Collections.Generic;

namespace SuomenkieliWebsite.Models
{
    public class FullWord:Word
    {
        public List<Definition> Definitions { get; set; }

        public Relationship Relationship { get; set; }
        public List<RelatedWord> Related { get; internal set; }

        public int SearchDistance { get; set; }
    }
}
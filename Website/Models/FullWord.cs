using Suomenkieli;
using System.Collections.Generic;

namespace SuomenkieliWebsite.Models
{
    public class FullWord: Word
    {
        public List<Concept> Concepts { get; set; }
        public List<Definition> Definitions { get; set; }
        public Relationship Relationship { get; set; }
        public List<Word> RelatedWords { get; set; }
        public Word BaseWord { get; set; }

    }
}
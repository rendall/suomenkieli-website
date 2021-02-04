using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SuomenkieliWebsite.Models
{
    public class ExploreVM
    {
        public string search { get; set; }

        public int languageID { get; set; }

        // booleans that start with 'is' are not included in the search.

        public bool isDefinitionSearch { get; set; }
        // search concepts

        public bool verb { get; set; }
        public bool noun { get; set; }
        public bool adjective { get; set; }
        public bool adverb { get; set; }
        public bool pronoun { get; set; }
        public bool number { get; set; }
        public bool singular { get; set; }
        public bool plural { get; set; }
        public bool positive { get; set; }
        public bool negative { get; set; }
        public bool active { get; set; }
        public bool passive { get; set; }
        public bool nominative { get; set; }
        public bool accusative { get; set; }
        public bool genitive { get; set; }
        public bool partitive { get; set; }
        public bool inessive { get; set; }
        public bool elative { get; set; }
        public bool illative { get; set; }
        public bool adessive { get; set; }
        public bool ablative { get; set; }
        public bool allative { get; set; }
        public bool essive { get; set; }
        public bool translative { get; set; }
        public bool instructive { get; set; }
        public bool abessive { get; set; }
        public bool comitative { get; set; }
        public bool comparative { get; set; }
        public bool superlative { get; set; }
        public bool present { get; set; }
        public bool perfect { get; set; }
        public bool past { get; set; }
        public bool pluperfect { get; set; }
        public bool indicative { get; set; }
        public bool imperative { get; set; }
        public bool conditional { get; set; }
        public bool potential { get; set; }
        [Display(Name = "1st-person")]
        public bool person_1st { get; set; }
        [Display(Name = "2nd-person")]
        public bool person_2nd { get; set; }
        [Display(Name = "3rd-person")]
        public bool person_3rd { get; set; }
        [Display(Name = "1st-infinitive")]
        public bool infinitive__1st { get; set; }
        [Display(Name = "1st-infinitive-long")]
        public bool infinitive_1st_long { get; set; }
        [Display(Name = "2nd-infinitive")]
        public bool infinitive_2nd { get; set; }
        [Display(Name = "3rd-infinitive")]
        public bool infinitive_3rd { get; set; }
        [Display(Name = "4th-infinitive")]
        public bool infinitive_4th { get; set; }
        [Display(Name = "5th-infinitive")]
        public bool infinitive_5th { get; set; }
        public bool participle { get; set; }
        public bool agent { get; set; }
        public bool synonym { get; set; }
        public bool antonym { get; set; }
        public bool anagram { get; set; }
        public bool compound { get; set; }
        public bool related { get; set; }
        public bool hypernym { get; set; }
        public bool hyponym { get; set; }
        public bool alternative { get; set; }
        public bool coordinate { get; set; }
        public bool see { get; set; }
        public bool phrase { get; set; }
        public bool conjunction { get; set; }
        public bool interjection { get; set; }
        public bool particle { get; set; }
        public bool postposition { get; set; }
        public bool prefix { get; set; }
        public bool preposition { get; set; }
        public bool abbreviation { get; set; }
        public bool cardinal { get; set; }
        public bool contraction { get; set; }
        public bool numeral { get; set; }
        public bool ordinal { get; set; }
        public bool suffix { get; set; }
        public string originalSearch { get; internal set; }
        public List<FullWord> resultList { get; internal set; }
        public bool isAdmin { get; internal set; }

        public int pageNum { get; set; }
        public int rowsPerPage { get; set; } = 100;
    }
}
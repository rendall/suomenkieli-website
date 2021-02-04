using System.ComponentModel.DataAnnotations;
using Suomenkieli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace SuomenkieliWebsite.Models
{
    public class WordwDefinition:Word
    {
        [Column] public string Definition { get; set; }
        [Column] public string Relationship { get; set; }
        [Column] public string BaseWord { get; set; }
    }
}
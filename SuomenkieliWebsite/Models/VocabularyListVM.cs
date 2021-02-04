using Suomenkieli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuomenkieliWebsite.Models
{
    public class VocabularyListVM:VocabularyList 
    {
        public List<FullWord> WordList { get; set; }

        public VocabularyListVM()
        {

        }

        public VocabularyListVM(VocabularyList list)
        {
            this.Description = list.Description;
            this.ID = list.ID;
            this.OwnerID = list.OwnerID;
            this.Status = list.Status;
            this.Title = list.Title;
            this.RandomURL = list.RandomURL;
            this.UserURL = list.UserURL;
            

        }

        //public static implicit operator VocabularyListVM(VocabularyList list)
        //{
        //    VocabularyListVM vm = new VocabularyListVM(list);
        //    return vm;
        //}
    }
}
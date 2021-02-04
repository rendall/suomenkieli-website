using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiktionaryUtil;

namespace Sk2Services
{
    
    public class TermToDatabase
    {
        private static SK2 _db;
        static SK2 db {
            get
            {
                if (_db == null) _db = SK2.GetInstance();
                return _db;
            }
        }

        public static bool InsertTermToDatabase(string term)
        {
            var termObj = Wiktionary.GetTerm(term);

            if (termObj.entries.Count() > 0)
            {
                bool termExists = (db.Exists<Term>("WHERE text = @0", termObj.term));

                var dbTerm = new Term();
                dbTerm.text = termObj.term;

                var termId = termExists? db.Single<Term>("WHERE text = @0", termObj.term).id : (int)db.Insert(dbTerm);
                DeleteOldDefinitions(termId);

                var defs = termObj.entries.SelectMany(e => e.definitions).Select( obj => DefinitionObjectToModel(obj, termId));
                InsertNewDefinitions(defs);

                var examples = termObj.entries.SelectMany(e => e.definitions).SelectMany(d => d.examples).SelectMany(e => e).Select( e => ExampleObjectToModel(e));
                var newExamples = examples.Where(e => !db.Exists<Example>("WHERE text = @0", e.text));

                InsertNewExamples(newExamples);                

                return true;
            }
            return false;
        }

        private static void InsertNewExamples(IEnumerable<Example> examples)
        {
            foreach (Example example in examples) example.Save();
        }

        private static Example ExampleObjectToModel(ExampleObject e)
        {
            return new Example()
            {
                text = e.text,
                language = e.language
            };
        }

        private static void DeleteOldDefinitions(int termId)
        {
            db.Delete<Definition>("WHERE termId = @0", termId);
        }

        private static void InsertNewDefinitions(IEnumerable<Definition> definitions)
        {
            foreach (Definition def in definitions) def.Save();
        }

        private static Definition DefinitionObjectToModel( DefinitionObject def, int termId)
        {
            return new Definition()
            {
                text = def.text,
                rank = (byte)def.rank,
                language = def.language,
                termId = termId
            };
        }


    }
}

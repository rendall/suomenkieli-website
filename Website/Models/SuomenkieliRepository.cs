using PetaPoco;
using Suomenkieli;
using System;
using System.Collections.Generic;

namespace SuomenkieliWebsite.Models
{
    public class SuomenkieliRepository
    {
        private static SuomenkieliDB _db;

        public Word GetWord(String word)
        {
            Sql wordQuery = new Sql("SELECT TOP 1 * FROM Words WHERE Word=@0", word);
            Word wordRet = db.FirstOrDefault<Word>(wordQuery);

            return wordRet;
        }

        public FullWord GetFullWord(String word)
        {
            FullWord fullWord = (FullWord)GetWord(word);

            fullWord.Definitions = GetDefinitions(fullWord.ID);
            fullWord.Relationship = GetRelationship(fullWord.ID);

            return fullWord;
        }

        public List<Definition> GetDefinitions(int ID)
        {
            Sql defQuery = new Sql(@"SELECT Definitions.Definition, Words.ID
                FROM Definitions INNER JOIN
                 Word_Definition ON Definitions.ID = Word_Definition.DefinitionID INNER JOIN
                 Words ON Word_Definition.WordID = Words.ID
                WHERE (Words.ID = @0)
                ORDER BY Word_Definition.Rank", ID);

            List<Definition> definitions = db.Fetch<Definition>(defQuery);

            return definitions;
        }

        public Relationship GetRelationship(int ID)
        {
            Sql relQuery = new Sql(@"SELECT TOP (1) Relationships.Relationship
                    FROM WordRelationships INNER JOIN
                     Words ON WordRelationships.WordA = Words.BaseWordID AND WordRelationships.WordB = Words.ID RIGHT OUTER JOIN
                     Relationships ON WordRelationships.RelationshipID = Relationships.ID
                    WHERE (Words.ID = @0)
                    ORDER BY Words.BaseWordID, Words.ID", ID);

            Relationship rel = db.FirstOrDefault<Relationship>(relQuery);

            return rel;
        }

        public static SuomenkieliDB db
        {
            get
            {
                if (_db == null) _db = new SuomenkieliDB();
                return _db;
            }
        }
    }
}
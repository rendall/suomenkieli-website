using PetaPoco;
using Suomenkieli;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Identity;

namespace SuomenkieliWebsite.Models
{
    public class SuomenkieliRepository
    {
        private static SuomenkieliDB _db;

        public static Word GetWord(int id)
        {
            Sql wordQuery = new Sql("SELECT * FROM Words WHERE ID=@0", id);
            Word wordRet = db.FirstOrDefault<Word>(wordQuery);

            return wordRet;
        }

        public static List<FullWord> GetFullWordList(String wordStr)
        {
            List<FullWord> fwList = new List<FullWord>();

            List<int> ids = db.Fetch<int>(@"SELECT ID FROM Words WHERE Word=@0", wordStr);

            for (int i = 0; i < ids.Count; i++)
            {
                int wordID = ids[i];
                FullWord fw = GetFullWord(wordID);
                fwList.Add(fw);
            }



            return fwList;
        }

        public static List<FullWord> GetWordsByExplore(ExploreVM explore)
        {

            IEnumerable<int> resultIDs = GetWordsToReturn(explore);

            List<WordwDefinition> resultWords = GetWordInformation(explore, resultIDs);

            Dictionary<string, FullWord> resultDict = new Dictionary<string, FullWord>();

            foreach (WordwDefinition entry in resultWords)
            {
                FullWord fw;

                // "Search Distance" is a measure of the difference between the search term and the returned term.
                // However, if the search is of definitions, the "Search Distance" is instead the
                // index of the first appearance of the search term in a definition.


                if (resultDict.ContainsKey(entry._Word)) fw = resultDict[entry._Word];
                else
                {
                    fw = new FullWord();
                    fw._Word = entry._Word;
                    fw.ID = entry.ID;
                    fw.Definitions = new List<Definition>();
                    fw.Popularity = entry.Popularity;
                    fw.Related = new List<RelatedWord>();
                    if (explore.isDefinitionSearch) fw.SearchDistance = int.MaxValue; // this is measured later.
                    else fw.SearchDistance = LevenshteinDistance.Compute(fw._Word, explore.search);
                    resultDict[entry._Word] = fw;
                }

                var isDef = entry.Definition != null;

                if (isDef)
                {
                    Definition def = new Definition() { _Definition = entry.Definition, LanguageID = entry.LanguageID };
                    fw.Definitions.Add(def);

                    if (explore.isDefinitionSearch)
                    {
                        // compute search distance
                        int defSearchDistance = -1;
                        if (!String.IsNullOrEmpty(explore.search)) def._Definition.IndexOf(explore.search);
                        if (defSearchDistance == -1) defSearchDistance = int.MaxValue;
                        fw.SearchDistance = Math.Min(fw.SearchDistance, defSearchDistance);
                    }
                }
                else
                {
                    RelatedWord rel = new RelatedWord() { ID = entry.BaseWordID ?? default(int), Relationship = entry.Relationship, Word = entry.BaseWord };
                    fw.Related.Add(rel);
                }


            }

            if (resultDict.Count > 0) return resultDict.Values.OrderBy(e => e.SearchDistance).ToList<FullWord>();
            else return new List<FullWord>(); // empty list.

        }
        /// <summary>
        /// Returns the ids of the words wanted.
        /// </summary>
        /// <param name="explore"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetWordsToReturn(ExploreVM explore)
        { // This just returns a set of ids.


            IEnumerable<int> resIDs;

            string searchTerm = explore.search;
            bool hasSearchTerm = !string.IsNullOrEmpty(searchTerm);
            if (explore.isAdmin) explore.rowsPerPage = 500;

            List<string> concepts = GetConcepts(explore);

            do
            {
                Sql sql = new Sql();

                sql.Append(@"SELECT Words.ID FROM Words");

                if (hasSearchTerm)
                {
                    string sqlSearchTerm = searchTerm.Replace('*', '%');
                    sqlSearchTerm = sqlSearchTerm.Trim().ToLower();


                    if (explore.isDefinitionSearch)
                    {
                        if (!sqlSearchTerm.Contains('%')) sqlSearchTerm = string.Format("%{0}%", sqlSearchTerm);
                        sql.Append(@"INNER JOIN Word_Definition ON Words.ID = Word_Definition.WordID
                                    INNER JOIN Definitions ON Word_Definition.DefinitionID = Definitions.ID 
                                    WHERE Definition LIKE (@0)", sqlSearchTerm);
                    }
                    else
                    {
                        sql.Append(@"WHERE Words.Word LIKE (@0)", sqlSearchTerm);
                    }
                }

                if (concepts.Count > 0)
                {
                    if (hasSearchTerm) sql.Append("AND");
                    else sql.Append("WHERE");
                    sql.Append(@"Words.ID IN (
                                SELECT Words.ID FROM Words
                                INNER JOIN Word_Concept ON Words.ID = Word_Concept.WordID
                                INNER JOIN Concepts ON Word_Concept.ConceptID = Concepts.ID
                                WHERE Concept IN (@0) AND Popularity IS NOT NULL
                                GROUP BY Words.ID
                                HAVING COUNT(*) = @1)", concepts.ToArray(), concepts.Count);
                }

                sql.Append("ORDER BY Popularity DESC");

                int offset = explore.pageNum * explore.rowsPerPage;
                int fetchNext = explore.rowsPerPage;

                sql.Append("OFFSET @0 ROWS", offset);
                sql.Append("FETCH NEXT @0 ROWS ONLY", fetchNext);


                resIDs = db.Fetch<int>(sql);

                if (resIDs.Any() || string.IsNullOrEmpty(searchTerm)) continue;

                bool hasEndWildcard = (searchTerm[searchTerm.Length - 1] == '*');

                if (hasEndWildcard)
                {
                    searchTerm = searchTerm.Substring(0, searchTerm.Length - 2);
                }


                searchTerm += '*';


            } while (!resIDs.Any() && searchTerm.Length > 2);

            return resIDs;
        }

        private static List<string> GetConcepts(ExploreVM explore)
        {
            PropertyInfo[] properties = typeof(ExploreVM).GetProperties();
            List<string> concepts = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                bool isPropBool = property.PropertyType == Type.GetType("System.Boolean");

                if (!isPropBool) continue;
                if (property.Name.StartsWith("is")) continue;

                bool isChecked = (bool)property.GetValue(explore);
                if (isChecked) concepts.Add(property.Name);
            }

            for (int i = 0; i < concepts.Count; i++)
            {
                string concept = concepts[i];
                if (!concept.Contains("_")) continue;

                string[] elems = concept.Split('_');
                concept = elems[1] + "-" + elems[0];
                if (elems.Length > 2) concept += "-long";
                concepts[i] = concept;

            }

            return concepts;
        }
        /// <summary>
        /// Returns the information on each word with ids in resultIDs.
        /// </summary>
        /// <param name="explore"></param>
        /// <param name="resultIDs"></param>
        /// <returns></returns>
        private static List<WordwDefinition> GetWordInformation(ExploreVM explore, IEnumerable<int> resultIDs)
        {
            Sql definitionsSql = new Sql(@"SELECT Words.ID, Words.Word, Words.Popularity, DefinitionID, Definition, Definitions.LanguageID AS LanguageID, NULL As Relationship, NULL AS BaseWord, Rank FROM Definitions
                    INNER JOIN Word_Definition ON Definitions.ID = Word_Definition.DefinitionID
                    INNER JOIN Words ON Word_Definition.WordID = Words.ID
                    WHERE Words.ID IN (@0)", resultIDs);

            Sql allRelatedSql = new Sql(@"SELECT Words.ID, Words.Word, Words.Popularity, NULL AS DefinitionID, NULL AS Definition, NULL AS LanguageID, Relationship, BaseWords.Word AS BaseWord, RelationshipID + 20 AS Rank 
                    FROM WordRelationships
                    INNER JOIN Words ON WordRelationships.WordB = Words.ID
                    INNER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
                    INNER JOIN Words AS BaseWords ON WordRelationships.WordA = BaseWords.ID
                    WHERE Words.ID IN (@0)", resultIDs);

            Sql compactInfoSql = new Sql(@"SELECT Words.ID, Words.Word, Words.Popularity, NULL AS DefinitionID, NULL AS Definition, NULL AS LanguageID, Relationship, BaseWords.Word AS BaseWord, RelationshipID + 20 AS Rank 
                    FROM WordRelationships
                    INNER JOIN Words ON WordRelationships.WordA = Words.ID
                    INNER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
                    INNER JOIN Words AS BaseWords ON WordRelationships.WordB = BaseWords.ID
                    WHERE Words.ID IN (@0)", resultIDs);

            //Sql compactInfoSql = new Sql(@"SELECT Words.ID, Words.Word, Words.Popularity, NULL AS DefinitionID, NULL AS Definition, NULL AS LanguageID, Relationship, BaseWords.Word AS BaseWord, RelationshipID + 20 AS Rank 
            //        FROM WordRelationships
            //        INNER JOIN Words ON WordRelationships.WordB = Words.ID
            //        INNER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
            //        INNER JOIN Words AS BaseWords ON WordRelationships.WordA = BaseWords.ID
            //        WHERE Words.ID IN (@0)", resultIDs);

            Sql infoSql;
            // if there is only one result, return all of the information available: itself and related entries.
            if (resultIDs.ToArray().Length == 1)
            {
                infoSql = definitionsSql.Append("UNION").Append(allRelatedSql);
            }
            // otherwise just return where it, itself, has an entry.
            else infoSql = definitionsSql.Append("UNION").Append(compactInfoSql);

            Sql orderBy = new Sql("ORDER BY LanguageID, Rank, DefinitionID");

            infoSql.Append(orderBy);

            List<WordwDefinition> results = db.Fetch<WordwDefinition>(infoSql);

            if (results.Count == 0)
            {
                results = db.Fetch<WordwDefinition>(compactInfoSql.Append(orderBy));
            }

            return results;


        }

        public static List<FullWord> GetWordsByExploreOLD(ExploreVM explore)
        {


            PropertyInfo[] properties = typeof(ExploreVM).GetProperties();
            List<string> concepts = new List<string>();

            foreach (PropertyInfo property in properties)
            {
                bool isPropBool = property.PropertyType == Type.GetType("System.Boolean");

                if (!isPropBool) continue;
                if (property.Name.StartsWith("is")) continue;

                bool isChecked = (bool)property.GetValue(explore);
                if (isChecked) concepts.Add(property.Name);
            }

            for (int i = 0; i < concepts.Count; i++)
            {
                string concept = concepts[i];
                if (!concept.Contains("_")) continue;

                string[] elems = concept.Split('_');
                concept = elems[1] + "-" + elems[0];
                if (elems.Length > 2) concept += "-long";
                concepts[i] = concept;

            }


            bool hasSearchTerm = !string.IsNullOrWhiteSpace(explore.search);
            Sql sql = new Sql(@"SELECT Words.ID, Words.Word, Words.Popularity, Definition");


            if (explore.isDefinitionSearch) sql.Append(@"FROM Definitions
                            INNER JOIN Word_Definition ON Definitions.ID = Word_Definition.DefinitionID
                            INNER JOIN Words ON Word_Definition.WordID = Words.ID");
            else
            {
                sql.Append(@", Relationship, BaseWords.Word as BaseWord FROM Words
                            LEFT OUTER JOIN Word_Definition ON Words.ID = Word_Definition.WordID
                            LEFT OUTER JOIN Definitions ON Word_Definition.DefinitionID = Definitions.ID
                            LEFT OUTER JOIN WordRelationships ON Words.ID = WordRelationships.WordA
                            LEFT OUTER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
                            LEFT OUTER JOIN Words AS BaseWords ON WordRelationships.WordB = BaseWords.ID");
            }


            if (hasSearchTerm)
            {

                if (explore.originalSearch == null)
                {
                    explore.originalSearch = explore.search.Replace("*", "");
                }
                string searchTerm = explore.search.Replace('*', '%');


                if (explore.isDefinitionSearch)
                {
                    if (!searchTerm.Contains('%')) searchTerm = string.Format("%{0}%", searchTerm);
                    sql.Append(@"WHERE Definition LIKE (@0)", searchTerm);
                }
                else
                {



                    searchTerm = searchTerm.Replace("a:", "ä").Replace("o:", "ö"); // perhaps this should be on the client side
                    searchTerm = searchTerm.Trim().ToLower();

                    sql.Append(@"WHERE Words.Word LIKE (@0)", searchTerm);
                }

            }

            // sql.Append(@"");

            if (concepts.Count > 0)
            {
                if (hasSearchTerm) sql.Append("AND");
                else sql.Append("WHERE");
                sql.Append(@"Words.ID IN (
                                SELECT Words.ID FROM Words
                                INNER JOIN Word_Concept ON Words.ID = Word_Concept.WordID
                                INNER JOIN Concepts ON Word_Concept.ConceptID = Concepts.ID
                                WHERE Concept IN (@0) AND Popularity IS NOT NULL
                                GROUP BY Words.ID
                                HAVING COUNT(*) = @1)", concepts.ToArray(), concepts.Count);
            }

            if (explore.isDefinitionSearch) sql.Append("ORDER BY Words.Popularity DESC, Words.Word");
            else sql.Append("ORDER BY Words.Popularity DESC, Words.ID, RelationshipID");

            Debug.WriteLine(sql);
            List<WordwDefinition> wordsDefs = db.Fetch<WordwDefinition>(sql);
            List<FullWord> resultSet = new List<FullWord>();
            List<string> relList = new List<string>();

            // TODO: There is a way to do this with LINQ but, for now....

            int wordID = 0;
            FullWord word = null;
            WordwDefinition wDef = null;

            for (int i = 0; i < wordsDefs.Count; i++)
            {
                wDef = wordsDefs[i];
                bool isNew = wordID != wDef.ID;
                if (isNew)
                {

                    if (word != null && relList.Count > 0)
                    { // finish up the previous word
                      // adding definitions derived from relationships at the end of the list of definitions.
                        if (word.Definitions == null) word.Definitions = new List<Definition>();
                        for (int j = 0; j < relList.Count; j++)
                        {
                            Definition def = new Definition() { _Definition = relList[j] };
                            word.Definitions.Add(def);
                        }


                    }


                    word = new FullWord();
                    relList = new List<string>();

                    word.ID = wDef.ID;
                    word._Word = wDef._Word;
                    word.Popularity = wDef.Popularity;
                    resultSet.Add(word);
                    wordID = wDef.ID;

                    word.SearchDistance = LevenshteinDistance.Compute(word._Word, explore.originalSearch);

                }

                if (!string.IsNullOrEmpty(wDef.Definition))
                {
                    if (word.Definitions == null) word.Definitions = new List<Definition>();
                    bool hasDef = word.Definitions.Any<Definition>(t => t._Definition == wDef.Definition);
                    if (!hasDef)
                    {
                        Definition def = new Definition() { _Definition = wDef.Definition };
                        if (!word.Definitions.Contains(def)) word.Definitions.Add(def);
                    }
                }

                if (!string.IsNullOrEmpty(wDef.Relationship))
                {
                    string relDefStr = String.Format("{0}: <a href={1}>{1}</a>", wDef.Relationship, wDef.BaseWord);
                    if (!relList.Contains(relDefStr)) relList.Add(relDefStr);
                }

            }





            if (resultSet.Count == 1)
            {
                Sql relatedSql = new Sql(@"SELECT RelatedWords.Word, Relationship FROM WordRelationships
                    INNER JOIN Words AS RelatedWords ON WordRelationships.WordA = RelatedWords.ID
                    INNER JOIN Words ON WordRelationships.WordB = Words.ID
                    INNER JOIN Relationships ON WordRelationships.RelationshipID = Relationships.ID
                    WHERE Words.Word = @0
                    ORDER BY RelationshipID", explore.search);

                List<WordwDefinition> relWordList = db.Fetch<WordwDefinition>(relatedSql);

                if (relWordList.Count > 0) relList = new List<string>();

                for (int k = 0; k < relWordList.Count; k++)
                {
                    WordwDefinition relW = relWordList[k];
                    if (!string.IsNullOrEmpty(relW.Relationship))
                    {
                        string relDefStr = String.Format("{0}: <a href={1}>{1}</a>", relW.Relationship, relW._Word);
                        if (!relList.Contains(relDefStr)) relList.Add(relDefStr);
                    }
                }
            }

            if (word != null && wDef != null && relList.Count > 0)
            { // finish up the previous word
              // adding definitions derived from relationships at the end of the list of definitions.
                if (word.Definitions == null) word.Definitions = new List<Definition>();
                for (int j = 0; j < relList.Count; j++)
                {
                    Definition def = new Definition() { _Definition = relList[j] };
                    word.Definitions.Add(def);
                }


            }


            if (resultSet.Count == 0 && explore.search.Length > 1)
            {
                string broaderTerm = explore.search;
                bool hasEndWildcard = (broaderTerm[broaderTerm.Length - 1] == '*');

                if (hasEndWildcard)
                {
                    broaderTerm = broaderTerm.Substring(0, broaderTerm.Length - 2);
                }


                broaderTerm += '*';
                explore.search = broaderTerm;

                resultSet = GetWordsByExplore(explore);

            }



            if (!explore.isDefinitionSearch && hasSearchTerm && resultSet.Count > 0 && resultSet.First().SearchDistance > 0) resultSet.Sort((a, b) => a.SearchDistance.CompareTo(b.SearchDistance));

            return resultSet;


        }

        internal static void OrderVocabularyList(string list, int[] order)
        {
            List<VocabularyList_Word> vlWords = VocabularyList_Word.Fetch("WHERE VocabularyListID = @0", list);

            Sql updateSQL = new Sql(@"UPDATE [VocabularyList_Word] 
                SET Rank = CASE WordID");

            for (int i = 0; i < order.Length; i++)
            {
                int wordID = order[i];
                VocabularyList_Word vlWord = vlWords.First(w => w.WordID == wordID);
                vlWord.Rank = i;
                updateSQL.Append("WHEN @0 THEN @1", wordID, i);
                //vlWord.Update(); // TODO: A more efficient version.
            }
            updateSQL.Append("END");
            updateSQL.Append("WHERE VocabularyListID=@0 AND WordID IN (@1)", list, order);

            db.Execute(updateSQL);

        }

        internal static List<FullWord> GetFullWordListByListID(int id)
        {
            List<VocabularyList_Word> vlWords = VocabularyList_Word.Fetch("WHERE VocabularyListID = @0 ORDER BY Rank", id);

            int[] wordIDs = vlWords.Select(t => t.WordID).ToArray();
            List<FullWord> vlFW = SuomenkieliRepository.GetFullWordList(wordIDs);

            return vlFW;
        }

        internal static void AddVocabularyWord(string wordStr, int id, string owner)
        {
            if (string.IsNullOrEmpty(owner)) throw new Exception("Owner needed");

            bool isList = VocabularyList.Exists("WHERE ID = @0 AND OwnerID = @1", id, owner);

            if (!isList) throw new Exception("Unknown list");

            Word word = Word.FirstOrDefault("WHERE Word=@0", wordStr);

            if (word == null) throw new Exception(String.Format("Unknown word {0}", wordStr));

            VocabularyList_Word vlw = new VocabularyList_Word();
            vlw.VocabularyListID = id;
            vlw.WordID = word.ID;
            vlw.Insert();
        }

        internal static int RemoveVocabularyWord(string wordStr, int id, string owner)
        {
            if (string.IsNullOrEmpty(owner)) throw new Exception("Owner needed");

            bool isList = VocabularyList.Exists("WHERE ID = @0 AND OwnerID = @1", id, owner);

            if (!isList) throw new Exception("Unknown list");

            Word word = Word.FirstOrDefault("WHERE Word=@0", wordStr);

            if (word == null) throw new Exception(String.Format("Unknown word {0}", wordStr));

            Sql delSql = new Sql(@"DELETE FROM [dbo].[VocabularyList_Word]
                    WHERE WordID=@0 AND VocabularyListID=@1", word.ID, id);

            return db.Execute(delSql);
        }

        public static List<FullWord> GetFullWordList(int[] ids)
        {
            List<FullWord> fwList = new List<FullWord>();

            for (int i = 0; i < ids.Length; i++)
            {
                int wordID = ids[i];
                FullWord fw = GetFullWord(wordID);
                fwList.Add(fw);
            }

            return fwList;
        }

        public static int? GetWordID(string word)
        {
            int? WordID = db.FirstOrDefault<int>("SELECT ID FROM Words WHERE Word=@0", word);
            return WordID;

        }

        public static FullWord GetFullWord(int id)
        {
            Word word = GetWord(id);

            if (word == null) return null;

            FullWord fullWord = new FullWord();

            fullWord.BaseWordID = word.BaseWordID;
            fullWord.ID = word.ID;
            fullWord._Word = word._Word;

            int defID = word.BaseWordID == null ? word.ID : (int)word.BaseWordID;

            fullWord.Definitions = GetDefinitions(defID);


            fullWord.Relationship = GetRelationship(fullWord.ID);
            fullWord.Related = GetRelatedWords(fullWord._Word);

            return fullWord;
        }

        private static List<RelatedWord> GetRelatedWords(string word)
        {
            Sql relQuery = new Sql(@"SELECT Relationships.Relationship, RelatedWords.Word, RelatedWords.ID
                    FROM Relationships INNER JOIN
                     WordRelationships ON Relationships.ID = WordRelationships.RelationshipID INNER JOIN
                     Words AS RelatedWords ON WordRelationships.WordB = RelatedWords.ID INNER JOIN
                     Words ON WordRelationships.WordA = Words.BaseWordID
                    WHERE (Words.Word = @0)
                    ORDER BY RelationshipID", word);

            List<RelatedWord> returnList = db.Fetch<RelatedWord>(relQuery);

            return returnList;
        }

        public static List<Definition> GetDefinitions(int ID)
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


        public static Relationship GetRelationship(int ID)
        {
            Sql relQuery = new Sql(@"SELECT Relationships.Relationship
                FROM WordRelationships
                JOIN Relationships
                ON Relationships.ID = RelationshipID
                JOIN Words
                ON Words.BaseWordID = WordRelationships.WordA AND Words.ID = WordRelationships.WordB
                WHERE Words.ID = @0", ID);

            Relationship rel = db.FirstOrDefault<Relationship>(relQuery);

            return rel;
        }

        public static SuomenkieliDB db
        {
            get
            {
                if (_db == null) _db = new SuomenkieliDB();
                if (_db.Connection != null)
                {
                    Debug.WriteLine("DB connection state:", _db.Connection.State.ToString());
                    if (_db.Connection.State != System.Data.ConnectionState.Closed) _db = new SuomenkieliDB();
                }
                return _db;
            }
        }
    }

    public class RelatedWord
    {
        public string Relationship { get; set; }
        public string Word { get; set; }
        public int ID { get; set; }
    }

    static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            if (string.IsNullOrWhiteSpace(s)) s = "";
            if (string.IsNullOrWhiteSpace(t)) t = "";
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }

}
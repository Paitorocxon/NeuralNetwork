using SPLAI_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLPModels
{

    public class ObjectGenerator
    {
        private List<string> adverbs;
        private List<string> articles;
        private List<string> demonstrativePronouns;
        private List<string> interrogativePronouns;
        private List<string> conjunctions;
        private List<string> modalVerbs;
        private List<string> prepositions;
        private List<string> nouns;
        private List<string> possessivePronouns;
        private List<string> pronouns;

        public ObjectGenerator(
            List<string> adverbs,
            List<string> articles,
            List<string> demonstrativePronouns,
            List<string> interrogativePronouns,
            List<string> conjunctions,
            List<string> modalVerbs,
            List<string> prepositions,
            List<string> nouns,
            List<string> possessivePronouns,
            List<string> pronouns)
        {
            this.adverbs = adverbs;
            this.articles = articles;
            this.demonstrativePronouns = demonstrativePronouns;
            this.interrogativePronouns = interrogativePronouns;
            this.conjunctions = conjunctions;
            this.modalVerbs = modalVerbs;
            this.prepositions = prepositions;
            this.nouns = nouns;
            this.possessivePronouns = possessivePronouns;
            this.pronouns = pronouns;
        }


        public void Run(string inputText)
        {
            ObjectGenerator generator = new ObjectGenerator(adverbs, articles, demonstrativePronouns, interrogativePronouns, conjunctions, modalVerbs, prepositions, nouns, possessivePronouns, pronouns);

            // Generieren Sie Objekte aus dem Eingabetext.
            List<ObjectEntity> generatedObjects = generator.GenerateObjectsFromText(inputText);

            // Schleife, um Informationen über die generierten Objekte anzuzeigen.
            foreach (ObjectEntity obj in generatedObjects)
            {
                Console.WriteLine($"Object Name: {obj.name}");
                Console.WriteLine("Properties:");
                foreach (string property in obj.properties)
                {
                    Console.WriteLine($"- {property}");
                }
                Console.WriteLine("Input Relations:");
                foreach (ObjectEntity inputEntity in obj.inputNode)
                {
                    Console.WriteLine($"- {inputEntity.name}");
                }
                Console.WriteLine("Output Relations:");
                foreach (ObjectEntity outputEntity in obj.outputNode)
                {
                    Console.WriteLine($"- {outputEntity.name}");
                }
                Console.WriteLine();
            }

        }


        public void AnalizeText(string inputText)
        {
            string pattern = @"\{([^}]+)\} ist (.+?)\.";

            // Erstellen Sie eine Regex-Instanz
            Regex regex = new Regex(pattern);

            // Suchen Sie nach Übereinstimmungen im Eingabetext
            MatchCollection matches = regex.Matches(inputText);

            // Durchlaufen Sie die Übereinstimmungen und extrahieren Sie die Informationen
            foreach (Match match in matches)
            {
                string wort1 = match.Groups[1].Value;
                string restDesSatzes = match.Groups[2].Value;

                Console.WriteLine($"{wort1} ist {restDesSatzes}");
            }
            Console.WriteLine("---");
            Console.ReadLine();
        }

        public List<ObjectEntity> GenerateObjectsFromText(string inputText)
        {


            AnalizeText(inputText);

            // Zerlegen Sie den Eingabetext in Sätze (Annahme: Sätze enden mit einem Punkt).
            string[] sentences = inputText.Split('.');

            // Initialisieren Sie eine Liste von ObjectEntity-Objekten für jeden erkannten Satz.
            List<ObjectEntity> generatedObjects = new List<ObjectEntity>();

            foreach (string sentence in sentences)
            {
                // Zerlegen Sie jeden Satz in Wörter (Tokens).
                string[] tokens = sentence.Trim().Split(' ');

                // Initialisieren Sie ein neues ObjectEntity-Objekt für diesen Satz.
                ObjectEntity generatedObject = new ObjectEntity("GeneratedObject");

                // Verfolgen Sie das vorherige Token, um Beziehungen zu erkennen.
                string previousToken = null;

                foreach (string token in tokens)
                {
                    if (char.IsUpper(token[1]))
                    {
                        // Wenn es sich um ein Nomen handelt, setzen Sie den Namen des Objekts.
                        generatedObject.name = token;

                        // Wenn es ein Nomen ist und ein vorheriges Token vorhanden ist, fügen Sie eine Beziehung hinzu.
                        if (previousToken != null)
                        {
                            generatedObject.AddInputNode(new ObjectEntity(previousToken));
                        }
                    }
                    else if (adverbs.Contains(token))
                    {
                        // Wenn es sich um ein Adverb handelt, fügen Sie es zu den Eigenschaften hinzu.
                        generatedObject.properties.Add($"is {token}");

                        // Wenn es ein Adverb ist und ein vorheriges Nomen vorhanden ist, fügen Sie eine Beziehung hinzu.
                        if (previousToken != null && nouns.Contains(previousToken))
                        {
                            generatedObject.AddOutputNode(new ObjectEntity(previousToken));
                        }
                    }
                    else if (articles.Contains(token))
                    {
                        // Wenn es sich um einen Artikel handelt, fügen Sie ihn zu den Eigenschaften hinzu.
                        generatedObject.properties.Add($"has the article '{token}'");
                    }

                    // Setzen Sie das vorherige Token für das nächste Durchlauf.
                    previousToken = token;
                }

                // Fügen Sie das generierte Objekt zur Liste hinzu.
                generatedObjects.Add(generatedObject);
            }

            return generatedObjects;
        }

    }
}

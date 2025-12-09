using Models;

namespace SPLAI_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            NLP nlp = new NLP();
            while (true)
            {
                Console.Write("Frag die KI: ");
                //LowestOccurence LO = new LowestOccurence(prompt());
                //KnowledgeGraph KG = new KnowledgeGraph(promptArray(), CleanInput());

                //KnowledgeGraphQA2 KGQA = new KnowledgeGraphQA2(promptArray());
                //KeywordBasedAnswerSelector KBAS = new KeywordBasedAnswerSelector();
                //Console.WriteLine(KBAS.SelectBestAnswer(Console.ReadLine(), new List<string>(promptArray())));

                //ObjectEntity OE = NLPModels.GermanNLPModel.AnalyzeAndCreateObjectEntity(Console.ReadLine());
                //Console.WriteLine(NLPModels.GermanNLPModel.GuessSubjectOrObject(Console.ReadLine()));
                //string path = @"C:\Users\Pait\Desktop\model\";
                //NLPModels.ObjectGenerator OG = new NLPModels.ObjectGenerator(
                //    new List<string>(File.ReadAllLines(path + "adv.txt")),                                        // adverbs 
                //    new List<string>(File.ReadAllLines(path + "art.txt")),                                        // articles 
                //    new List<string>(File.ReadAllLines(path + "dempron.txt")),                                    // demonstrativePronouns 
                //    new List<string>(File.ReadAllLines(path + "interropron.txt")),                                // interrogativePronouns 
                //    new List<string>(File.ReadAllLines(path + "konj.txt")),                                       // conjunctions 
                //    new List<string>(File.ReadAllLines(path + "modverb.txt")),                                    // modalVerbs 
                //    new List<string>(File.ReadAllLines(path + "prepo.txt")),                                      // prepositions 
                //    new List<string>(File.ReadAllLines(path + "nouns.txt")),                                      // nouns 
                //    new List<string>(File.ReadAllLines(path + "posspron.txt")),                                   // possessivePronouns 
                //    new List<string>(File.ReadAllLines(path + "pronoun.txt"))                                     // pronouns 
                //    );
                //OG.Run(Console.ReadLine());

                //NeuralNetwork.NeuralNetwork.Run();
                //NeuralNetwork.NeuralNetworkRNN.Run();
                //TokenizedNeuralNetwork.Run();


                // Hier drunter sind nur die Besten!!!!!
                //NLPSentencedObjection NLPSO = new NLPSentencedObjection();
                NeuralNetwork.NeuralNetwork_Primitive.Run();



                Console.WriteLine();
            }
        }

        public static string prompt()
        {
            string prompt = "";
            if (File.Exists("input.txt"))
            {
                foreach (string line in File.ReadAllLines("input.txt"))
                {
                    prompt += " " + line.ToLower().Replace(".", "").Replace(",", "").Replace("?", "").Replace("!", "");
                }
            }
            return prompt;
        }

        public static string[] promptArray()
        {
            string[] prompt = new string[File.ReadAllLines("input.txt").Length];
            int i = 0;
            if (File.Exists("input.txt"))
            {
                foreach (string line in File.ReadAllLines("input.txt"))
                {
                    prompt[i] = line.ToLower().Replace(".", "").Replace(",", "").Replace("?", "").Replace("!", "");
                    i++;
                }
            }
            return prompt;
        }
        public static string CleanInput()
        {
            string prompt = Console.ReadLine().ToLower();
            string path = @"C:\Users\Pait\Desktop\model\";
            List<string> words = new List<string>(File.ReadAllLines(path + "FILTERFILE.txt"));
            var sortedWords = words.OrderByDescending(word => word.Length);

            foreach (string filterword in sortedWords)
            {
                prompt = prompt.Replace(filterword, "");
            }

            prompt = prompt.Replace("?", "").Replace("!", "").Replace(".", "");
            Console.Title = prompt;
            return prompt;
        }
    }
}
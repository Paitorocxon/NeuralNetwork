using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SPLAI_2;

namespace Models
{
    class LowestOccurence
    {
        public LowestOccurence(string INPUTSTRING)
        {
            string inputText = INPUTSTRING;

            Dictionary<string, ObjectEntity> wordEntities = CreateWordEntities(inputText);

            // Sortieren des Dictionary nach der Anzahl der Output-Nodes (absteigend)
            var sortedWordEntities = wordEntities.OrderByDescending(pair => pair.Value.outputNode.Count);

            foreach (var pair in sortedWordEntities)
            {
                Console.WriteLine($"Wort: {pair.Key}, Anzahl der Output-Nodes: {pair.Value.outputNode.Count}");
            }

            Console.Write("Geben Sie einen Satz ein: ");
            string inputSentence = Console.ReadLine().ToLower();

            string[] inputWords = inputSentence.Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            List<ObjectEntity> selectedNodes = SelectBestNodes(inputWords, sortedWordEntities, 10);

            if (selectedNodes.Count > 0)
            {
                Console.WriteLine("Gewählte Nodes: ");
                foreach (var selectedNode in selectedNodes)
                {
                    Console.WriteLine(selectedNode.name);
                }

                // 10 Mal von diesen Nodes ausgehen
                Random randomOutput = new Random();
                ObjectEntity currentNode = selectedNodes.Last();

                for (int i = 0; i < 10; i++)
                {
                    List<ObjectEntity> outputNodes = currentNode.outputNode;

                    if (outputNodes.Count > 0)
                    {
                        int randomOutputIndex = randomOutput.Next(0, outputNodes.Count);
                        ObjectEntity nextNode = outputNodes[randomOutputIndex];

                        Console.WriteLine($"Von {currentNode.name} zu {nextNode.name}");
                        currentNode = nextNode;
                    }
                    else
                    {
                        Console.WriteLine($"Ende des Netzwerks erreicht.");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Keine passenden Nodes gefunden.");
            }
        }

        static List<ObjectEntity> SelectBestNodes(string[] inputWords, IOrderedEnumerable<KeyValuePair<string, ObjectEntity>> sortedWordEntities, int maxNodes)
        {
            List<ObjectEntity> selectedNodes = new List<ObjectEntity>();
            Dictionary<ObjectEntity, int> nodeScores = new Dictionary<ObjectEntity, int>();

            foreach (var pair in sortedWordEntities)
            {
                ObjectEntity node = pair.Value;
                int score = node.outputNode.Count;

                foreach (string inputWord in inputWords)
                {
                    if (node.AreYou(inputWord))
                    {
                        score += 1; // Erhöhe den Score für passende Nodes im Satz
                    }
                }

                nodeScores.Add(node, score);
            }

            var sortedNodeScores = nodeScores.OrderByDescending(pair => pair.Value);

            int nodesToAdd = Math.Min(maxNodes, sortedNodeScores.Count());

            for (int i = 0; i < nodesToAdd; i++)
            {
                ObjectEntity node = sortedNodeScores.ElementAt(i).Key;
                selectedNodes.Add(node);
            }

            return selectedNodes;
        }



        static Dictionary<string, ObjectEntity> CreateWordEntities(string text)
        {
            string[] words = text.Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, ObjectEntity> wordEntities = new Dictionary<string, ObjectEntity>();

#pragma warning disable CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.
            ObjectEntity previousEntity = null;
#pragma warning restore CS8600 // Das NULL-Literal oder ein möglicher NULL-Wert wird in einen Non-Nullable-Typ konvertiert.

            foreach (string word in words)
            {
                string cleanedWord = word.ToLower();

                if (!wordEntities.ContainsKey(cleanedWord))
                {
                    ObjectEntity entity = new ObjectEntity(cleanedWord);
                    wordEntities[cleanedWord] = entity;
                }

                ObjectEntity currentEntity = wordEntities[cleanedWord];

                // Vorheriges Wort
                if (previousEntity != null)
                {
                    currentEntity.AddInputNode(previousEntity);
                }

                // Nächstes Wort
                if (previousEntity != null)
                {
                    previousEntity.AddOutputNode(currentEntity);
                }

                previousEntity = currentEntity;
            }

            return wordEntities;
        }

        // ============================
    }


    public class KnowledgeGraph
    {
        public List<ObjectEntity> Nodes { get; } = new List<ObjectEntity>();


        public KnowledgeGraph(string[] INPUTTEXT, string question)
        {
            ObjectEntity sentenceBefore = FindOrCreateNode("");
            foreach (string str in INPUTTEXT)
            {
                ObjectEntity sentence = FindOrCreateNode(str);
                ConnectNodes(sentence, sentenceBefore);
                sentenceBefore = sentence;
            }

            string answer = AnswerQuestion(question);
            Console.WriteLine("Antwort: " + answer);

        }


        public ObjectEntity FindOrCreateNode(string nodeName)
        {
            ObjectEntity node = Nodes.FirstOrDefault(n => n.AreYou(nodeName));

            if (node == null)
            {
                node = new ObjectEntity(nodeName);
                Nodes.Add(node);
            }

            return node;
        }

        public void ConnectNodes(ObjectEntity parent, ObjectEntity child)
        {
            parent.AddOutputNode(child);
            child.AddInputNode(parent);
        }

        public string AnswerQuestion(string question)
        {
            string[] questionKeywords = question.ToLower().Split(' ');

            // Finden der besten Antwort basierend auf der Anzahl der übereinstimmenden Keywords
            string bestAnswer = null;
            int bestMatchCount = 0;

            foreach (ObjectEntity node in Nodes)
            {
                int matchCount = node.name.ToLower().Split(' ').Intersect(questionKeywords).Count();

                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestAnswer = node.name;
                }
            }

            return bestAnswer;
        }
    }

    public class KnowledgeGraphQA_Primitive
    {
        public List<ObjectEntity> Nodes { get; } = new List<ObjectEntity>();

        public KnowledgeGraphQA_Primitive(string[] inputText)
        {
            ObjectEntity sentenceBefore = FindOrCreateNode("");
            foreach (string str in inputText)
            {
                ObjectEntity sentence = FindOrCreateNode(str);
                ConnectNodes(sentence, sentenceBefore);
                sentenceBefore = sentence;
            }

            while (true)
            {
                Console.Write("Stellen Sie eine Frage (oder 'exit' zum Beenden): ");
                string question = Console.ReadLine();

                if (question.ToLower() == "exit")
                    break;

                string answer = AnswerQuestion(question);
                Console.WriteLine("Antwort: " + answer);
            }
        }

        public ObjectEntity FindOrCreateNode(string nodeName)
        {
            ObjectEntity node = Nodes.FirstOrDefault(n => n.AreYou(nodeName));

            if (node == null)
            {
                node = new ObjectEntity(nodeName);
                Nodes.Add(node);
            }

            return node;
        }

        public void ConnectNodes(ObjectEntity parent, ObjectEntity child)
        {
            parent.AddOutputNode(child);
            child.AddInputNode(parent);
        }

        public string AnswerQuestion(string question)
        {
            string[] questionKeywords = question.ToLower().Split(' ');

            // Finden der besten Antwort basierend auf der Anzahl der übereinstimmenden Keywords
            string bestAnswer = null;
            int bestMatchCount = 0;

            foreach (ObjectEntity node in Nodes)
            {
                int matchCount = node.name.ToLower().Split(' ').Intersect(questionKeywords).Count();

                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestAnswer = node.name;
                }
            }

            return bestAnswer;
        }
    }

    public class KnowledgeGraphQA
    {
        public List<ObjectEntity> Nodes { get; } = new List<ObjectEntity>();

        public KnowledgeGraphQA(string[] inputText)
        {
            ObjectEntity sentenceBefore = FindOrCreateNode("");
            foreach (string str in inputText)
            {
                ObjectEntity sentence = FindOrCreateNode(str);
                ConnectNodes(sentence, sentenceBefore);
                sentenceBefore = sentence;
            }

            while (true)
            {
                Console.Write("Stellen Sie eine Frage (oder 'exit' zum Beenden): ");
                string question = Console.ReadLine();

                if (question.ToLower() == "exit")
                    break;

                string answer = AnswerQuestion(question);
                Console.WriteLine("Antwort: " + answer);
            }
        }

        public ObjectEntity FindOrCreateNode(string nodeName)
        {
            ObjectEntity node = Nodes.FirstOrDefault(n => n.AreYou(nodeName));

            if (node == null)
            {
                node = new ObjectEntity(nodeName);
                Nodes.Add(node);
            }

            return node;
        }

        public void ConnectNodes(ObjectEntity parent, ObjectEntity child)
        {
            parent.AddOutputNode(child);
            child.AddInputNode(parent);
        }

        public string AnswerQuestion(string question)
        {
            string[] questionKeywords = question.ToLower().Split(' ');

            // Finden der am wenigsten häufigen Antwort basierend auf der Anzahl der übereinstimmenden Keywords
            string bestAnswer = null;
            int bestMatchCount = 100;

            foreach (ObjectEntity node in Nodes)
            {
                int matchCount = node.name.ToLower().Split(' ').Intersect(questionKeywords).Count();

                if (matchCount < bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestAnswer = node.name;
                }
            }

            return bestAnswer;
        }
    }

    public class KnowledgeGraphQA2
    {
        public List<ObjectEntity> Nodes { get; } = new List<ObjectEntity>();

        public KnowledgeGraphQA2(string[] inputText)
        {
            ObjectEntity sentenceBefore = FindOrCreateNode("");
            foreach (string str in inputText)
            {
                ObjectEntity sentence = FindOrCreateNode(str);
                ConnectNodes(sentence, sentenceBefore);
                sentenceBefore = sentence;
            }

            while (true)
            {
                Console.Write("Stellen Sie eine Frage (oder 'exit' zum Beenden): ");
                string question = Console.ReadLine();

                if (question.ToLower() == "exit")
                    break;

                string answer = AnswerQuestion(question);
                Console.WriteLine("Antwort: " + answer);
            }
        }

        public ObjectEntity FindOrCreateNode(string nodeName)
        {
            ObjectEntity node = Nodes.FirstOrDefault(n => n.AreYou(nodeName));

            if (node == null)
            {
                node = new ObjectEntity(nodeName);
                Nodes.Add(node);
            }

            return node;
        }

        public void ConnectNodes(ObjectEntity parent, ObjectEntity child)
        {
            parent.AddOutputNode(child);
            child.AddInputNode(parent);
        }

        public string AnswerQuestion(string question)
        {
            string[] questionKeywords = question.ToLower().Split(' ');

            // Durchsuchen aller Nodes und Ermitteln der besten Antwort und des sinnvollsten Satzes
            string bestAnswer = null;
            string bestSentence = null;
            int bestMatchCount = 0;

            foreach (ObjectEntity node in Nodes)
            {
                string nodeSentence = node.name;
                int matchCount = nodeSentence.ToLower().Split(' ').Intersect(questionKeywords).Count();

                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestAnswer = node.name;
                    bestSentence = nodeSentence;
                }
            }

            return $"Antwort: {bestAnswer}\nSinnvollster Satz: {bestSentence}";
        }
    }

    public class KeywordBasedAnswerSelector
    {
        public string SelectBestAnswer(string question, List<string> answers)
        {
            // Schritt 1: Extrahieren der wichtigsten Keywords aus der Frage
            List<string> importantKeywords = ExtractImportantKeywords(question, answers);

            // Schritt 2: Auswählen der besten Antwort basierend auf den wichtigen Keywords
            string bestAnswer = SelectBestAnswerByKeywords(importantKeywords, answers);

            return bestAnswer;
        }

        private List<string> ExtractImportantKeywords(string question, List<string> answers)
        {
            // Zählen Sie die Anzahl der Vorkommen jedes Wortes in den Antworten
            Dictionary<string, int> wordCounts = new Dictionary<string, int>();

            foreach (string answer in answers)
            {
                string[] words = answer.ToLower().Split(' ');

                foreach (string word in words)
                {
                    if (!wordCounts.ContainsKey(word))
                    {
                        wordCounts[word] = 1;
                    }
                    else
                    {
                        wordCounts[word]++;
                    }
                }
            }

            // Finden Sie die Wörter mit den niedrigsten Vorkommen (z. B. nur in wenigen Antworten)
            int minCountThreshold = 2; // Schwellenwert für die Mindestanzahl der Antworten, in denen ein Wort vorkommen muss
            List<string> importantKeywords = wordCounts.Where(kv => kv.Value <= minCountThreshold).Select(kv => kv.Key).ToList();

            return importantKeywords;
        }

        private string SelectBestAnswerByKeywords(List<string> importantKeywords, List<string> answers)
        {
            string bestAnswer = null;
            int bestKeywordMatchCount = 0;

            foreach (string answer in answers)
            {
                int keywordMatchCount = importantKeywords.Count(keyword => answer.ToLower().Contains(keyword));

                if (keywordMatchCount > bestKeywordMatchCount)
                {
                    bestKeywordMatchCount = keywordMatchCount;
                    bestAnswer = answer;
                }
            }

            return bestAnswer;
        }
    }

    public class NLPSentencedObjection
    {

        List<ObjectEntity> Nodes = new List<ObjectEntity>();
        private string conversationContext = ""; // Hinzugefügtes Feld für den Unterhaltungskontext
        public NLPSentencedObjection()
        {
            List<string> levensteinWordsList = new List<string>(
                File.ReadLines("FILTERFILE.txt")
            );

            string[] levensteinWords;
            bool explainDeeper = false;

            TrainByFile("trainfile.txt");

            // Summarize wordlist
            foreach (ObjectEntity objectEntity in Nodes)
            {
                levensteinWordsList.Add(objectEntity.name);
            }
            levensteinWords = levensteinWordsList.ToArray();

            // Cleanup console input and prepare prompt
            string[] prompt = Console.ReadLine().ToLower().Split(' ');
            string stringPrompt = "";

            // Iterate prompt parts and parse
            for (int i = 0; i < prompt.Length; i++)
            {
                prompt[i] = WordProcessing.FindBestMatch(prompt[i], levensteinWords, 0.4);
                stringPrompt += prompt[i] + " ";

                // Aktualisieren Sie den Kontext basierend auf der Benutzereingabe
                conversationContext += " " + prompt[i];

                if (Nodes.Any(ObjectEntity => ObjectEntity.name == prompt[i]))
                {
                    ObjectEntity relevantEntity = Nodes.FirstOrDefault(ObjectEntity => ObjectEntity.name == prompt[i]);

                    if (relevantEntity != null)
                    {
                        if (stringPrompt.Contains("detail") || stringPrompt.Contains("genauer") || stringPrompt.Contains("erkläre") || stringPrompt.Contains("erläutere") || stringPrompt.Contains("definiere"))
                        {
                            explainDeeper = true;
                        }
                        ShowRelevantEntity(relevantEntity, explainDeeper);
                    }
                    else
                    {
                        Console.WriteLine("Node vom Namen {0} konnte nicht gefunden werden", prompt[i]);
                    }
                }
            }
            Console.WriteLine(stringPrompt);
        }

        public void TrainByFile(string path)
        {
            if (File.Exists(path))
            {
                string[] promptArray = File.ReadAllLines(path);
                foreach (string prompt in promptArray)
                {
                    ObjectEntity objectEntity = ParseObject(prompt);
                    if (objectEntity != null)
                    {
                        Nodes.Add(objectEntity);
                    }
                    else
                    {
                        Console.WriteLine("Dirty Trainprompt '" + prompt + "'!");
                    }
                }
                CreateRelations(); // Creating relation between nodes.
            }
        }

        public ObjectEntity ParseObject(string prompt)
        {
            if (prompt.EndsWith(".") || prompt.EndsWith("!") || prompt.EndsWith("?")) { prompt = prompt.Substring(0, prompt.Length - 1); }
            if (prompt.StartsWith("Bei "))
            {
                prompt = prompt.Substring(4);
            }

            string pattern = @"( ist | war | handelt es sich um | sind )";
            string[] parts = Regex.Split(prompt, pattern);

            string name = parts[0];
            string description = parts[2];

            for (int i = 3; i < parts.Length - 1; i++)
            {
                description += parts[i];
            }

            ObjectEntity objectEntity = new ObjectEntity(name);
            objectEntity.description = description;
            objectEntity.shortDescription = description.Split(".")[0] + ".";
            if (description.StartsWith("ein "))
            {
                objectEntity.gender = 1;
            }
            else if (description.StartsWith("eine "))
            {
                objectEntity.gender = 2;
            }
            return objectEntity;
        }


        public void ShowRelevantEntity(ObjectEntity objectEntity, bool detailed = false)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(objectEntity.name + ":" + (detailed ? objectEntity.description : objectEntity.shortDescription));

            // Fügen Sie den aktuellen Kontext hinzu
            Console.WriteLine("Kontext: " + conversationContext);

            if (detailed)
            {
                if (objectEntity.outputNode.Count > 0)
                {
                    ObjectEntity subEntity = objectEntity.outputNode.ElementAt(0);
                    string subDescription = objectEntity.description.Substring(objectEntity.shortDescription.Length);

                    string pattern = @"(\.|\?|\!)";
                    string[] parts = Regex.Split(subDescription, pattern);

                    string name = parts[0];
                    string description = "";

                    for (int i = 2; i < parts.Length - 1; i++)
                    {
                        if (parts[i].Contains(subEntity.name))
                        {
                            Console.WriteLine(parts[i]);
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(subEntity.name + " ist " + subEntity.shortDescription);
                            break;
                        }
                    }
                }
            }
        }

        public void ShowRelations()
        {
            foreach (ObjectEntity objectEntity in Nodes)
            {
                Console.WriteLine(objectEntity.name);
                foreach (ObjectEntity subObjectEntity in objectEntity.outputNode)
                {
                    Console.WriteLine("\t\t>>" + subObjectEntity.name);
                }
            }
        }

        int iterations = 0;
        public void ShowRelationsRecursive(ObjectEntity entity, int iteration)
        {
            Console.WriteLine(entity.name + " nr" + iteration);
            foreach (ObjectEntity subObjectEntity in entity.outputNode)
            {
                Console.WriteLine("\t".PadRight(iteration, '\t') + "\t>>" + subObjectEntity.name);
                ShowRelationsRecursive(subObjectEntity, iteration);
            }
            iteration++;
            
        }

        public void CreateRelations()
        {
            foreach (ObjectEntity entity in Nodes)
            {
                foreach (ObjectEntity subEntity in Nodes)
                {
                    if (subEntity != entity && subEntity.description.Contains(entity.name))
                    {
                        entity.AddInputNode(subEntity);
                        subEntity.AddOutputNode(entity);
                    }
                }
            }
        }

    }

    class TokenizedNeuralNetwork
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;
        private double learningRate;
        private List<double> inputLayer;
        private List<double> hiddenLayer;
        private List<double> outputLayer;
        private List<List<double>> weightsInputToHidden;
        private List<List<double>> weightsHiddenToOutput;
        private Dictionary<string, string> questionAnswers;

        public TokenizedNeuralNetwork(int inputSize, int hiddenSize, int outputSize, double learningRate)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;
            this.learningRate = learningRate;

            inputLayer = new List<double>(inputSize);
            hiddenLayer = new List<double>(hiddenSize);
            outputLayer = new List<double>(outputSize);

            // Initialisieren Sie die Gewichtsmatrizen zufällig
            weightsInputToHidden = InitializeWeights(inputSize, hiddenSize);
            weightsHiddenToOutput = InitializeWeights(hiddenSize, outputSize);

            questionAnswers = new Dictionary<string, string>();
        }

        private List<List<double>> InitializeWeights(int numRows, int numCols)
        {
            Random rand = new Random();
            List<List<double>> weights = new List<List<double>>();
            for (int i = 0; i < numRows; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < numCols; j++)
                {
                    row.Add(rand.NextDouble() * 2 - 1); // Zufällige Werte zwischen -1 und 1
                }
                weights.Add(row);
            }
            return weights;
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        private void FeedForward(List<double> inputs)
        {
            inputLayer = new List<double>(inputs);

            // Berechnen Sie die Ausgabe der versteckten Schicht
            hiddenLayer = new List<double>();
            for (int i = 0; i < hiddenSize; i++)
            {
                double weightedSum = 0;
                for (int j = 0; j < inputSize; j++)
                {
                    weightedSum += inputLayer[j] * weightsInputToHidden[j][i];
                }
                hiddenLayer.Add(Sigmoid(weightedSum));
            }

            // Berechnen Sie die Ausgabe der Ausgabeschicht
            outputLayer = new List<double>();
            for (int i = 0; i < outputSize; i++)
            {
                double weightedSum = 0;
                for (int j = 0; j < hiddenSize; j++)
                {
                    weightedSum += hiddenLayer[j] * weightsHiddenToOutput[j][i];
                }
                outputLayer.Add(Sigmoid(weightedSum));
            }
        }

        private void Backpropagation(List<double> target)
        {
            // Berechnen Sie den Fehler
            List<double> outputErrors = outputLayer;
            for (int i = 0; i < outputSize; i++)
            {
                outputErrors[i] = (target[i] - outputLayer[i]) * outputLayer[i] * (1 - outputLayer[i]);
            }

            // Berechnen Sie die Gradienten und aktualisieren Sie die Gewichtsmatrizen
            List<double> hiddenErrors = new List<double>();
            for (int i = 0; i < hiddenSize; i++)
            {
                double error = 0;
                for (int j = 0; j < outputSize; j++)
                {
                    error += outputErrors[j] * weightsHiddenToOutput[i][j];
                }
                hiddenErrors.Add(hiddenLayer[i] * (1 - hiddenLayer[i]) * error);
            }

            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenSize; j++)
                {
                    weightsInputToHidden[i][j] += learningRate * hiddenErrors[j] * inputLayer[i];
                }
            }

            for (int i = 0; i < hiddenSize; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    weightsHiddenToOutput[i][j] += learningRate * outputErrors[j] * hiddenLayer[i];
                }
            }
        }

        public void Train(List<List<double>> inputs, List<List<double>> targets, int epochs)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                double totalError = 0;
                for (int i = 0; i < inputs.Count; i++)
                {
                    List<double> input = inputs[i];
                    List<double> target = targets[i];

                    FeedForward(input);
                    Backpropagation(target);

                    double instanceError = outputLayer.Zip(target, (output, t) => Math.Pow(t - output, 2)).Sum() / 2;
                    totalError += instanceError;
                }

                Console.WriteLine($"Epoch {epoch + 1}, Error: {totalError}");
            }
        }

        public List<double> Predict(List<double> inputs)
        {
            FeedForward(inputs);
            return outputLayer;
        }

        public string GenerateResponse(string userInput)
        {
            List<double> words = TokenizeInput(userInput.ToLower());
            FeedForward(words);
            string modifiedInputString = string.Join(", ", words);

            if (questionAnswers.ContainsKey(modifiedInputString))
            {
                return questionAnswers[modifiedInputString];
            }
            else
            {
                return "Entschuldigung, ich habe keine Antwort auf diese Frage.";
            }
        }

        private List<double> TokenizeInput(string input)
        {
            List<double> tokens = new List<double>();

            foreach (string word in input.Split(' '))
            {
                tokens.Add(GetTokenValue(word));
            }

            return tokens;
        }

        private double GetTokenValue(string word)
        {
            // Hier können Sie die Zuordnung von Wörtern zu Token-Werten basierend auf Ihren Daten definieren.
            // Zum Beispiel: Adjektive = 1.0, Nomen = 2.0, Verben = 3.0
            // Stellen Sie sicher, dass diese Methode Ihre Token-Werte korrekt zurückgibt.

            if (word == "was" || word == "ist" || word == "die" || word == "der" || word == "ein" || word == "eine" || word == "der" || word == "und" || word == "von")
            {
                return 0.0; // Ignorieren von bestimmten Stoppwörtern
            }
            else if (word == "rahasia" || word == "magische" || word == "akademie" || word == "erzmagier" || word == "orden" || word == "elementari" || word == "hauptstadt"
                || word == "abenteurer-gilde" || word == "leitet" || word == "akademie" || word == "tempelschrein" || word == "ark'zoth" || word == "tale" || word == "goldfeder"
                || word == "grush" || word == "blutfaust" || word == "grim" || word == "kupferbart" || word == "zwielicht-ebene" || word == "geschichte" || word == "gilden"
                || word == "erze" || word == "kristalle" || word == "kreislauf" || word == "kann" || word == "ich" || word == "dich" || word == "über")
            {
                return 1.0;
            }
            else if (word == "rahasia?" || word == "akademie?" || word == "erzmagier?" || word == "orden?" || word == "hauptstadt?" || word == "abenteurer-gilde?"
                || word == "leitet?" || word == "akademie?" || word == "tempelschrein?" || word == "ark'zoth?" || word == "tale?" || word == "goldfeder?" || word == "grush?"
                || word == "blutfaust?" || word == "grim?" || word == "kupferbart?" || word == "zwielicht-ebene?" || word == "geschichte?" || word == "gilden?" || word == "erze?"
                || word == "kristalle?" || word == "kreislauf?" || word == "kann?" || word == "ich?" || word == "dich?" || word == "über?")
            {
                return 2.0;
            }
            else
            {
                return 3.0; // Standard-Token-Wert für unbekannte Wörter
            }
        }

        public void AddQuestionAnswer(string question, string answer)
        {
            string modifiedQuestion = string.Join(", ", TokenizeInput(question.ToLower()));
            questionAnswers[modifiedQuestion] = answer;
        }

        public static void Run()
        {
            int inputSize = 3; // Anzahl der Eingabekategorien (z. B. Token in der Frage)
            int hiddenSize = 4;
            int outputSize = 1; // Ein Ausgabeneuron für die Antwort
            double learningRate = 0.1;

            TokenizedNeuralNetwork neuralNetwork = new TokenizedNeuralNetwork(inputSize, hiddenSize, outputSize, learningRate);

            // Laden der Fragen und Antworten aus den Dateien
            List<string> questions = File.ReadAllLines("./.train/question.txt").ToList();
            List<string> answers = File.ReadAllLines("./.train/answer.txt").ToList();

            // Tokenisieren und vorverarbeiten Sie die Fragen und Antworten
            List<List<double>> tokenizedQuestions = new List<List<double>>();
            List<List<double>> tokenizedAnswers = new List<List<double>>();

            foreach (string question in questions)
            {
                tokenizedQuestions.Add(neuralNetwork.TokenizeInput(question.ToLower()));
            }

            foreach (string answer in answers)
            {
                tokenizedAnswers.Add(neuralNetwork.TokenizeInput(answer.ToLower()));
            }

            // Trainieren Sie das neuronale Netzwerk, um Muster in den tokenisierten Fragen und Antworten zu lernen
            neuralNetwork.Train(tokenizedQuestions, tokenizedAnswers, epochs: 10000);

            Console.WriteLine("Das neuronale Netzwerk wurde trainiert. Geben Sie eine Frage ein oder beenden Sie mit 'exit':");

            while (true)
            {
                Console.Write("Frage: ");
                string userInput = Console.ReadLine();

                if (userInput.ToLower() == "exit")
                {
                    break;
                }

                string response = neuralNetwork.GenerateResponse(userInput);
                Console.WriteLine("Antwort: " + response);
            }
        }
    }

}



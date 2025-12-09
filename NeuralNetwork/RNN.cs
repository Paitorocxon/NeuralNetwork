using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

namespace SPLAI_2.NeuralNetwork
{
    class RNN
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;
        private Matrix<double> weightsInputHidden;
        private Matrix<double> weightsHiddenOutput;
        private Vector<double> hiddenState;

        public RNN(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            // Initialisieren Sie die Gewichtsmatrizen und den versteckten Zustand.
            weightsInputHidden = Matrix<double>.Build.Random(hiddenSize, inputSize, new ContinuousUniform(0, 1));
            weightsHiddenOutput = Matrix<double>.Build.Random(outputSize, hiddenSize, new ContinuousUniform(0, 1));
            hiddenState = Vector<double>.Build.Dense(hiddenSize);
        }

        public void Train(Vector<double> inputVector, Vector<double> targetVector, double learningRate)
        {
            // Implementieren Sie den Trainingsschritt für das RNN.
            // Aktualisieren Sie die Gewichtsmatrizen basierend auf dem Fehler zwischen
            // dem erzeugten Ausgabewert und dem Zielwert.

            // Stellen Sie sicher, dass inputVector und hiddenState die richtige Dimension haben.
            if (inputVector.Count != inputSize || hiddenState.Count != hiddenSize)
            {
                throw new ArgumentException("Invalid input vector dimension");
            }

            // Berechnen Sie den Ausgabewert des RNN.
            var hiddenInput = weightsInputHidden * inputVector + weightsHiddenOutput * hiddenState;
            var hiddenOutput = hiddenInput.Map(Sigmoid);
            var outputInput = weightsHiddenOutput * hiddenOutput;
            var networkOutput = outputInput.Map(Sigmoid);

            // Berechnen Sie den Fehler (Differenz zwischen Zielwert und erzeugtem Wert).
            var outputError = targetVector - networkOutput;

            // Berechnen Sie die Gradienten für die Gewichtsmatrizen.
            var deltaOutput = outputError.PointwiseMultiply(networkOutput.PointwiseMultiply(1 - networkOutput));
            var deltaHidden = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput).PointwiseMultiply(hiddenOutput.PointwiseMultiply(1 - hiddenOutput));

            // Aktualisieren Sie die Gewichtsmatrizen.
            weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
            weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();

            // Aktualisieren Sie den versteckten Zustand für den nächsten Zeitschritt.
            hiddenState = hiddenOutput;
        }


        public Vector<double> Predict(Vector<double> inputVector)
        {
            // Implementieren Sie die Vorhersage (Inferenz) des RNN.
            // Geben Sie den Ausgabewert des RNN für eine gegebene Eingabe zurück.

            var hiddenInput = weightsInputHidden * inputVector + weightsHiddenOutput * hiddenState;
            var hiddenOutput = hiddenInput.Map(Sigmoid);
            var outputInput = weightsHiddenOutput * hiddenOutput;
            var networkOutput = outputInput.Map(Sigmoid);

            // Aktualisieren Sie den versteckten Zustand für den nächsten Zeitschritt.
            hiddenState = hiddenOutput;

            return networkOutput;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
    }


    class NeuralNetworkRNN
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;
        private RNN rnn; // Ihr RNN-Objekt
        private Dictionary<string, int> wordToIndex;
        private Dictionary<int, string> indexToWord;

        public static void Run()
        {
            NeuralNetworkRNN neuralNetwork = CreateAndInitializeRNN();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Question Levenshtein:");
                // Geben Sie eine Antwort auf eine Eingabe aus.
                string input = Console.ReadLine();

                string response = neuralNetwork.GenerateResponse(input);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Antwort: " + response + ".\n Auf die Frage, " + input);
            }
        }

        private static NeuralNetworkRNN CreateAndInitializeRNN()
        {
            // Beispieltextdaten aus Dateien einlesen (inputText, targetText und wrongAnswerText).
            string[] inputText = File.ReadAllLines("./.train/question.txt");
            string[] targetText = File.ReadAllLines("./.train/answer.txt");
            string[] wrongAnswerText = File.ReadAllLines("./.train/wronganswer.txt");

            int inputSize = CalculateInputSize(inputText, targetText, wrongAnswerText);

            // Erstellen Sie ein neuronales Netzwerk mit entsprechender Eingabe-, versteckter und Ausgabegröße.
            var neuralNetwork = new NeuralNetworkRNN(inputSize, hiddenSize: 5, outputSize: inputSize);
            Console.WriteLine("Learning...");

            // Trainieren Sie das Netzwerk mit den Textdaten (positive und negative Beispiele).
            neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 2, learningRate: 0.0);

            return neuralNetwork;
        }

        private static int CalculateInputSize(params string[][] texts)
        {
            var uniqueWords = new HashSet<string>();
            foreach (var text in texts)
            {
                foreach (var word in text)
                {
                    uniqueWords.Add(word);
                }
            }
            return uniqueWords.Count;
        }
        public NeuralNetworkRNN(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            // Initialisieren Sie das RNN (bereits erstellt in Ihrer vorherigen Implementierung).
            rnn = new RNN(inputSize, hiddenSize, outputSize);

            // Initialisieren Sie indexToWord und wordToIndex.
            indexToWord = new Dictionary<int, string>();
            wordToIndex = new Dictionary<string, int>();
        }

        public void Train(string[] inputText, string[] targetText, string[] wrongAnswerText, int epochs, double learningRate)
        {
            // Erstellen Sie ein Wörterbuch, um Wörter in Indizes zu mappen und umgekehrt.
            wordToIndex = BuildWordToIndexDictionary(inputText, targetText, wrongAnswerText);
            indexToWord = BuildIndexToWordDictionary(wordToIndex);

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Epoch {epoch + 1}/{epochs}");

                // Zufällige Reihenfolge der Sätze für besseres Training.
                ShuffleTexts(ref inputText, ref targetText, ref wrongAnswerText);

                for (int i = 0; i < inputText.Length; i++)
                {
                    if (i >= targetText.Length || i >= wrongAnswerText.Length)
                    {
                        break; // Sicherstellen, dass wir nicht über die Grenzen der Texte hinausgehen.
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{i} -> {inputText[i]}");

                    var inputVector = TextToVector(inputText[i]);
                    var targetVector = TextToVector(targetText[i]);
                    var wrongAnswerVector = TextToVector(wrongAnswerText[i]);

                    // Trainieren Sie das RNN mit den Eingabe- und Zielvektoren.
                    rnn.Train(inputVector, targetVector, learningRate);
                    rnn.Train(inputVector, wrongAnswerVector, learningRate);
                }
            }
        }

        public string GenerateResponse(string inputText)
        {
            var inputVector = TextToVector(inputText);

            // Verwenden Sie das trainierte RNN, um eine Antwort zu generieren.
            var responseVector = rnn.Predict(inputVector);

            // Konvertieren Sie die Antwort in Text.
            var response = VectorToText(responseVector);

            return response;
        }

        private Vector<double> TextToVector(string text)
        {
            var words = text.Split(' ');
            var vector = Vector<double>.Build.Dense(inputSize);

            foreach (var word in words)
            {
                if (wordToIndex.ContainsKey(word))
                {
                    int newIndex = wordToIndex[word];

                    // Überprüfen Sie, ob newIndex außerhalb des gültigen Bereichs liegt.
                    if (newIndex >= 0 && newIndex < vector.Count)
                    {
                        vector[newIndex] = 1.0;
                    }
                    else
                    {
                        // Passen Sie den Index an den gültigen Bereich an.
                        newIndex = Math.Max(0, Math.Min(vector.Count - 1, newIndex));
                        vector[newIndex] = 1.0;
                    }
                }
                else
                {
                    // Neues Wort hinzufügen und den Index aktualisieren.
                    int newIndex = wordToIndex.Count;
                    wordToIndex[word] = newIndex;
                    indexToWord[newIndex] = word;

                    // Überprüfen Sie erneut, ob newIndex außerhalb des gültigen Bereichs liegt.
                    if (newIndex >= 0 && newIndex < vector.Count)
                    {
                        vector[newIndex] = 1.0;
                    }
                    else
                    {
                        // Passen Sie den Index an den gültigen Bereich an.
                        newIndex = Math.Max(0, Math.Min(vector.Count - 1, newIndex));
                        vector[newIndex] = 1.0;
                    }
                }
            }

            return vector;
        }

        private string VectorToText(Vector<double> vector)
        {
            var words = new List<string>();

            for (int i = 0; i < vector.Count; i++)
            {
                if (vector[i] > 0.5)
                {
                    words.Add(indexToWord[i]);
                }
            }

            return string.Join(" ", words);
        }

        private static void ShuffleTexts(ref string[] inputText, ref string[] targetText, ref string[] wrongAnswerText)
        {
            var rng = new Random();
            int minLength = Math.Min(inputText.Length, Math.Min(targetText.Length, wrongAnswerText.Length));

            for (int i = 0; i < minLength; i++)
            {
                int j = rng.Next(i, minLength);

                Swap(ref inputText[i], ref inputText[j]);
                Swap(ref targetText[i], ref targetText[j]);
                Swap(ref wrongAnswerText[i], ref wrongAnswerText[j]);
            }
        }

        private static void Swap(ref string a, ref string b)
        {
            string temp = a;
            a = b;
            b = temp;
        }




        private Dictionary<string, int> BuildWordToIndexDictionary(params string[][] texts)
        {
            var wordToIndex = new Dictionary<string, int>();
            int index = 0;

            foreach (var text in texts)
            {
                foreach (var word in text)
                {
                    if (!wordToIndex.ContainsKey(word))
                    {
                        wordToIndex[word] = index;
                        index++;
                    }
                }
            }

            return wordToIndex;
        }

        private Dictionary<int, string> BuildIndexToWordDictionary(Dictionary<string, int> wordToIndex)
        {
            var indexToWord = wordToIndex.ToDictionary(pair => pair.Value, pair => pair.Key);
            return indexToWord;
        }
    }
}

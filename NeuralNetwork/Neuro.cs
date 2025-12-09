using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SPLAI_2.NeuralNetwork
{
    internal class NeuroDouble
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;
        private Matrix<double> weightsInputHidden;
        private Matrix<double> weightsHiddenOutput;
        public NeuroDouble(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            // Initialisieren Sie die Gewichtsmatrizen mit zufälligen Werten oder anderen gewünschten Methoden.
            weightsInputHidden = Matrix<double>.Build.Random(hiddenSize, inputSize);
            weightsHiddenOutput = Matrix<double>.Build.Random(outputSize, hiddenSize);
        }


        public static void Run()
        {
            var inputVector = Vector<double>.Build.DenseOfArray(new double[] { 0.1, 0.2, 0.3 });

            // Erstellen Sie ein neuronales Netzwerk mit 3 Eingaben, 4 versteckten Neuronen und 2 Ausgaben.
            var neuralNetwork = new NeuroDouble(3, 20, 3);

            // Feedforward-Durchlauf durch das Netzwerk.
            var output = neuralNetwork.FeedForward(inputVector.ToColumnMatrix());

            Console.WriteLine("Netzwerkausgabe:");
            Console.WriteLine(output);
        }


        private static Matrix<double> Sigmoid(Matrix<double> matrix)
        {
            return matrix.Map(x => 1.0 / (1.0 + Math.Exp(-x)));
        }

        public Matrix<double> FeedForward(Matrix<double> input)
        {
            // Berechnen Sie den Ausgang der versteckten Schicht.
            var hiddenInput = weightsInputHidden * input;
            var hiddenOutput = Sigmoid(hiddenInput);

            // Berechnen Sie den endgültigen Ausgang.
            var outputInput = weightsHiddenOutput * hiddenOutput;
            var networkOutput = Sigmoid(outputInput);

            return networkOutput;
        }

    }

    [Serializable]
    class NeuralNetwork_Primitive
    {
        private int inputSize;
        private int hiddenSize;
        private int outputSize;
        private Matrix<double> weightsInputHidden;
        private Matrix<double> weightsHiddenOutput;
        private Dictionary<string, int> wordToIndex;
        private Dictionary<int, string> indexToWord;
        private double bestValidationError = double.MaxValue; // Variable zur Verfolgung des besten Validierungsfehlers

        public NeuralNetwork_Primitive(int inputSize, int hiddenSize, int outputSize)
        {
            this.inputSize = inputSize;
            this.hiddenSize = hiddenSize;
            this.outputSize = outputSize;

            // Initialisieren Sie die Gewichtsmatrizen mit zufälligen Werten oder anderen gewünschten Methoden.
            weightsInputHidden = Matrix<double>.Build.Random(hiddenSize, inputSize);
            weightsHiddenOutput = Matrix<double>.Build.Random(outputSize, hiddenSize);

            // Initialisieren Sie indexToWord und wordToIndex.
            indexToWord = new Dictionary<int, string>();
            wordToIndex = new Dictionary<string, int>();
        }


        public static void Run(bool trainParallel = false)
        {
            // Beispieltextdaten aus Dateien einlesen (inputText, targetText und wrongAnswerText).
            //string[] inputText = File.ReadAllLines("./.train/question.txt");
            //string[] targetText = File.ReadAllLines("./.train/answer.txt");
            //string[] inputText = File.ReadAllLines("./.train/question.txt");
            //string[] targetText = File.ReadAllLines("./.train/answer.txt");
            //string[] wrongAnswerText = File.ReadAllLines("./.train/wronganswer.txt");


            string[] inputText = File.ReadAllLines("./.train/question.txt");
            string[] targetText = File.ReadAllLines("./.train/answer.txt");
            string[] wrongAnswerText = File.ReadAllLines("./.train/wronganswer.txt");

            string[] levenshteinWords = new List<string>(File.ReadAllLines("./.train/question.txt")).ToArray();

            // Erstellen Sie ein neuronales Netzwerk mit entsprechender Eingabe-, versteckter und Ausgabegröße.
            var neuralNetwork = new NeuralNetwork_Primitive(inputSize: 320, hiddenSize: 20, outputSize: 320);
            Console.WriteLine("learning...");



            Random r = new Random();


            // Trainieren Sie das Netzwerk mit den Textdaten (positive und negative Beispiele).
            if (!trainParallel)
            { // Serielle verarbeitung!
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 10, learningRate: 0.6);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 6, learningRate: 0.1);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 12, learningRate: 10.1);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 3, learningRate: 10.1);


                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: r.Next(1,3), learningRate: 3.6);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: r.Next(1, 6), learningRate: 9.1);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: r.Next(1, 12), learningRate: 0.1);
                neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: r.Next(1, 24), learningRate: 0.01);

            }
            else
            {
                neuralNetwork.TrainParallel(inputText, targetText, wrongAnswerText, epochs: 10, learningRate: 0.1, 4);

            }

            while (true)
            {

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Question Levenshtein:");
                // Geben Sie eine Antwort auf eine Eingabe aus.
                string input = WordProcessing.FindBestMatch(Console.ReadLine(), levenshteinWords, 0.2);

                string response = neuralNetwork.GenerateResponse(input);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Antwort: " + response + ".\n Auf die Frage, " + input);


            }
        }

        public static void RunWithValidation()
        {
            // Beispieltextdaten aus Dateien einlesen (inputText und targetText).
            string[] inputText = File.ReadAllLines("./.train/question.txt");
            string[] targetText = File.ReadAllLines("./.train/answer.txt");

            // Erstellen Sie ein neuronales Netzwerk mit entsprechender Eingabe-, versteckter und Ausgabegröße.
            var neuralNetwork = new NeuralNetwork_Primitive(inputSize: 100, hiddenSize: 20, outputSize: 100);

            // Trainieren Sie das Netzwerk mit den Trainingsdaten und Validierungsdaten.
            // Die Methode TrainWithValidation verwendet eine Aufteilung der Daten für Training und Validierung.
            neuralNetwork.TrainWithValidation(inputText, targetText, epochs: 1000, learningRate: 0.1, validationSplit: 0.2);

            // Geben Sie eine Antwort auf eine Eingabe aus.
            string input = Console.ReadLine();
            string response = neuralNetwork.GenerateResponse(input);
            Console.WriteLine("Antwort: " + response);
        }


        private static Vector<double> Sigmoid(Vector<double> vector)
        {
            return vector.Map(x => 1.0 / (1.0 + Math.Exp(-x)));
        }

        public void Train(string[] inputText, string[] targetText, string[] wrongAnswerText, int epochs, double learningRate)
        {
            // Erstellen Sie ein Wörterbuch, um Wörter in Indizes zu mappen und umgekehrt.
            wordToIndex = inputText
                .Concat(targetText)
                .Concat(wrongAnswerText) // Hinzufügen der falschen Antworten zur Vokabelliste
                .Distinct()
                .Select((word, index) => new { word, index })
                .ToDictionary(pair => pair.word, pair => pair.index);
            indexToWord = wordToIndex.ToDictionary(pair => pair.Value, pair => pair.Key);

            Console.Clear();
            for (int epoch = 0; epoch < epochs; epoch++)
            {

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(0, 0);
                Console.Write("Epoch " + epoch + "/" + epochs + "\n");
                for (int i = 0; i < inputText.Length; i++)
                {
                    if (i >= targetText.Length || i >= wrongAnswerText.Length)
                    {
                        break; // Sicherstellen, dass wir nicht über die Grenzen der Texte hinausgehen
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.SetCursorPosition(0, 1);
                    Console.WriteLine(i + "->" +  WordProcessing.ShuffleWords(inputText[i], inputText));
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    var inputVector = TextToVector(WordProcessing.ShuffleWords(inputText[i],inputText));
                    var targetVector = TextToVector(targetText[i]);
                    var wrongAnswerVector = TextToVector(WordProcessing.ShuffleWords(wrongAnswerText[i], wrongAnswerText)); // Hinzufügen der falschen Antwort

                    // Feedforward-Durchlauf durch das Netzwerk für positive Antwort.
                    var hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                    var hiddenOutput = Sigmoid(hiddenInput.Column(0));
                    var outputInput = weightsHiddenOutput * hiddenOutput;
                    var networkOutput = Sigmoid(outputInput);
                    Console.WriteLine(">>" + hiddenInput + ">>" + outputInput);
                    Console.WriteLine(">>" + outputInput + ">>" + networkOutput);


                    // Berechnen Sie den Fehler und aktualisieren Sie die Gewichtsmatrizen durch Backpropagation für positive Antwort.
                    var outputError = targetVector - networkOutput;
                    var deltaOutput = outputError.PointwiseMultiply(networkOutput).PointwiseMultiply(1.0 - networkOutput);
                    var hiddenError = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput);
                    var deltaHidden = hiddenError.PointwiseMultiply(hiddenOutput).PointwiseMultiply(1.0 - hiddenOutput);

                    weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
                    weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();

                    // Feedforward-Durchlauf durch das Netzwerk für negative Antwort.
                    hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                    hiddenOutput = Sigmoid(hiddenInput.Column(0));
                    outputInput = weightsHiddenOutput * hiddenOutput;
                    networkOutput = Sigmoid(outputInput);

                    // Berechnen Sie den Fehler und aktualisieren Sie die Gewichtsmatrizen durch Backpropagation für negative Antwort.
                    outputError = wrongAnswerVector - networkOutput;
                    deltaOutput = outputError.PointwiseMultiply(networkOutput).PointwiseMultiply(1.0 - networkOutput);
                    hiddenError = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput);
                    deltaHidden = hiddenError.PointwiseMultiply(hiddenOutput).PointwiseMultiply(1.0 - hiddenOutput);

                    weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
                    weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();
                }
            }
        }

        public void TrainParallel(string[] inputText, string[] targetText, string[] wrongAnswerText, int epochs, double learningRate, int numThreads)
        {
            // Erstellen Sie ein Wörterbuch, um Wörter in Indizes zu mappen und umgekehrt.
            wordToIndex = inputText
                .Concat(targetText)
                .Concat(wrongAnswerText)
                .Distinct()
                .Select((word, index) => new { word, index })
                .ToDictionary(pair => pair.word, pair => pair.index);
            indexToWord = wordToIndex.ToDictionary(pair => pair.Value, pair => pair.Key);

            Console.Clear();
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(0, 0);
                Console.Write("Epoch " + epoch + "/" + epochs + "\n");

                Parallel.For(0, inputText.Length, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, i =>
                {
                    if (i >= targetText.Length || i >= wrongAnswerText.Length)
                    {
                        return; // Sicherstellen, dass wir nicht über die Grenzen der Texte hinausgehen
                    }

                    Console.WriteLine(i + "->" + inputText[i]);

                    var inputVector = TextToVector(inputText[i].Replace("?", "").Replace("!", "").Replace(".", "").Replace(",", ""));
                    var targetVector = TextToVector(targetText[i].Replace("?", "").Replace("!", "").Replace(".", "").Replace(",", ""));
                    var wrongAnswerVector = TextToVector(wrongAnswerText[i].Replace("?", "").Replace("!", "").Replace(".", "").Replace(",", ""));

                    // Feedforward-Durchlauf durch das Netzwerk für positive Antwort.
                    var hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                    var hiddenOutput = Sigmoid(hiddenInput.Column(0));
                    var outputInput = weightsHiddenOutput * hiddenOutput;
                    var networkOutput = Sigmoid(outputInput);

                    // Berechnen Sie den Fehler und aktualisieren Sie die Gewichtsmatrizen durch Backpropagation für positive Antwort.
                    var outputError = targetVector - networkOutput;
                    var deltaOutput = outputError.PointwiseMultiply(networkOutput).PointwiseMultiply(1.0 - networkOutput);
                    var hiddenError = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput);
                    var deltaHidden = hiddenError.PointwiseMultiply(hiddenOutput).PointwiseMultiply(1.0 - hiddenOutput);

                    lock (this)
                    {
                        weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
                        weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();
                    }

                    // Feedforward-Durchlauf durch das Netzwerk für negative Antwort.
                    hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                    hiddenOutput = Sigmoid(hiddenInput.Column(0));
                    outputInput = weightsHiddenOutput * hiddenOutput;
                    networkOutput = Sigmoid(outputInput);

                    // Berechnen Sie den Fehler und aktualisieren Sie die Gewichtsmatrizen durch Backpropagation für negative Antwort.
                    outputError = wrongAnswerVector - networkOutput;
                    deltaOutput = outputError.PointwiseMultiply(networkOutput).PointwiseMultiply(1.0 - networkOutput);
                    hiddenError = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput);
                    deltaHidden = hiddenError.PointwiseMultiply(hiddenOutput).PointwiseMultiply(1.0 - hiddenOutput);

                    lock (this)
                    {
                        weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
                        weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();
                    }
                });
            }
        }

        public string GenerateResponse(string inputText)
        {
            var inputVector = TextToVector(inputText);

            // Feedforward-Durchlauf durch das Netzwerk.
            var hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
            var hiddenOutput = Sigmoid(hiddenInput.Column(0));
            var outputInput = weightsHiddenOutput * hiddenOutput;
            var networkOutput = Sigmoid(outputInput);

            // Konvertieren Sie die Netzwerkausgabe in Text.
            var response = VectorToText(networkOutput);

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

        // Validation

        public void TrainWithValidation(string[] inputText, string[] targetText, int epochs, double learningRate, double validationSplit)
        {
            // Aufteilung der Daten in Trainingsdaten und Validierungsdaten
            int dataSize = inputText.Length;
            int validationSize = (int)(dataSize * validationSplit);
            int trainingSize = dataSize - validationSize;

            string[] trainingInput = inputText.Take(trainingSize).ToArray();
            string[] trainingTarget = targetText.Take(trainingSize).ToArray();

            string[] validationInput = inputText.Skip(trainingSize).ToArray();
            string[] validationTarget = targetText.Skip(trainingSize).ToArray();

            // Trainingsdaten- und Validierungsdaten-Dictionary erstellen
            wordToIndex = trainingInput
                .Concat(trainingTarget)
                .Distinct()
                .Select((word, index) => new { word, index })
                .ToDictionary(pair => pair.word, pair => pair.index);

            // Beginnen Sie mit dem Training
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                for (int i = 0; i < trainingInput.Length; i++)
                {
                    var inputVector = TextToVector(trainingInput[i]);
                    var targetVector = TextToVector(trainingTarget[i]);

                    // Feedforward-Durchlauf durch das Netzwerk.
                    var hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                    var hiddenOutput = Sigmoid(hiddenInput.Column(0));
                    var outputInput = weightsHiddenOutput * hiddenOutput;
                    var networkOutput = Sigmoid(outputInput);

                    // Berechnen Sie den Fehler und aktualisieren Sie die Gewichtsmatrizen durch Backpropagation.
                    var outputError = targetVector - networkOutput;
                    var deltaOutput = outputError.PointwiseMultiply(networkOutput).PointwiseMultiply(1.0 - networkOutput);
                    var hiddenError = weightsHiddenOutput.TransposeThisAndMultiply(deltaOutput);
                    var deltaHidden = hiddenError.PointwiseMultiply(hiddenOutput).PointwiseMultiply(1.0 - hiddenOutput);

                    weightsHiddenOutput += learningRate * deltaOutput.ToColumnMatrix() * hiddenOutput.ToRowMatrix();
                    weightsInputHidden += learningRate * deltaHidden.ToColumnMatrix() * inputVector.ToRowMatrix();
                }

                // Validierungsfehler berechnen
                double validationError = CalculateValidationError(validationInput, validationTarget);

                // Speichern Sie das Modell, wenn der Validierungsfehler abnimmt
                if (epoch == 0 || validationError < bestValidationError)
                {
                    bestValidationError = validationError;
                    // Hier können Sie das Modell speichern (z. B. Gewichtungen und Modellparameter).
                }

                Console.WriteLine($"Epoch {epoch + 1}/{epochs}, Validation Error: {validationError}");
            }
        }

        private double CalculateValidationError(string[] validationInput, string[] validationTarget)
        {
            double totalError = 0.0;

            for (int i = 0; i < validationInput.Length; i++)
            {
                var inputVector = TextToVector(validationInput[i]);
                var targetVector = TextToVector(validationTarget[i]);

                // Feedforward-Durchlauf durch das Netzwerk.
                var hiddenInput = weightsInputHidden * inputVector.ToColumnMatrix();
                var hiddenOutput = Sigmoid(hiddenInput.Column(0));
                var outputInput = weightsHiddenOutput * hiddenOutput;
                var networkOutput = Sigmoid(outputInput);

                // Berechnen Sie den Fehler (z. B. quadratischer Fehler) und addieren Sie ihn zum Gesamtfehler.
                var outputError = targetVector - networkOutput;
                totalError += outputError.PointwiseMultiply(outputError).Sum();
            }

            // Durchschnittlicher Fehler über alle Validierungsdaten berechnen
            return totalError / validationInput.Length;
        }


    }


}

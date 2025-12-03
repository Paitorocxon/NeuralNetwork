# SPLAI_2.NeuralNetwork

**Written by Fabian Müller**  
**Copyright © 2023 Fabian Müller**

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

SPLAI_2.NeuralNetwork is a C# project developed by Fabian Müller. This project provides a neural network implementation that can be used for various applications.

## Table of Contents

- [License](#license)
- [Description](#description)
- [Usage](#usage)
- [Training](#training)
- [Contributing](#contributing)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Description

SPLAI_2.NeuralNetwork is a neural network library designed for C#. It includes features for creating, training, and using neural networks in various applications. The library is designed to be easy to use and customizable.

## Usage

To use this library in your C# project, you can follow these steps:

1. Clone or download the repository.

2. Include the `SPLAI_2.NeuralNetwork` namespace in your C# code.

3. Create an instance of the `NeuralNetwork_Primitive` class with the desired input size, hidden size, and output size.

4. Train the neural network using your training data.

5. Use the trained neural network to make predictions or generate responses.

Here's a simple example of how to create and train a neural network:

```csharp
// Create a neural network with input size 320, hidden size 20, and output size 320.
var neuralNetwork = new NeuralNetwork_Primitive(inputSize: 320, hiddenSize: 20, outputSize: 320);

// Train the network with your training data.
neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 10, learningRate: 0.6);

// Use the trained network to generate responses.
string input = "Your input text here";
string response = neuralNetwork.GenerateResponse(input);
Console.WriteLine("Response: " + response);
```
## Training

The `NeuralNetwork_Primitive` class provides methods for training the neural network using your data. You can adjust the number of epochs and the learning rate to fine-tune the training process.

```csharp
// Train the network with training data for a specified number of epochs and learning rate.
neuralNetwork.Train(inputText, targetText, wrongAnswerText, epochs: 10, learningRate: 0.6);
```

## Contributing

Contributions to this project are welcome. If you have any ideas, bug fixes, or improvements, please open an issue or create a pull request.

Enjoy using SPLAI_2.NeuralNetwork!


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;

namespace NeuralNetwork.Model.Head
{
    internal static class Dense
    {
        public static Functional GetModel(Shape inputShape, string inputName, int numberOfLayers, int numberOfOutputs, string outputName)
        {
            var layers = new LayersApi();
            // input layer
            var inputs = keras.Input(shape: inputShape, name: inputName);
            // dense layers
            int numberOfUnits = numberOfOutputs * (int)Math.Pow(2, numberOfLayers);
            var x = inputs;
            for (int i = 0; i < numberOfLayers - 1; i++)
            {
                x = layers.Dense(numberOfUnits, activation: "relu").Apply(x);
                x = layers.Dropout(0.5f).Apply(x);
                numberOfUnits /= 2;
            }
            var outputs = layers.Dense(numberOfOutputs).Apply(x);
            // build keras model
            return keras.Model(inputs, outputs, name: outputName);
        }
    }
}

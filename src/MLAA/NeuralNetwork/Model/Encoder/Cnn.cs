using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tensorflow.KerasApi;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Utils;
using System.Security.Cryptography.X509Certificates;

namespace NeuralNetwork.Model.Encoder
{
    internal static class Cnn
    {
        public static Functional GetModel(Shape inputShape, int numberOfLayers)
        {
            var layers = new LayersApi();
            // input layer
            var inputs = keras.Input(shape: inputShape, name: "ff");
            var x = inputs;
            // convolutional layers
            int currentFilterSize = 16;
            for (int i = 0; i < numberOfLayers - 1; i++)
            {
                x = layers.Conv2D(currentFilterSize, 3, activation: "relu").Apply(x);
                x = layers.MaxPooling2D(3).Apply(x);
                currentFilterSize *= 2;
            }
            var outputs = layers.Conv2D(currentFilterSize, 3, activation: "relu").Apply(x);
            
            // build keras model
            return keras.Model(inputs, outputs, name: "gg");
        }
    }
}

using Tensorflow;
using Tensorflow.Keras.Layers;
using Tensorflow.Keras.Utils;
using static Tensorflow.KerasApi;

namespace NeuralNetwork
{
    public class Class1
    {
        public static void Test()
        {
            // input layer
            var inputs = keras.Input(shape: new Shape(32, 32, 3), name: "inputImage");
            var encoder = Model.Encoder.Cnn.GetModel(new Shape(32, 32, 3), 1);
            var encodedFeatures = encoder.Apply(inputs);   
            var head = Model.Head.Dense.GetModel(encoder.output_shape, "features", 1, 10, "head");
            var outputs = head.Apply(encodedFeatures);
            var model = keras.Model(inputs, outputs, name: "model");
            model.summary();
            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.RMSprop(1e-3f),
                loss: keras.losses.CategoricalCrossentropy(from_logits: true),
                metrics: new[] { "acc" });
            // prepare dataset
            var ((x_train, y_train), (x_test, y_test)) = keras.datasets.cifar10.load_data();
            x_train = x_train / 255.0f;
            y_train = np_utils.to_categorical(y_train, 10);
            // training
            /*model.fit(x_train[new Slice(0, 2000)], y_train[new Slice(0, 2000)],
                      batch_size: 64,
                      epochs: 10,
                      validation_split: 0.2f);*/
            for (int epoch = 0; epoch < 10; epoch++)
            {
                model.fit(x_train[new Slice(0, 2000)], y_train[new Slice(0, 2000)],
                      batch_size: 64,
                      epochs: 1,
                      validation_split: 0.2f);
                Console.WriteLine("Epoch: " + epoch);
            }
        }
        public static void Test2()
        {
            var layers = new LayersApi();
            // input layer
            var inputs = keras.Input(shape: (32, 32, 3), name: "img");
            // convolutional layer
            var x = layers.Conv2D(32, 3, activation: "relu").Apply(inputs);
            x = layers.Conv2D(64, 3, activation: "relu").Apply(x);
            var block_1_output = layers.MaxPooling2D(3).Apply(x);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(block_1_output);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x);
            var block_2_output = layers.Add().Apply(new Tensors(x, block_1_output));
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(block_2_output);
            x = layers.Conv2D(64, 3, activation: "relu", padding: "same").Apply(x);
            var block_3_output = layers.Add().Apply(new Tensors(x, block_2_output));
            x = layers.Conv2D(64, 3, activation: "relu").Apply(block_3_output);
            x = layers.GlobalAveragePooling2D().Apply(x);
            x = layers.Dense(256, activation: "relu").Apply(x);
            x = layers.Dropout(0.5f).Apply(x);
            // output layer
            var outputs = layers.Dense(10).Apply(x);
            // build keras model
            var model = keras.Model(inputs, outputs, name: "toy_resnet");
            model.summary();
            // compile keras model in tensorflow static graph
            model.compile(optimizer: keras.optimizers.RMSprop(1e-3f),
                loss: keras.losses.CategoricalCrossentropy(from_logits: true),
                metrics: new[] { "acc" });
            // prepare dataset
            var ((x_train, y_train), (x_test, y_test)) = keras.datasets.cifar10.load_data();
            x_train = x_train / 255.0f;
            y_train = np_utils.to_categorical(y_train, 10);
            // training
            /*model.fit(x_train[new Slice(0, 2000)], y_train[new Slice(0, 2000)],
                      batch_size: 64,
                      epochs: 10,
                      validation_split: 0.2f);*/
            for (int epoch = 0; epoch < 10; epoch++)
            {
                model.fit(x_train[new Slice(0, 2000)], y_train[new Slice(0, 2000)],
                      batch_size: 64,
                      epochs: 1,
                      validation_split: 0.2f);
                Console.WriteLine("Epoch: " + epoch);
            }

        }
    }
}
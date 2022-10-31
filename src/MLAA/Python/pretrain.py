import tensorflow as tf
import numpy as np

DEBUG = False
if (DEBUG and __name__ == "__main__"):
    # dont forget to use @tf.function decorator!
    tf.config.run_functions_eagerly(True)
    tf.config.experimental_run_functions_eagerly(True)
    tf.data.experimental.enable_debug_mode()


def autoencoder(input_shape: tuple[int, int, int], cnn_depth: int):
    input = tf.keras.layers.Input(input_shape, name="encoder_input")
    x = input
    for i in range(cnn_depth):
        x = tf.keras.layers.Conv2D(
            2**(i+3), (3, 3), padding="same", activation="relu")(x)
        x = tf.keras.layers.MaxPool2D()(x)
        x = tf.keras.layers.BatchNormalization()(x)
    encoder = tf.keras.Model(input, x, name="encoder")
    input = tf.keras.layers.Input(
        encoder.output_shape[1:], name="decoder_input")
    x = input
    for i in range(cnn_depth):
        x = tf.keras.layers.UpSampling2D()(x)
        filters = 2**(cnn_depth-i+2)
        # in the last layer, we want to use the same number of filters as the input
        if (i == cnn_depth-1):
            filters = input_shape[2]
        x = tf.keras.layers.Conv2DTranspose(
            filters, (3, 3), strides=(1, 1), padding="same", activation="relu")(x)
        x = tf.keras.layers.BatchNormalization()(x)
    decoder = tf.keras.Model(input, x, name="decoder")
    encoder.summary()
    decoder.summary()
    return encoder, decoder


def train_autoencoder(encoder: tf.keras.Model, decoder: tf.keras.Model, dataset: tf.data.Dataset, epochs: int, batch_size: int):
    autoencoder = tf.keras.Model(encoder.input, decoder(
        encoder.output), name="autoencoder")
    autoencoder.compile(optimizer="adam", loss="mse")
    autoencoder.fit(dataset, epochs=epochs, batch_size=batch_size)


# Only add this if you want to debug the code. In "release" mode this causes an error
# @tf.function
def load_dataset_from_csv(csv_path: str, image_shape: tuple[int, int, int], batch_size: int):
    # Only add this if you want to debug the code. In "release" mode this causes an error
    # @tf.function
    def parse_csv_line(line: str):
        line_parts = tf.strings.split(line, sep=',')
        image_path = "./valorant/" + line_parts[0]
        image = tf.io.read_file(image_path)
        image = tf.io.decode_jpeg(image, channels=3)
        image = tf.image.random_crop(image, image_shape)
        image = tf.cast(image, tf.float32)
        image = image / 255.0
        return image, image
    dataset = tf.data.TextLineDataset(csv_path)
    dataset = dataset.skip(1)
    dataset = dataset.map(parse_csv_line)
    dataset = dataset.batch(batch_size)
    return dataset


def run():
    encoder, decoder = autoencoder((None, None, 3), 5)
    dataset = load_dataset_from_csv(
        "./valorant/dataset.csv", (144*2, 256*2, 3), 32)
    train_autoencoder(encoder, decoder, dataset, 25, 32)
    encoder.save("encoder.h5")
    decoder.save("decoder.h5")

    # test the model on the first 10 samples in the dataset
    for i, (image, _) in enumerate(dataset.take(10)):
        encoded = encoder.predict(image)
        decoded = decoder.predict(encoded)
        tf.keras.utils.save_img(
            f"./valorant/prediction_{i}.png", decoded[0])
        tf.keras.utils.save_img(
            f"./valorant/original_{i}.png", image[0])
        # save encoded raw data
        np.save(f"./valorant/encoded_{i}.npy", encoded[0])
    # try on a random images
    image = tf.keras.utils.load_img(
        "./valorant/1080p_15000_astra_breeze/1080p_15000_astra_breeze.mp4_10155.jpg")
    image = tf.keras.preprocessing.image.img_to_array(image)
    image = tf.expand_dims(image, axis=0)
    image = image / 255.0
    encoded = encoder.predict(image)
    decoded = decoder.predict(encoded)
    tf.keras.utils.save_img(
        "./valorant/prediction_random.png", decoded[0])


if __name__ == "__main__":
    run()

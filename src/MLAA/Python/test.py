from typing import Any
import numpy as np
import tensorflow as tf

DEBUG = True
if (DEBUG and __name__ == "__main__"):
    # dont forget to use @tf.function decorator!
    tf.config.run_functions_eagerly(True)
    tf.config.experimental_run_functions_eagerly(True)
    tf.data.experimental.enable_debug_mode()

csv_path = "./killfeed_data.csv"


@tf.function
def load_image(features):
    new_feats = {}
    for name, value in features.items():
        if name.endswith("path"):
            filecontent = tf.io.read_file(tf.reshape(value, []))
            new_feats[name] = tf.image.decode_jpeg(filecontent, channels=3)
        else:
            new_feats[name] = value
    return new_feats


@tf.function
def supervised_labels_mapper(example: dict[str, Any]) -> tuple[dict[str, Any], dict[str, Any]]:
    example_inputs: dict[str, Any] = dict()
    input_layer_name = "sample_path"
    example_inputs[input_layer_name] = example[input_layer_name]
    example_outputs: dict[str, Any] = dict()
    output_layer_names = ["weapon"]
    for layer_name in output_layer_names:
        example_outputs[layer_name] = example[layer_name]
    return example_inputs, example_outputs


raw_dataset = tf.data.experimental.make_csv_dataset(
    csv_path,
    batch_size=1,  # fir simple mapping (string handling).
    num_epochs=1,
    ignore_errors=False,)

for features in raw_dataset.take(1):
    for i, (name, value) in enumerate(features.items()):
        if i > 15:
            break
        print(f"{name:20s}: {value}")
print('...')
print(f"[total: {len(features)} features]")

images_dataset = raw_dataset.map(load_image)

for features in images_dataset.take(1):
    for i, (name, value) in enumerate(features.items()):
        if i > 15:
            break
        print(f"{name:20s}: {value}")
print('...')
print(f"[total: {len(features)} features]")


images_dataset = images_dataset.map(supervised_labels_mapper)
labels = np.concatenate([x["weapon"] for x in raw_dataset], axis=0)
lookup = tf.keras.layers.StringLookup(vocabulary=np.unique(labels))
one_hot = tf.keras.layers.CategoryEncoding(num_tokens=lookup.vocabulary_size())
images_dataset = images_dataset.map(
    lambda x, y: (x, {"weapon": one_hot(lookup(y["weapon"]))}))
for input, output in images_dataset.batch(6).take(1):
    for i, (name, value) in enumerate(input.items()):
        if i > 15:
            break
        print(f"input-{name:20s}: {value}")
    print(output)
    for i, (name, value) in enumerate(output.items()):
        if i > 15:
            break
        print(f"output-{name:20s}: {value}")
print('...')
print(f"[total: {len(features)} features]")

input = tf.keras.layers.Input((46, 600, 3), name="sample_path")
encoder = tf.keras.Sequential([
    tf.keras.layers.Conv2D(filters=8, kernel_size=(3, 3), strides=(1, 1), dilation_rate=(
        1, 1), padding="same", use_bias=True, kernel_initializer=tf.keras.initializers.GlorotUniform(),
        activation="relu"),
    tf.keras.layers.BatchNormalization(),
    tf.keras.layers.MaxPooling2D(pool_size=(
        2, 2), padding="same", strides=(2, 2)),
    tf.keras.layers.Conv2D(filters=16, kernel_size=(3, 3), strides=(1, 1), dilation_rate=(
        1, 1), padding="same", use_bias=True, kernel_initializer=tf.keras.initializers.GlorotUniform(),
        activation="relu"),
    tf.keras.layers.BatchNormalization(),
    tf.keras.layers.MaxPooling2D(pool_size=(
        2, 2), padding="same", strides=(2, 2)),
    tf.keras.layers.Conv2D(filters=32, kernel_size=(3, 3), strides=(1, 1), dilation_rate=(
        1, 1), padding="same",  use_bias=True, kernel_initializer=tf.keras.initializers.GlorotUniform(),
        activation="relu"),
    tf.keras.layers.BatchNormalization(),
    tf.keras.layers.MaxPooling2D(pool_size=(
        2, 2), padding="same", strides=(2, 2)),
    tf.keras.layers.Conv2D(filters=64, kernel_size=(3, 3), strides=(1, 1), dilation_rate=(
        1, 1), padding="same",  use_bias=True, kernel_initializer=tf.keras.initializers.GlorotUniform(),
        activation="relu"),
    tf.keras.layers.BatchNormalization(),
    tf.keras.layers.MaxPooling2D(pool_size=(
        2, 2), padding="same", strides=(2, 2)),
    tf.keras.layers.Flatten(),
    tf.keras.layers.Dense(64, kernel_initializer=tf.keras.initializers.GlorotUniform(),
                          activation="sigmoid"),
])
encoded_features = encoder(input)
output = tf.keras.layers.Dense(units=22, kernel_initializer=tf.keras.initializers.GlorotUniform(
), activation="softmax", name="weapon")(encoded_features)
model = tf.keras.Model(inputs=input, outputs=output)
model.compile(
    optimizer=tf.keras.optimizers.Adam(0.001),
    loss=tf.keras.losses.CategoricalCrossentropy(from_logits=False),
    metrics=["accuracy"],
)
model.summary()

model.fit(images_dataset.batch(32), epochs=5)
model.save("model.h5")

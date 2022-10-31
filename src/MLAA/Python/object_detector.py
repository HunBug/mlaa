import tensorflow as tf
import cv2
import numpy as np

DEBUG = False
if (DEBUG and __name__ == "__main__"):
    # dont forget to use @tf.function decorator!
    tf.config.run_functions_eagerly(True)
    tf.config.experimental_run_functions_eagerly(True)
    tf.data.experimental.enable_debug_mode()


def get_conv_segment(kernel_size: tuple[int, int], repetitions: int, filters: int):
    conv = tf.keras.layers.Conv2D(
        filters, kernel_size, padding="same", activation="relu")
    pool = tf.keras.layers.MaxPool2D()
    layers = []
    for i in range(repetitions):
        layers.append(conv)
        layers.append(pool)
    return tf.keras.Sequential(layers)


def get_encoder(input_shape: tuple[int, int, int], number_of_segments: int):
    # cnn encoder
    input = tf.keras.layers.Input(input_shape, name="encoder_input")
    x = input
    for i in range(number_of_segments):
        x = get_conv_segment((3, 3), 2, 2**(i+3))(x)
    encoder = tf.keras.Model(input, x, name="encoder")
    return encoder


def get_bbox_head(encoder: tf.keras.Model, num_classes: int, num_anchors: int):
    # bbox head
    input = tf.keras.layers.Input(
        encoder.output_shape[1:], name="bbox_head_input")
    # generate bounding boxes output
    x = input
    x = tf.keras.layers.Flatten()(x)
    x = tf.keras.layers.Dense(64, activation="relu")(x)
    x = tf.keras.layers.Dense(
        num_anchors * (4 + 1 + num_classes), activation="sigmoid")(x)
    bbox_head = tf.keras.Model(input, x, name="bbox_head")

    return bbox_head


def train_detector(encoder: tf.keras.Model, bbox_head: tf.keras.Model, dataset: tf.data.Dataset, epochs: int):
    # train
    optimizer = tf.keras.optimizers.Adam(learning_rate=0.0001)
    loss_fn = tf.keras.losses.MeanSquaredError()
    detector_model = tf.keras.Model(encoder.input, bbox_head(encoder.output))
    detector_model.compile(optimizer=optimizer, loss=loss_fn)
    detector_model.fit(dataset, epochs=epochs)


# Only add this if you want to debug the code. In "release" mode this causes an error
# @tf.function
def load_dataset_from_csv(csv_path: str, image_shape: tuple[int, int, int], max_objects: int, batch_size: int):
    # Only add this if you want to debug the code. In "release" mode this causes an error
    # @tf.function
    def parse_csv_line(line: str):
        line_parts = tf.strings.split(line, sep=',')
        image_path = "../../../../data/valorant2/" + line_parts[0]
        image = tf.io.read_file(image_path)
        image = tf.io.decode_jpeg(image, channels=3)
        resize_ratio = tf.cast(image_shape[:2], tf.float32) / \
            tf.cast(tf.shape(image)[:2], tf.float32)
        image = tf.image.resize(image, image_shape[:2])
        image = tf.cast(image, tf.float32)
        image = image / 255.0

        # parse bounding boxes
        # read list of values
        bbox_xs = tf.strings.split(line_parts[4], sep=';')
        bbox_ys = tf.strings.split(line_parts[5], sep=';')
        bbox_widths = tf.strings.split(line_parts[6], sep=';')
        bbox_heights = tf.strings.split(line_parts[7], sep=';')

        bbox_xs = tf.strings.to_number(bbox_xs, out_type=tf.float32)
        bbox_ys = tf.strings.to_number(bbox_ys, out_type=tf.float32)
        bbox_widths = tf.strings.to_number(bbox_widths, out_type=tf.float32)
        bbox_heights = tf.strings.to_number(bbox_heights, out_type=tf.float32)
        # resize bounding boxes
        bbox_xs = bbox_xs * resize_ratio[1]
        bbox_ys = bbox_ys * resize_ratio[0]
        bbox_widths = bbox_widths * resize_ratio[1]
        bbox_heights = bbox_heights * resize_ratio[0]
        # normalize bounding boxes
        bbox_xs = bbox_xs / image_shape[1]
        bbox_ys = bbox_ys / image_shape[0]
        bbox_widths = bbox_widths / image_shape[1]
        bbox_heights = bbox_heights / image_shape[0]
        # cretae confidence values
        bbox_confidences = tf.ones_like(bbox_xs)
        # create class values
        bbox_classes = tf.zeros_like(bbox_xs)
        # create bounding boxes
        bboxes = tf.stack(
            [bbox_ys, bbox_xs, bbox_heights, bbox_widths, bbox_confidences, bbox_classes], axis=1)
        # pad bounding boxes
        output = tf.pad(
            bboxes, [[0, max_objects - tf.shape(bboxes)[0]], [0, 0]])
        # flatten output
        output = tf.reshape(output, [-1])
        return image, output
    dataset = tf.data.TextLineDataset(csv_path)
    dataset = dataset.skip(1)
    dataset = dataset.map(parse_csv_line)
    dataset = dataset.batch(batch_size)
    return dataset


def inspect_data(dataset: tf.data.Dataset):
    # get first 1 samples
    for image, bbox in dataset.take(1):
        print(image.shape, bbox.shape)
        print(bbox)


def draw_bbox(image: np.ndarray, bbox: tf.Tensor, color: tuple[int, int, int] = (0, 255, 0)):
    bbox = bbox.numpy()
    number_of_boxes = bbox.shape[0] // 6
    for i in range(number_of_boxes):
        y = int(bbox[i*6] * image.shape[0])
        x = int(bbox[i*6+1] * image.shape[1])
        h = int(bbox[i*6+2] * image.shape[0])
        w = int(bbox[i*6+3] * image.shape[1])
        cv2.rectangle(image, (x, y), (x+w, y+h), color, 2)
    return image


def run():
    max_objects = 4
    encoder = get_encoder((144*2, 256*2, 3), 5)
    bbox_head = get_bbox_head(encoder, 1, max_objects)
    encoder.summary()
    bbox_head.summary()
    dataset = load_dataset_from_csv(
        "../../../../data/valorant2/dataset.csv", (144*2, 256*2, 3), max_objects, 32)
    inspect_data(dataset)
    train_detector(encoder, bbox_head, dataset, 100)

    # test the model on the first 10 samples in the dataset
    for image, bbox in dataset.take(1):
        output = bbox_head(encoder(image))
        output_images = []
        # draw the bounding boxes
        for i in range(len(image)):
            meas_image = draw_bbox(image[i].numpy()*255.0, output[i])
            output_images.append(draw_bbox(meas_image, bbox[i], (255, 0, 0)))
        # save the images
        for i in range(len(output_images)):
            cv2.imwrite(
                f"../../../../data/valorant2/bbox_{i}.png", output_images[i])


if __name__ == "__main__":
    run()

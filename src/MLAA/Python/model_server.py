import io
import os
import uvicorn
import numpy as np
import nest_asyncio
import cv2
import tensorflow as tf
from enum import Enum
from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import StreamingResponse


class ModelType(Enum):
    OBJECT_DETECTION = "object_detection"
    IMAGE_CLASSIFICATION = "image_classification"
    IMAGE_SEGMENTATION = "image_segmentation"


class ModelServer():
    def __init__(self, model_type: ModelType, model_path: str, host: str = "", port: int = 8000):
        self.model_type = model_type
        self.model_path = model_path
        self.model = tf.keras.models.load_model(self.model_path)
        self.input_shape = self.model.input_shape
        self.port = port
        if host == "":
            self.host = "0.0.0.0" if os.getenv("DOCKER-SETUP") else "127.0.0.1"
        self.app = FastAPI()
        self._register_routes()

    def _register_routes(self):
        self._register_home_routes()
        if self.model_type == ModelType.OBJECT_DETECTION:
            self._register_object_detection_routes()
        elif self.model_type == ModelType.IMAGE_CLASSIFICATION:
            self._register_image_classification_routes()
        elif self.model_type == ModelType.IMAGE_SEGMENTATION:
            self._register_image_segmentation_routes()
        else:
            raise NotImplementedError(
                f"Model type {self.model_type} is not implemented.")

    def _preprocess_image_input(self, image_bytes):
        image = np.frombuffer(image_bytes, dtype=np.uint8)
        image = cv2.imdecode(image, cv2.IMREAD_COLOR)
        # check image and model input color channels
        model_input_channels = 1 if len(
            self.input_shape) == 2 else self.input_shape[-1]
        image_channels = 1 if len(image.shape) == 2 else image.shape[-1]
        if model_input_channels != image_channels:
            if model_input_channels == 1:
                image = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
            else:
                image = cv2.cvtColor(image, cv2.COLOR_GRAY2BGR)
        else:
            if image_channels == 3:
                image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        image = cv2.resize(image, (self.input_shape[2], self.input_shape[1]))
        image = image / 255.0
        image = image[np.newaxis, ...].astype(np.float32)
        return image

    def _register_object_detection_routes(self):
        @self.app.post("/predict")
        async def predict(file: UploadFile = File(...)):
            # if file.content_type != "image/jpeg":
            #     raise HTTPException(
            #         status_code=415, detail="Unsupported media type.")
            image_bytes = await file.read()
            image = self._preprocess_image_input(image_bytes)
            detections = self.model.predict(image)
            detections = tf.image.non_max_suppression(detections, 0.45, 0.45)
            detections = detections[0]
            detections = detections.numpy()
            detections = detections.tolist()
            return {"detections": detections}

    def _register_image_classification_routes(self):
        @self.app.post("/predict")
        async def predict(file: UploadFile = File(...)):
            # if file.content_type != "image/jpeg":
            #     raise HTTPException(
            #         status_code=415, detail="Unsupported media type.")
            image_bytes = await file.read()
            image = self._preprocess_image_input(image_bytes)
            predictions = self.model.predict(image)
            predictions = tf.nn.softmax(predictions).numpy()
            predictions = predictions.tolist()
            return {"predictions": predictions}

    def _register_image_segmentation_routes(self):
        @self.app.post("/predict")
        async def predict(file: UploadFile = File(...)):
            # if file.content_type != "image/jpeg":
            #     raise HTTPException(
            #         status_code=415, detail="Unsupported media type.")
            image_bytes = await file.read()
            image = self._preprocess_image_input(image_bytes)
            predictions = self.model.predict(image)
            predictions = tf.argmax(predictions, axis=-1)
            predictions = predictions.numpy()
            predictions = predictions[0]
            predictions = predictions.tolist()
            return {"predictions": predictions}

    def _register_home_routes(self):
        @self.app.get("/")
        async def home():
            return "Hello World!"

    def run(self):
        # Allows the server to be run in this interactive environment
        nest_asyncio.apply()

        # Spin up the server!
        uvicorn.run(self.app, host=self.host, port=self.port)

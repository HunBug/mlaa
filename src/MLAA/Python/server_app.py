import model_server

if __name__ == "__main__":
    server = model_server.ModelServer(
        model_server.ModelType.IMAGE_CLASSIFICATION, "model.h5")
    server.run()

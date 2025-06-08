using System;
using System.IO;
using Microsoft.ML;

namespace practica4.MLModels
{
    public class SentimentService
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public SentimentService()
        {
            _mlContext = new MLContext();
            EntrenarModelo();
        }

        /// <summary>
        /// Entrena el modelo de análisis de sentimientos usando el archivo TSV.
        /// </summary>
        private void EntrenarModelo()
        {
            // Ruta al archivo de datos
            var dataPath = Path.Combine("Data", "sentiment-data.tsv");

            // Carga de datos
            var dataView = _mlContext.Data.LoadFromTextFile<SentimentData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: '\t');

            // Construcción del pipeline
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: "Label", featureColumnName: "Features"));

            // Entrena el modelo
            var model = pipeline.Fit(dataView);

            // Crea el motor de predicción
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
        }

        /// <summary>
        /// Realiza una predicción para un texto de opinión.
        /// </summary>
        /// <param name="texto">Opinión a analizar.</param>
        /// <returns>Predicción con etiqueta y score.</returns>
        public SentimentPrediction Predecir(string texto)
        {
            var input = new SentimentData { Text = texto };
            return _predictionEngine.Predict(input);
        }
    }
}


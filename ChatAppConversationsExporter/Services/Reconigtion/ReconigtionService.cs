using Google.Cloud.Speech.V1;
using System;
using System.IO;
using System.Text;
using WindowsFormsApp1.Configurations;
using WindowsFormsApp1.Services.Reconigtion.DataTransferObjects;

namespace WindowsFormsApp1.Services.Reconigtion
{
    public class ReconigtionService
    {
        public RecognitionResponse GetAudioTranscription(string audioFilePath)
        {
            var response = new RecognitionResponse();
            var sb = new StringBuilder();

            try
            {
                byte[] credentialsData = Convert.FromBase64String(new ContextParameters().GoogleCredentials);
                string jsonCredentials = Encoding.UTF8.GetString(credentialsData);

                SpeechClientBuilder builder = new SpeechClientBuilder();
                builder.JsonCredentials = jsonCredentials;

                var speech = builder.Build();

                var audioEncoding = DefineAudioEncoding(Path.GetExtension(audioFilePath));

                var config = new RecognitionConfig
                {
                    Encoding = audioEncoding,
                    SampleRateHertz = 16000,
                    LanguageCode = LanguageCodes.Portuguese.Brazil
                };
                var audio = RecognitionAudio.FromFile(audioFilePath);

                var transcriptResponse = speech.Recognize(config, audio);

                foreach (var result in transcriptResponse.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        sb.AppendLine(alternative.Transcript);
                    }
                }

                response.IsSuccess = true;
                response.Result = sb.ToString();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Result = ex.Message;
            }

            return response;
        }

        private RecognitionConfig.Types.AudioEncoding DefineAudioEncoding(string extesion)
        {
            switch (extesion)
            {
                case ".opus":
                case ".ogg":
                    return RecognitionConfig.Types.AudioEncoding.OggOpus;
                case ".wav":
                case ".mp3":
                    return RecognitionConfig.Types.AudioEncoding.Flac;
            }

            return RecognitionConfig.Types.AudioEncoding.EncodingUnspecified;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services.Reconigtion;

namespace WindowsFormsApp1.Services.Conversation
{
    public class ConversationService
    {
        private List<string> _audioFileExtensions;
        private List<string> _imageFileExtensions;
        private ConversationModel _conversationModel;
        private ReconigtionService _reconigtionService;

        public ConversationService()
        {
            _reconigtionService = new ReconigtionService();
            _conversationModel = new ConversationModel();
            _audioFileExtensions = new List<string>() { ".opus", ".mp3", ".wav", ".ogg" };
            _imageFileExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
        }

        public ConversationModel GetConversation(string conversationFolderPath)
        {
            var sb = new StringBuilder();

            var allFiles = Directory.GetFiles(conversationFolderPath).ToList();
            _conversationModel.TextFilePath = allFiles.FirstOrDefault(c => c.EndsWith("txt"));
            _conversationModel.AudioFilePaths = allFiles.Where(c => _audioFileExtensions.Contains(Path.GetExtension(c))).ToList();
            _conversationModel.ImageFilePaths = allFiles.Where(c => _imageFileExtensions.Contains(Path.GetExtension(c))).ToList();

            if (_conversationModel.TextFilePath != null)
            {
                _conversationModel.ConversationTitle = Path.GetFileNameWithoutExtension(_conversationModel.TextFilePath);                
                
                _conversationModel.Messages = GetMessagesModels(File.ReadAllText(_conversationModel.TextFilePath));

                sb.AppendLine($"CONVERSA: {_conversationModel.ConversationTitle.ToUpper()}");
                sb.AppendLine($"Quantidade de Mensagens: {_conversationModel.Messages.Count}");

                // Handling audio files
                if (_conversationModel.AudioFilePaths.Any())
                {

                    // Verificar se o arquivo de conversa possui as referencias de anexo de midia
                    var attachedAudioFilesCount = _conversationModel.AudioFilePaths.Count;
                    sb.AppendLine($"Número de arquivos de áudios anexados: {attachedAudioFilesCount}");

                    var mentionedAudioFileCount = 0;
                    foreach (var audioFileName in _conversationModel.AudioFilePaths.Select(c => Path.GetFileName(c)).ToList())
                    {
                        if (_conversationModel.Messages.FirstOrDefault(line => line.Text.Contains(audioFileName)) != null)
                            mentionedAudioFileCount++;
                    }

                    sb.AppendLine($"Número de arquivos de áudio mencionados: {mentionedAudioFileCount}");

                    if (mentionedAudioFileCount.Equals(attachedAudioFilesCount))
                    {
                        sb.AppendLine($"Todos arquivos de audio mencionados na conversa foram encontrados");
                    }
                    else
                    {
                        if (mentionedAudioFileCount > attachedAudioFilesCount)
                            sb.AppendLine($"A conversa possui mais arquivos de áudio mencionados do que os anexados");
                        else
                            sb.AppendLine($"Foram anexados mais arquivos de áudio do que a quantidade mencionada na conversa.");
                    }
                }
                else
                {
                    sb.AppendLine($"Quantidade de arquivos áudio: 0");
                }

                // Handling image files
                if (_conversationModel.ImageFilePaths.Any())
                {
                    // Verificar se o arquivo de conversa possui as referencias de anexo de midia
                    var attachedImageFilesCount = _conversationModel.ImageFilePaths.Count;
                    sb.AppendLine($"Número de arquivos de imagem anexados: {attachedImageFilesCount}");

                    var mentionedImageFileCount = 0;
                    foreach (var imageFileName in _conversationModel.ImageFilePaths.Select(c => Path.GetFileName(c)).ToList())
                    {
                        if (_conversationModel.Messages.FirstOrDefault(line => line.Text.Contains(imageFileName)) != null)
                            mentionedImageFileCount++;
                    }

                    sb.AppendLine($"Número de arquivos de imagem mencionados: {mentionedImageFileCount}");

                    if (mentionedImageFileCount.Equals(attachedImageFilesCount))
                    {
                        sb.AppendLine($"Todos arquivos de imagem mencionados na conversa foram encontrados");
                    }
                    else
                    {
                        if (mentionedImageFileCount > attachedImageFilesCount)
                            sb.AppendLine($"A conversa possui mais arquivos de imagem mencionados do que os anexados");
                        else
                            sb.AppendLine($"Foram anexados mais arquivos de imagem do que a quantidade mencionada na conversa.");
                    }
                }
                else
                {
                    sb.AppendLine($"Quantidade de arquivos imagem: 0");
                }

                sb.AppendLine($"FIM DO RESUMO DA CONVERSA {_conversationModel.ConversationTitle.ToUpper()}");

                _conversationModel.ImportReport = sb.ToString();

                return _conversationModel;
            }

            return null;
        }

        public string GetChatHtml(ConversationModel conversation)
        {
            var sb = new StringBuilder();

            sb.Append($"<p class=\"conversation-title\">Início da Conversa: {conversation.ConversationTitle}</p>");

            foreach (var message in conversation.Messages)
            {
                if (message.IsAudioTranscription)
                {
                    sb.Append($"<p><b>{message.Timestamp} {message.Author}:</b>&nbsp;&nbsp;<span style=\"font-size:smaller;font-style:italic\">(Transcrição de áudio)</span><br/><span style=\"font-style:italic;padding-left:15px\">{message.Text}</span></p>");
                }
                else if (message.IsImage)
                {                    
                    sb.Append($"<p><b>{message.Timestamp} {message.Author}:</b><br/><img src=\"{message.Text}\" class=\"conversation-image\"/></p>");
                }
                else
                {
                    sb.Append($"<p><b>{message.Timestamp} {message.Author}:</b><br/><span style=\"padding-left:15px\">{message.Text}</span></p>");
                }
            }

            sb.Append($"<hr>");

            return sb.ToString();
        }

        private List<MessageModel> GetMessagesModels(string conversationContent)
        {
            var response = new List<MessageModel>();

            // Split content by timestamp pattern
            var timestampRegex = new Regex("([0-9]{2}[/][0-9]{2}[/][0-9]{4}[ ][0-9]{2}[:][0-9]{2})");
            var allMessages = timestampRegex.Split(conversationContent).ToList();

            // Get only messages are nou null or empty
            var usefullMessages = allMessages.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            // usefullMessages is an array that contains: "timestamp1", "Text1", "timestamp2", "Text2", "timestampN", "TextN", ...
            for (int i = 0; i < usefullMessages.Count(); i += 2)
            {
                var timeStamp = usefullMessages[i];
                var text = usefullMessages[i + 1].Replace(" - ", string.Empty);

                var model = new MessageModel();

                try
                {
                    model.Timestamp = timeStamp;

                    var endNamePosition = text.IndexOf(":");

                    if (endNamePosition != -1)
                    {
                        model.Author = text.Substring(0, endNamePosition);
                    }
                    else
                    {
                        model.Author = "Aplicativo";
                    }

                    var audioFileName = GetAudioFile(text);

                    if (!string.IsNullOrEmpty(audioFileName))
                    {
                        model.IsAudioTranscription = true;
                        var audioFilePath = _conversationModel.AudioFilePaths.FirstOrDefault(c => Path.GetFileName(c).Equals(audioFileName));

                        var audioRecognitionResponse = _reconigtionService.GetAudioTranscription(audioFilePath);

                        if (audioRecognitionResponse.IsSuccess)
                        {
                            model.Text = audioRecognitionResponse.Result;
                        }
                        else
                        {
                            model.Text = $"Não foi possível transcrever o aúdio {text}. Motivo: {audioRecognitionResponse.Result}";
                        }
                    }
                    else
                    {
                        model.IsAudioTranscription = false;

                        var imageFileName = GetImageFile(text);

                        if (!string.IsNullOrEmpty(imageFileName))
                        {
                            model.IsImage = true;
                            var imageFilePath = _conversationModel.ImageFilePaths.FirstOrDefault(c => Path.GetFileName(c).Equals(imageFileName));
                            var imageRecognitionResponse = _reconigtionService.GetImageBase64(imageFilePath);

                            if (imageRecognitionResponse.IsSuccess)
                            {
                                model.Text = imageRecognitionResponse.Result;
                            }
                            else
                            {
                                model.IsImage = false;
                                model.Text = $"Não foi possível converter a image {text}. Motivo: {imageRecognitionResponse.Result}";
                            }
                        }
                        else
                        {
                            model.IsImage = false;
                            var startIndex = endNamePosition == -1 ? 0 : endNamePosition + 2;
                            model.Text = text.Substring(startIndex, text.Length - startIndex);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro");
                }

                response.Add(model);

            }

            return response;
        }

        private string GetImageFile(string text)
        {
            if (_conversationModel.ImageFilePaths != null && _conversationModel.ImageFilePaths.Any())
            {
                var fileNames = _conversationModel.ImageFilePaths.Select(c => Path.GetFileName(c));

                return fileNames.FirstOrDefault(c => text.Contains(c));

            }

            return null;
        }

        private string GetAudioFile(string messageString)
        {
            // TODO: implement (disabled due transciption library limitations)
            return null;
        }
    }
}

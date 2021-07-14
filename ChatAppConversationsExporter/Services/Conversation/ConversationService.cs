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
        private List<string> audioFileExtensions;

        public ConversationService()
        {
            audioFileExtensions = new List<string>() { ".opus", ".mp3", ".wav", ".ogg" };
        }

        public ConversationModel GetConversation(string conversationFolderPath)
        {
            var sb = new StringBuilder();

            var allFiles = Directory.GetFiles(conversationFolderPath).ToList();
            var textFilePath = allFiles.FirstOrDefault(c => c.EndsWith("txt"));
            var audioFilePaths = allFiles.Where(c => audioFileExtensions.Contains(Path.GetExtension(c))).ToList();

            if (textFilePath != null)
            {
                var conversationModel = new ConversationModel();
                conversationModel.ConversationTitle = Path.GetFileNameWithoutExtension(textFilePath);
                conversationModel.TextFilePath = textFilePath;
                conversationModel.Messages = GetConversationModels(File.ReadAllText(textFilePath));

                sb.AppendLine($"CONVERSA: {conversationModel.ConversationTitle.ToUpper()}");
                sb.AppendLine($"Quantidade de Mensagens: {conversationModel.Messages.Count}");

                if (audioFilePaths.Any())
                {
                    conversationModel.AudioFilePaths = audioFilePaths;

                    // Verificar se o arquivo de conversa possui as referencias de anexo de midia
                    var attachedAudioFilesCount = audioFilePaths.Count;
                    sb.AppendLine($"Número de arquivos de áudios anexados: {attachedAudioFilesCount}");

                    var mentionedAudioFileCount = 0;
                    foreach (var audioFileName in audioFilePaths.Select(c => Path.GetFileName(c)).ToList())
                    {
                        if (conversationModel.Messages.FirstOrDefault(line => line.Text.Contains(audioFileName)) != null)
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

                sb.AppendLine($"FIM DO RESUMO DA CONVERSA {conversationModel.ConversationTitle.ToUpper()}");

                conversationModel.ImportReport = sb.ToString();

                return conversationModel;
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
                else
                {
                    sb.Append($"<p><b>{message.Timestamp} {message.Author}:</b><br/><span style=\"padding-left:15px\">{message.Text}</span></p>");
                }
            }

            sb.Append($"<hr>");

            return sb.ToString();
        }

        private List<MessageModel> GetConversationModels(string conversationContent)
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

                    var audioFilePath = GetAudioFile(text);

                    if (string.IsNullOrEmpty(audioFilePath))
                    {
                        model.IsAudioTranscription = false;

                        var startIndex = endNamePosition == -1 ? 0 : endNamePosition + 2;
                        model.Text = text.Substring(startIndex, text.Length - startIndex);
                    }
                    else
                    {
                        model.IsAudioTranscription = false;
                        var reconigtionService = new ReconigtionService();

                        var recognitionResponse = reconigtionService.GetAudioTranscription(audioFilePath);

                        if (recognitionResponse.IsSuccess)
                        {
                            model.Text = recognitionResponse.Result;
                        }
                        else
                        {
                            model.Text = $"Não foi possível transcrever o aúdio. Motivo: {recognitionResponse.Result}";
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

        private string GetAudioFile(string messageString)
        {
            // TODO: implement
            return null;
        }
    }
}

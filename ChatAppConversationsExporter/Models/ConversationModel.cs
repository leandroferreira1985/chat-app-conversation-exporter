using System.Collections.Generic;

namespace WindowsFormsApp1.Models
{
    public class ConversationModel
    {
        public string ConversationTitle { get; set; }
        public string TextFilePath { get; set; }
        public List<string> AudioFilePaths { get; set; }
        public List<MessageModel> Messages { get; set; }

        public string ImportReport { get; set; }
    }
}

namespace WindowsFormsApp1.Models
{
    public class MessageModel
    {
        public string Author { get; set; }
        public string Timestamp { get; set; }
        public string Text { get; set; }
        public bool IsAudioTranscription { get; set; }
        public bool IsImage { get; set; }
    }
}

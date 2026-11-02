namespace blazornacho.Models
{
    public class ChatMessage
    {
        public string Texto { get; set; } = "";
        public string Tipo { get; set; } = "bot";
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
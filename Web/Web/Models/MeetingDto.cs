namespace Web.Models
{
    public class MeetingDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Link { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

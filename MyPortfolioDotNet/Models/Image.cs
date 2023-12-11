namespace MyPortfolioDotNet.Models
{
    public class Image
    {
        public int  Id { get; set; }
        public string ImageUrl { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

    }
}

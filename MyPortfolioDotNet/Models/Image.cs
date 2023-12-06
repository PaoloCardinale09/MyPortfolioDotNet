using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolioDotNet.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
       
        public string? ScreenshotPath { get; set; }

        // Chiave esterna per collegarsi al progetto
        public int ProjectId { get; set; }
        public Project Project { get; set; }

    }
}

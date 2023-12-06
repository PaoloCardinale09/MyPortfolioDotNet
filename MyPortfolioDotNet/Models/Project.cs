using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyPortfolioDotNet.Models
{
    public class Project
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Technology { get; set; }
        
        public string Link { get; set; }
        [NotMapped]
        public IFormFile UploadFile { get; set; }
        public List<Image>? Images { get; set; }




    }
}

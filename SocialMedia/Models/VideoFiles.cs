using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Models
{
    public class VideoFiles
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile VideoFile { get; set; }
        public int AccountID { get; set; }
    }
}

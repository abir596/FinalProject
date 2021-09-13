using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }       
        public Account Sender { get; set; }
        public Account Receiver { get; set; }
    }
}

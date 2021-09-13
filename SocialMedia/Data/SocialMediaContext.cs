using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Models;

namespace SocialMedia.Data
{
    public class SocialMediaContext : DbContext
    {
        public SocialMediaContext (DbContextOptions<SocialMediaContext> options)
            : base(options)
        {
        }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<VideoFiles> VideoFiles { get; set; }
    }
}

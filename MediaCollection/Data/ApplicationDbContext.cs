using System;
using System.Collections.Generic;
using System.Text;
using MediaCollection.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediaCollection.Data
{
    public class ApplicationDbContext : IdentityDbContext<MediaCollectionUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        DbSet<Band> Bands { get; set; }
        DbSet<Album> Albums { get; set; }
        DbSet<Song> Songs { get; set; }
    }
}

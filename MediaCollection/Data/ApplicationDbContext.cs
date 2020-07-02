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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayListSong>()
                .HasKey(et => new { et.PlayListId, et.SongId });

            builder.Entity<PlayListSong>()
                .HasOne(et => et.PlayList)
                .WithMany(e => e.PlayListSongs)
                .HasForeignKey(et => et.PlayListId);

            builder.Entity<PlayListSong>()
                .HasOne(et => et.Song)
                .WithMany(e => e.PlayListSongs)
                .HasForeignKey(et => et.SongId);
        }

        public DbSet<Band> Bands { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlayList> PlayLists { get; set; }
        public DbSet<PlayListSong> PlayListSongs { get; set; }
        public DbSet<SongReview> SongReviews { get; set; }
        public DbSet<AlbumReview> AlbumReviews { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<PodcastEpisode> PodcastEpisodes { get; set; }
        public DbSet<PodcastEpisodeReview> PodcastEpisodeReviews { get; set; }
    }
}

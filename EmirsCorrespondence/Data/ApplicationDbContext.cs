using DreyCorrespondence.Models;
using EmirsCorrespondence.Models;
using JWTRoleBasedAuthorization.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace EmirsCorrespondence.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<User>().HasOne(x => x.Role);


        //}

        // DbSets
        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentShareLink> DocumentShareLinks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<DocumentLog> DocumentLogs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var adminRole = new Role { RoleId = 1, RoleName = "Admin" };
            var adminUser = new Users
            {
                UserId = 1,
                UserName = "admin",
                Email = "admin@corp.com",
                RoleId = adminRole.RoleId,
                IsActive = true,
                PasswordHash = "Dahmylarey" // Hash this properly
            };

            modelBuilder.Entity<Role>().HasData(adminRole);
            modelBuilder.Entity<Users>().HasData(adminUser);

            // User-Role Relationship
            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Self-referencing Relationship for Messages
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Message-Attachment Relationship
            modelBuilder.Entity<Attachment>()
           .HasOne(a => a.Message)
           .WithMany(m => m.Attachments)
           .HasForeignKey(a => a.MessageId);

            // User-Document Relationship
            modelBuilder.Entity<Document>()
         .HasOne(d => d.UploadedBy)
         .WithMany(u => u.Documents)
         .HasForeignKey(d => d.UploadedById); // Use the new property as the foreign key

            // User-Document Relationship
            modelBuilder.Entity<DocumentVersion>()
            .HasOne(dv => dv.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(dv => dv.DocumentId);

            // User-Notification Relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // User-Log Relationship
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId);




            modelBuilder.Entity<ContentType>().HasNoKey();


        }



    }
}

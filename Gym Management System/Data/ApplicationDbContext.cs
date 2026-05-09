using Gym_Management_System.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Gym_Management_System.Data
{
    public class ApplicationDbContext:IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }


        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<TrainerProfile> TrainerProfiles { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 

  
            builder.Entity<GymClass>().Property(e => e.Status).HasConversion<string>();
            builder.Entity<GymClass>().Property(e => e.Type).HasConversion<string>();
            builder.Entity<Booking>().Property(e => e.Status).HasConversion<string>();
            builder.Entity<Subscription>().Property(e => e.Plan).HasConversion<string>();
            builder.Entity<Payment>().Property(e => e.Status).HasConversion<string>();
            builder.Entity<Payment>().Property(e => e.Type).HasConversion<string>();
            builder.Entity<TrainerAvailability>().Property(e => e.DayOfWeek).HasConversion<string>();


            builder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

            builder.Entity<PromoCode>()
              .Property(p => p.DiscountPercentage)
                .HasPrecision(5, 2);

            builder.Entity<GymClass>()
                .HasOne(g => g.Trainer)
                .WithMany()
                .HasForeignKey(g => g.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Booking>()
                .HasOne(b => b.GymClass)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GymClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Trainer)
                .WithMany()
                .HasForeignKey(r => r.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerProfile>()
                .HasOne(tp => tp.User)
                .WithMany()
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerAvailability>()
                .HasOne(ta => ta.Trainer)
                .WithMany()
                .HasForeignKey(ta => ta.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TasksScaffold.Models;

namespace TasksScaffold.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<SimpleTask> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UndoRedoHistory> UndoRedoHistories { get; set; }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("test_db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SimpleTask>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tasks)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);
        
        
        modelBuilder.Entity<UndoRedoHistory>(entity =>
        {
            entity.HasKey(urh => urh.Id);

            entity.Property(urh => urh.UndoStack)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.None).ToList()
                );
                
            entity.Property(urh => urh.RedoStack)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.None).ToList()
                );
        });
    }
}
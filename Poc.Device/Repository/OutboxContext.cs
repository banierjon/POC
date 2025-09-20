using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Poc.Device.Repository;

public class OutboxContext : DbContext
{
    public OutboxContext(DbContextOptions<OutboxContext> options) : base(options)
    {
    }
    
    public DbSet<OutboxItem> OutboxItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Level).IsRequired();
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Sent).IsRequired();
            entity.Property(e => e.Sign).IsRequired();
        });
    }
}
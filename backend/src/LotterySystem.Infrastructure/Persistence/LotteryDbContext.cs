using System.Linq.Expressions;
using LotterySystem.Domain.Common;
using LotterySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LotterySystem.Infrastructure.Persistence;

public sealed class LotteryDbContext(DbContextOptions<LotteryDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<LotteryRound> LotteryRounds => Set<LotteryRound>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketItem> TicketItems => Set<TicketItem>();
    public DbSet<LotteryResult> LotteryResults => Set<LotteryResult>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<LotteryRound>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<LotteryRound>().Property(x => x.DrawDate).HasConversion<DateOnlyConverter>();

        modelBuilder.Entity<Ticket>().HasMany(x => x.Items).WithOne(x => x.Ticket).HasForeignKey(x => x.TicketId);
        modelBuilder.Entity<Ticket>().HasMany(x => x.PaymentTransactions).WithOne(x => x.Ticket).HasForeignKey(x => x.TicketId);

        modelBuilder.Entity<Ticket>().HasIndex(x => new { x.CustomerId, x.CreatedAtUtc });
        modelBuilder.Entity<Ticket>().HasIndex(x => new { x.LotteryRoundId, x.CreatedAtUtc });
        modelBuilder.Entity<TicketItem>().HasIndex(x => x.Number);
        modelBuilder.Entity<LotteryResult>().HasIndex(x => new { x.LotteryRoundId, x.WinningNumber });
        modelBuilder.Entity<PaymentTransaction>().HasIndex(x => new { x.Status, x.PaidAtUtc });
        modelBuilder.Entity<AuditLog>().HasIndex(x => new { x.EntityName, x.CreatedAtUtc });

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(BuildIsDeletedFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression BuildIsDeletedFilter(Type clrType)
    {
        var parameter = Expression.Parameter(clrType, "e");
        var prop = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var compare = Expression.Equal(prop, Expression.Constant(false));
        return Expression.Lambda(compare, parameter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAtUtc = DateTime.UtcNow;
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

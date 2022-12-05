using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


class TransactionDbContextFactory : IDesignTimeDbContextFactory<TransactionDBContext>
{
    public TransactionDBContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TransactionDBContext>();

        var connStr = "Host=localhost:5432;Database=transactions;Username=postgres;Password=example";
        builder.UseNpgsql(connStr);
        return new TransactionDBContext(builder.Options);
    }
}

public class TransactionDBContext : DbContext
{
    public DbSet<TransactionDefinition>? Definitions { get; set; }
    public DbSet<Transaction>? Transactions { get; set; }

    public TransactionDBContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionDefinition>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<TransactionDefinition>()
            .HasAlternateKey(c => new { c.OrderUrlTemplate, c.Client })
            .HasName("Unique_OrderUrlTemplate");

        modelBuilder.Entity<DataChecker>()
            .HasKey(e => new { e.TransactionDefinitionId, e.RequestDataPath });

        modelBuilder.Entity<Transaction>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<TransactionLog>()
            .HasKey(e => e.Id);


        var EftId = Guid.NewGuid();

        modelBuilder.Entity<TransactionDefinition>().HasData(new
        {
            Id = EftId,
            RequestUrlTemplate = "/transfers/eft/simulate",
            OrderUrlTemplate = "/transfers/eft/execute",
            Client = "Web",
            Workflow = "transaction-transfer-over-web",
            TTL = 600,
            SignalRHub = "hub-transaction-transfer-over-web"
        });

        modelBuilder.Entity<DataChecker>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.amount.value",
            OrderDataPath = "$.amount.value",
            Type = DataChecker.ComparerType.Number
        });

        modelBuilder.Entity<DataChecker>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.target.iban",
            OrderDataPath = "$.target.iban",
            Type = DataChecker.ComparerType.String
        });

         modelBuilder.Entity<DataChecker>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.target.name",
            OrderDataPath = "$.target.name",
            Type = DataChecker.ComparerType.String
        });
    }
}

public class TransactionDefinition
{
    public Guid Id { get; set; }

    public string RequestUrlTemplate { get; set; } = string.Empty;
    public string OrderUrlTemplate { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;

    public string Workflow { get; set; } = string.Empty;
    public int TTL { get; set; } = 300;
    public string SignalRHub { get; set; } = string.Empty;

    public List<DataChecker>? Checkers { get; set; }

}


public class DataChecker
{
    public Guid TransactionDefinitionId { get; set; }
    public TransactionDefinition? TransactionDefinition { get; set; }

    public string RequestDataPath { get; set; } = string.Empty;

    public string OrderDataPath { get; set; } = string.Empty;
    public ComparerType Type { get; set; }

    public enum ComparerType
    {
        String,
        Number,
        Boolean
    }
}


public class Transaction
{
    public Guid Id { get; set; }

    public Guid TransactionDefinitionId { get; set; }
    public TransactionDefinition? TransactionDefinition { get; set; }

    public Guid WorkflowId { get; set; }
    public string Workflow { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string StatusReason { get; set; } = string.Empty;

    public string RequestUpstreamResponse { get; set; } = string.Empty;
    public string RequestRouteResponse { get; set; } = string.Empty;

    public string OrderUpstreamResponse { get; set; } = string.Empty;
    public string OrderRouteResponse { get; set; } = string.Empty;

    public string? SignalRHubToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}


public class TransactionLog
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string StatusReason { get; set; } = string.Empty;

    public string Log { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}




﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

class TransactionDbContextFactory : IDesignTimeDbContextFactory<TransactionDBContext>
{
    public TransactionDBContext CreateDbContext(string[] args)
    {
        var path = Directory.GetCurrentDirectory();
            
        var builder = new DbContextOptionsBuilder<TransactionDBContext>();
        var connStr = "Host=localhost:5432;Database=transactions;Username=postgres;Password=postgres";
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

        modelBuilder.Entity<DataValidator>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Transaction>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<TransactionLog>()
            .HasKey(e => e.Id);


        var EftId = Guid.NewGuid();

        modelBuilder.Entity<TransactionDefinition>().HasData(new
        {
            Id = EftId,
            RequestUrlMethod = TransactionDefinition.MethodType.POST,
            RequestUrlTemplate = "/transfers/eft/simulate",
            OrderUrlMethod = TransactionDefinition.MethodType.POST,
            OrderUrlTemplate = "/transfers/eft/execute",
            Client = "Web",
            Workflow = "transaction-transfer-eft-over-web",
            TTL = 600
        });

        modelBuilder.Entity<DataValidator>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.amount.value",
            OrderDataPath = "$.amount.value",
            Type = DataValidator.ComparerType.Number
        });

        modelBuilder.Entity<DataValidator>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.target.iban",
            OrderDataPath = "$.target.iban",
            Type = DataValidator.ComparerType.String
        });

        modelBuilder.Entity<DataValidator>().HasData(new
        {
            Id = Guid.NewGuid(),
            TransactionDefinitionId = EftId,
            RequestDataPath = "$.target.name",
            OrderDataPath = "$.target.name",
            Type = DataValidator.ComparerType.String
        });
    }
}

public class TransactionDefinition
{
    public Guid Id { get; set; }

    public MethodType RequestUrlMethod { get; set; } = MethodType.POST;
    public string RequestUrlTemplate { get; set; } = string.Empty;
    public MethodType OrderUrlMethod { get; set; } = MethodType.POST;
    public string OrderUrlTemplate { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;

    public string Workflow { get; set; } = string.Empty;
    public int TTL { get; set; } = 300;

    public List<DataValidator>? Validators { get; set; }

    public enum MethodType { POST, GET }
}


public class DataValidator
{
    public Guid Id { get; set; }

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

    public string RequestUpStreamUrl{get;set;} = string.Empty;
    public string RequestUpstreamResponse { get; set; } = string.Empty;
    public string RequestUpstreamBody { get; set; } = string.Empty;
    public string RequestRouteResponse { get; set; } = string.Empty;

    public MethodType OrderUpstreamType{get;set;} = MethodType.POST;
    public string OrderUpStreamUrl{get;set;} = string.Empty;
    public string OrderUpstreamResponse { get; set; } = string.Empty;
    public string OrderUpstreamBody { get; set; } = string.Empty;
    public string OrderRouteResponse { get; set; } = string.Empty;

    public string? SignalRHubToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public enum MethodType { POST, GET }
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




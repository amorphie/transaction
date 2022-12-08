﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace amorphie.transaction.data.Migrations
{
    [DbContext(typeof(TransactionDBContext))]
    partial class TransactionDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataValidator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("OrderDataPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestDataPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("TransactionDefinitionId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TransactionDefinitionId");

                    b.ToTable("DataValidator");

                    b.HasData(
                        new
                        {
                            Id = new Guid("e145fba6-455a-4c5f-b5a6-37ec9a09c542"),
                            OrderDataPath = "$.amount.value",
                            RequestDataPath = "$.amount.value",
                            TransactionDefinitionId = new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"),
                            Type = 1
                        },
                        new
                        {
                            Id = new Guid("de2881e4-0b03-4410-b9f7-1cfaf7514e66"),
                            OrderDataPath = "$.target.iban",
                            RequestDataPath = "$.target.iban",
                            TransactionDefinitionId = new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"),
                            Type = 0
                        },
                        new
                        {
                            Id = new Guid("7ed0628a-d400-42d5-b286-47865993ebc0"),
                            OrderDataPath = "$.target.name",
                            RequestDataPath = "$.target.name",
                            TransactionDefinitionId = new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"),
                            Type = 0
                        });
                });

            modelBuilder.Entity("Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OrderRouteResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OrderUpstreamResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestRouteResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestUpstreamResponse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SignalRHubToken")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StatusReason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("TransactionDefinitionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Workflow")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("WorkflowId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TransactionDefinitionId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("TransactionDefinition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Client")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OrderUrlTemplate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RequestUrlTemplate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SignalRHub")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TTL")
                        .HasColumnType("integer");

                    b.Property<string>("Workflow")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasAlternateKey("OrderUrlTemplate", "Client")
                        .HasName("Unique_OrderUrlTemplate");

                    b.ToTable("Definitions");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"),
                            Client = "Web",
                            OrderUrlTemplate = "/transfers/eft/execute",
                            RequestUrlTemplate = "/transfers/eft/simulate",
                            SignalRHub = "hub-transaction-transfer-eft-over-web",
                            TTL = 600,
                            Workflow = "transaction-transfer-eft-over-web"
                        });
                });

            modelBuilder.Entity("TransactionLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FromStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StatusReason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ToStatus")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionLog");
                });

            modelBuilder.Entity("DataValidator", b =>
                {
                    b.HasOne("TransactionDefinition", "TransactionDefinition")
                        .WithMany("Validators")
                        .HasForeignKey("TransactionDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransactionDefinition");
                });

            modelBuilder.Entity("Transaction", b =>
                {
                    b.HasOne("TransactionDefinition", "TransactionDefinition")
                        .WithMany()
                        .HasForeignKey("TransactionDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransactionDefinition");
                });

            modelBuilder.Entity("TransactionLog", b =>
                {
                    b.HasOne("Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("TransactionDefinition", b =>
                {
                    b.Navigation("Validators");
                });
#pragma warning restore 612, 618
        }
    }
}

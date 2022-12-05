﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using amorphie.transaction.data;

#nullable disable

namespace amorphie.transaction.data.Migrations
{
    [DbContext(typeof(TransactionDBContext))]
    [Migration("20221205175607_setup2")]
    partial class setup2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("amorphie.transaction.data.DataChecker", b =>
                {
                    b.Property<Guid>("TransactionDefinitionId")
                        .HasColumnType("uuid");

                    b.Property<string>("RequestDataPath")
                        .HasColumnType("text");

                    b.Property<string>("OrderDataPath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("TransactionDefinitionId", "RequestDataPath");

                    b.ToTable("DataChecker");
                });

            modelBuilder.Entity("amorphie.transaction.data.Transaction", b =>
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

            modelBuilder.Entity("amorphie.transaction.data.TransactionDefinition", b =>
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
                });

            modelBuilder.Entity("amorphie.transaction.data.TransactionLog", b =>
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

            modelBuilder.Entity("amorphie.transaction.data.DataChecker", b =>
                {
                    b.HasOne("amorphie.transaction.data.TransactionDefinition", "TransactionDefinition")
                        .WithMany("Checkers")
                        .HasForeignKey("TransactionDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransactionDefinition");
                });

            modelBuilder.Entity("amorphie.transaction.data.Transaction", b =>
                {
                    b.HasOne("amorphie.transaction.data.TransactionDefinition", "TransactionDefinition")
                        .WithMany()
                        .HasForeignKey("TransactionDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TransactionDefinition");
                });

            modelBuilder.Entity("amorphie.transaction.data.TransactionLog", b =>
                {
                    b.HasOne("amorphie.transaction.data.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("amorphie.transaction.data.TransactionDefinition", b =>
                {
                    b.Navigation("Checkers");
                });
#pragma warning restore 612, 618
        }
    }
}

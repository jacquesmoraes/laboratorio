﻿// <auto-generated />
using System;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infra.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20250606031334_OptionalTablePriceInClient")]
    partial class OptionalTablePriceInClient
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Models.Billing.BillingInvoice", b =>
                {
                    b.Property<int>("BillingInvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BillingInvoiceId"));

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("PreviousCredit")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PreviousDebit")
                        .HasColumnType("numeric");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalServiceOrdersAmount")
                        .HasColumnType("numeric");

                    b.HasKey("BillingInvoiceId");

                    b.HasIndex("ClientId");

                    b.ToTable("BillingInvoice");
                });

            modelBuilder.Entity("Core.Models.Clients.Client", b =>
                {
                    b.Property<int>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ClientId"));

                    b.Property<int>("BillingMode")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ClientCpf")
                        .HasColumnType("text");

                    b.Property<string>("ClientEmail")
                        .HasColumnType("text");

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClientPhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Cnpj")
                        .HasColumnType("text");

                    b.Property<string>("Cro")
                        .HasColumnType("text");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<int?>("TablePriceId")
                        .HasColumnType("integer");

                    b.HasKey("ClientId");

                    b.HasIndex("TablePriceId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Core.Models.Payments.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AmountPaid")
                        .HasColumnType("numeric");

                    b.Property<int?>("BillingInvoiceId")
                        .HasColumnType("integer");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BillingInvoiceId");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientPayments");
                });

            modelBuilder.Entity("Core.Models.Pricing.TablePrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("TablePrices");
                });

            modelBuilder.Entity("Core.Models.Pricing.TablePriceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int?>("TablePriceId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TablePriceId");

                    b.HasIndex("WorkTypeId");

                    b.ToTable("TablePriceItems");
                });

            modelBuilder.Entity("Core.Models.Production.Scale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Scales");
                });

            modelBuilder.Entity("Core.Models.Production.Shade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ScaleId")
                        .HasColumnType("integer");

                    b.Property<string>("color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ScaleId");

                    b.ToTable("Shades");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.ProductionStage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateOut")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("SectorId")
                        .HasColumnType("integer");

                    b.Property<int>("ServiceOrderId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SectorId");

                    b.HasIndex("ServiceOrderId");

                    b.ToTable("ProductionStages");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.Sector", b =>
                {
                    b.Property<int>("SectorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SectorId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("SectorId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.ServiceOrder", b =>
                {
                    b.Property<int>("ServiceOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServiceOrderId"));

                    b.Property<int?>("BillingInvoiceId")
                        .HasColumnType("integer");

                    b.Property<int>("ClientId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateIn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateOut")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateOutFinal")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("OrderTotal")
                        .HasColumnType("numeric");

                    b.Property<string>("PatientName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("ServiceOrderId");

                    b.HasIndex("BillingInvoiceId");

                    b.HasIndex("ClientId");

                    b.ToTable("ServiceOrders");
                });

            modelBuilder.Entity("Core.Models.Works.Work", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Notes")
                        .HasColumnType("text");

                    b.Property<decimal>("PriceUnit")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<int?>("ScaleId")
                        .HasColumnType("integer");

                    b.Property<int>("ServiceOrderId")
                        .HasColumnType("integer");

                    b.Property<int?>("ShadeId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScaleId");

                    b.HasIndex("ServiceOrderId");

                    b.HasIndex("ShadeId");

                    b.HasIndex("WorkTypeId");

                    b.ToTable("Works");
                });

            modelBuilder.Entity("Core.Models.Works.WorkSection", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WorkSections");
                });

            modelBuilder.Entity("Core.Models.Works.WorkType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WorkSectionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WorkSectionId");

                    b.ToTable("WorkTypes");
                });

            modelBuilder.Entity("Core.Models.Billing.BillingInvoice", b =>
                {
                    b.HasOne("Core.Models.Clients.Client", "Client")
                        .WithMany("BillingInvoices")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Core.Models.Clients.Client", b =>
                {
                    b.HasOne("Core.Models.Pricing.TablePrice", "TablePrice")
                        .WithMany("Clients")
                        .HasForeignKey("TablePriceId");

                    b.OwnsOne("Core.Models.Clients.Address", "Address", b1 =>
                        {
                            b1.Property<int>("ClientId")
                                .HasColumnType("integer");

                            b1.Property<string>("Cep")
                                .HasColumnType("text");

                            b1.Property<string>("City")
                                .HasColumnType("text");

                            b1.Property<string>("Complement")
                                .HasColumnType("text");

                            b1.Property<string>("Neighborhood")
                                .HasColumnType("text");

                            b1.Property<int?>("Number")
                                .HasColumnType("integer");

                            b1.Property<string>("Street")
                                .HasColumnType("text");

                            b1.HasKey("ClientId");

                            b1.ToTable("Clients");

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.OwnsMany("Core.Models.Clients.Patient", "Patients", b1 =>
                        {
                            b1.Property<int>("ClientId")
                                .HasColumnType("integer");

                            b1.Property<int>("PatientInternalId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("PatientInternalId"));

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Notes")
                                .HasColumnType("text");

                            b1.HasKey("ClientId", "PatientInternalId");

                            b1.ToTable("Patients", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ClientId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Patients");

                    b.Navigation("TablePrice");
                });

            modelBuilder.Entity("Core.Models.Payments.Payment", b =>
                {
                    b.HasOne("Core.Models.Billing.BillingInvoice", "BillingInvoice")
                        .WithMany("Payments")
                        .HasForeignKey("BillingInvoiceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Models.Clients.Client", "Client")
                        .WithMany("Payments")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BillingInvoice");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Core.Models.Pricing.TablePriceItem", b =>
                {
                    b.HasOne("Core.Models.Pricing.TablePrice", "TablePrice")
                        .WithMany("Items")
                        .HasForeignKey("TablePriceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Models.Works.WorkType", "WorkType")
                        .WithMany()
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TablePrice");

                    b.Navigation("WorkType");
                });

            modelBuilder.Entity("Core.Models.Production.Shade", b =>
                {
                    b.HasOne("Core.Models.Production.Scale", "Scale")
                        .WithMany("Colors")
                        .HasForeignKey("ScaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scale");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.ProductionStage", b =>
                {
                    b.HasOne("Core.Models.ServiceOrders.Sector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Models.ServiceOrders.ServiceOrder", "ServiceOrder")
                        .WithMany("Stages")
                        .HasForeignKey("ServiceOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sector");

                    b.Navigation("ServiceOrder");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.ServiceOrder", b =>
                {
                    b.HasOne("Core.Models.Billing.BillingInvoice", "BillingInvoice")
                        .WithMany("ServiceOrders")
                        .HasForeignKey("BillingInvoiceId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Core.Models.Clients.Client", "Client")
                        .WithMany("ServiceOrders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BillingInvoice");

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Core.Models.Works.Work", b =>
                {
                    b.HasOne("Core.Models.Production.Scale", "Scale")
                        .WithMany()
                        .HasForeignKey("ScaleId");

                    b.HasOne("Core.Models.ServiceOrders.ServiceOrder", "ServiceOrder")
                        .WithMany("Works")
                        .HasForeignKey("ServiceOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Models.Production.Shade", "Shade")
                        .WithMany()
                        .HasForeignKey("ShadeId");

                    b.HasOne("Core.Models.Works.WorkType", "WorkType")
                        .WithMany()
                        .HasForeignKey("WorkTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scale");

                    b.Navigation("ServiceOrder");

                    b.Navigation("Shade");

                    b.Navigation("WorkType");
                });

            modelBuilder.Entity("Core.Models.Works.WorkType", b =>
                {
                    b.HasOne("Core.Models.Works.WorkSection", "WorkSection")
                        .WithMany()
                        .HasForeignKey("WorkSectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkSection");
                });

            modelBuilder.Entity("Core.Models.Billing.BillingInvoice", b =>
                {
                    b.Navigation("Payments");

                    b.Navigation("ServiceOrders");
                });

            modelBuilder.Entity("Core.Models.Clients.Client", b =>
                {
                    b.Navigation("BillingInvoices");

                    b.Navigation("Payments");

                    b.Navigation("ServiceOrders");
                });

            modelBuilder.Entity("Core.Models.Pricing.TablePrice", b =>
                {
                    b.Navigation("Clients");

                    b.Navigation("Items");
                });

            modelBuilder.Entity("Core.Models.Production.Scale", b =>
                {
                    b.Navigation("Colors");
                });

            modelBuilder.Entity("Core.Models.ServiceOrders.ServiceOrder", b =>
                {
                    b.Navigation("Stages");

                    b.Navigation("Works");
                });
#pragma warning restore 612, 618
        }
    }
}

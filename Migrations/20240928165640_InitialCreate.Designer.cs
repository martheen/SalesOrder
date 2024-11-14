﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SalesOrder.Data;

#nullable disable

namespace SalesOrder.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240928165640_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SalesOrder.Entities.ComCustomer", b =>
                {
                    b.Property<int>("ComCustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("COM_CUSTOMER_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ComCustomerId"));

                    b.Property<string>("CustomerName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CUSTOMER_NAME");

                    b.HasKey("ComCustomerId");

                    b.ToTable("COM_CUSTOMER", (string)null);
                });

            modelBuilder.Entity("SalesOrder.Entities.SoItem", b =>
                {
                    b.Property<long>("SoItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("SO_ITEM_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("SoItemId"));

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ITEM_NAME");

                    b.Property<double>("Price")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValue(0.0)
                        .HasColumnName("PRICE");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(-99)
                        .HasColumnName("QUANTITY");

                    b.Property<long>("SoOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(-99L)
                        .HasColumnName("SO_ORDER_ID");

                    b.HasKey("SoItemId");

                    b.HasIndex("SoOrderId");

                    b.ToTable("SO_ITEM", (string)null);
                });

            modelBuilder.Entity("SalesOrder.Entities.SoOrder", b =>
                {
                    b.Property<long>("SoOrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("SO_ORDER_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("SoOrderId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("ADDRESS");

                    b.Property<int>("ComCustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(-99)
                        .HasColumnName("COM_CUSTOMER_ID");

                    b.Property<DateTime>("OrderDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("ORDER_DATE")
                        .HasDefaultValueSql("'1900-01-01'");

                    b.Property<string>("OrderNo")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("ORDER_NO");

                    b.HasKey("SoOrderId");

                    b.HasIndex("ComCustomerId");

                    b.ToTable("SO_ORDER", (string)null);
                });

            modelBuilder.Entity("SalesOrder.Entities.SoItem", b =>
                {
                    b.HasOne("SalesOrder.Entities.SoOrder", "Order")
                        .WithMany("Items")
                        .HasForeignKey("SoOrderId")
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("SalesOrder.Entities.SoOrder", b =>
                {
                    b.HasOne("SalesOrder.Entities.ComCustomer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("ComCustomerId")
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("SalesOrder.Entities.ComCustomer", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("SalesOrder.Entities.SoOrder", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
using ECommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderModel>
    {
        public void Configure(EntityTypeBuilder<OrderModel> builder)
        {
            builder
                .HasKey(o => o.Id);
            builder
                .Property(o => o.Id)
                .UseIdentityColumn();
            builder
                .Property(o => o.OrderDate)
                .IsRequired();

            builder
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)");
            
            builder
                .HasOne(o => o.AppUser)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

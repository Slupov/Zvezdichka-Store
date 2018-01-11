﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zvezdichka.Data.EntityConfigurations.Extensions;
using Zvezdichka.Data.Models;

namespace Zvezdichka.Data.EntityConfigurations
{
    public class ProductConfiguration : DbEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.ToTable("Products");

            entity
                .HasIndex(x => x.Name)
                .IsUnique();

            entity
                .Property(x => x.SalePrice)
                .HasDefaultValue(null);

            entity
                .Property(x => x.IsInSale)
                .HasDefaultValue(false);

            entity
                .Property(x => x.Description)
                .HasDefaultValue("");

            entity
                .Property(x => x.Make)
                .HasDefaultValue("");
        }
    }
}

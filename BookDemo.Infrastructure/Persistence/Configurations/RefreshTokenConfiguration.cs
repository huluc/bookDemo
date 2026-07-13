using BookDemo.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {  
            builder.HasKey(rt => rt.Id);

            // Unique: two tokens must never share a hash (would be a serious
            // bug), and this index also speeds up the lookup we run on every
            // single refresh/logout request (WHERE TokenHash = @hash).
            builder.HasIndex(rt => rt.TokenHash).IsUnique();

            // Non-unique index on UserId: supports "get all active sessions for
            // this user" queries (e.g. a future "log out everywhere" feature)
            // without a full table scan.
            builder.HasIndex(rt => rt.UserId);

            builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(200);

            // IsActive is computed in memory only — it has no business being
            // a database column.
            builder.Ignore(rt => rt.IsActive);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // deleting a user cleans up their tokens
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.SecondName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Nickname).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Password).HasMaxLength(30).IsRequired();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasIndex(x => x.Nickname).IsUnique();
        builder.HasMany(x => x.Messages)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProfilePic)
            .WithMany(x => x.Users)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.ProfilePicId).HasDefaultValue(4);
    }
}
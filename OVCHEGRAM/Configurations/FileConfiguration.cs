using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.isImage).HasDefaultValue(false);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasMany(x => x.Messages)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId);
        builder.HasMany(x => x.Users)
            .WithOne(x => x.ProfilePic)
            .HasForeignKey(x => x.ProfilePicId);
    }
}
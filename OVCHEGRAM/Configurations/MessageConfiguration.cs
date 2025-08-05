using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.Property(x => x.Content).HasMaxLength(1000);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasOne(x => x.User)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.File)
            .WithMany(x => x.Messages)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.CreateTime).HasDefaultValueSql("NOW()");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<ConversationEntity>
{
    public void Configure(EntityTypeBuilder<ConversationEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.IsGroupChat).HasDefaultValue(false);
        builder.Property(x => x.LastMessageId).HasDefaultValue(0);
    }
}
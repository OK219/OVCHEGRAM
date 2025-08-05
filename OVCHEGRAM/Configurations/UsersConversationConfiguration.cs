using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Configurations;

public class UsersConversationConfiguration : IEntityTypeConfiguration<UsersConversationEntity>
{
    public void Configure(EntityTypeBuilder<UsersConversationEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.HasOne(x => x.User)
            .WithMany(x => x.UsersConversations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.UsersConversations)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.File)
            .WithMany(x => x.UsersConversations)
            .HasForeignKey(x => x.PictureId);
        builder.HasOne(x => x.LastMessageSeen)
            .WithMany(x => x.UsersConversation)
            .HasForeignKey(x => x.PictureId);
        builder.Property(x => x.PictureId).HasDefaultValue(4);
    }
}
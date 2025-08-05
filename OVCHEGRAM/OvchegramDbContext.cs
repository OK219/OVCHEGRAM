using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.Configurations;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM;

public class OvchegramDbContext : DbContext
{
    public OvchegramDbContext()
    {
    }

    public OvchegramDbContext(DbContextOptions<OvchegramDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity?> Users { get; set; }
    public DbSet<ConversationEntity> Conversations { get; set; }
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<UsersConversationEntity?> UsersConversation { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("OvchegramDbContext"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new ConversationConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        modelBuilder.ApplyConfiguration(new UsersConversationConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public List<T> GetPage<T>(DbSet<T> table, int page = 1, int pageSize = 50, Func<T, bool> filter = null)
        where T : class
    {
        return table
            .Where(filter)
            .Skip(page - 1)
            .Take(pageSize).ToList();
    }
}
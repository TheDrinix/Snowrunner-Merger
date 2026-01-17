using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.OAuth;
using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Api.Models.Saves;

namespace SnowrunnerMerger.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<OAuthConnection> OAuthConnections { get; set; }
    public DbSet<StoredSaveInfo> StoredSaves { get; set; }
    public DbSet<SaveGroup> SaveGroups { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<Map> Maps { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<User>()
            .Property(u => u.Id)
            .HasValueGenerator<GuidValueGenerator>();

        modelBuilder
            .Entity<UserSession>()
            .Property(s => s.Id)
            .HasValueGenerator<GuidValueGenerator>();
        
        modelBuilder
            .Entity<OAuthConnection>()
            .HasKey(c => new { c.Provider, c.ProviderAccountId });
        
        // Unique constraint to ensure a user can only have one connection per provider
        modelBuilder
            .Entity<OAuthConnection>()
            .HasIndex(c => new { c.UserId, c.Provider })
            .IsUnique();
        
        modelBuilder
            .Entity<UserSession>()
            .HasOne(s => s.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(s => s.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<SaveGroup>()
            .Property(g => g.Id)
            .HasValueGenerator<GuidValueGenerator>();
        
        modelBuilder
            .Entity<StoredSaveInfo>()
            .Property(s => s.Id)
            .HasValueGenerator<GuidValueGenerator>();
        
        modelBuilder.Entity<SaveGroup>()
            .HasOne<User>(g => g.Owner)
            .WithMany(u => u.OwnedGroups)
            .HasForeignKey(g => g.OwnerId)
            .HasPrincipalKey(u => u.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaveGroup>()
            .HasMany<User>(g => g.Members)
            .WithMany(u => u.JoinedGroups);

        modelBuilder
            .Entity<UserToken>()
            .HasDiscriminator<string>("TokenType")
            .HasValue<AccountConfirmationToken>("AccountConfirmation")
            .HasValue<PasswordResetToken>("PasswordReset")
            .HasValue<AccountLinkingToken>("AccountLinking")
            .HasValue<AccountCompletionToken>("AccountCompletion");
        
        modelBuilder
            .Entity<AccountLinkingToken>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<PasswordResetToken>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder
            .Entity<AccountConfirmationToken>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<UserSession>()
            .Property(s => s.HasLongLivedRefreshToken)
            .HasDefaultValue(false);

        modelBuilder
            .Entity<Map>()
            .HasKey(m => m.Id);

        modelBuilder
            .Entity<Map>()
            .HasData(new List<Map>()
            {
                new Map("US_01", "Michigan"),
                new Map("US_02", "Alaska"),
                new Map("US_03", "Wisconsin"),
                new Map("US_04", "Yukon"),
                new Map("US_06", "Maine"),
                new Map("US_07", "Tennessee"),
                new Map("US_09", "Ontario"),
                new Map("US_10", "British Columbia"),
                new Map("US_11", "Scandinavia"),
                new Map("US_12", "North Carolina"),
                new Map("US_14", "Austria"),
                new Map("US_15", "Quebec"),
                new Map("US_16", "Washington"),
                new Map("RU_02", "Taymyr"),
                new Map("RU_03", "Kola Peninsula"),
                new Map("RU_04", "Amur"),
                new Map("RU_05", "Don"),
                new Map("RU_08", "Belozersk Glades"),
                new Map("RU_13", "Almaty Region"),
            });
        
        modelBuilder
            .Entity<StoredSaveInfo>()
            .HasMany(s => s.DiscoveredMaps)
            .WithMany();
    }
}
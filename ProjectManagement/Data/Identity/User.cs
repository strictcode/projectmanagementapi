using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Database;

namespace ProjectManagement.Data.Identity;
public class User : IdentityUser<Guid>, ITrackable
{
    public ICollection<IdentityUserClaim<Guid>> Claims { get; } = new HashSet<IdentityUserClaim<Guid>>();

    public Instant CreatedTimestamp { get; set; }
    public string CreatedBy { get; set; } = null!;

    public Instant ModifiedTimestamp { get; set; }
    public string ModifiedBy { get; set; } = null!;

    public static class Entry
    {
        public const string ClaimTypeSuperUser = "CLAIM_SU";
    }

    private class Configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasMany(x => x.Claims)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}


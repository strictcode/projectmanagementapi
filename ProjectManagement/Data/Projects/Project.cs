using System.ComponentModel.DataAnnotations;
using ProjectManagement.Database;

namespace ProjectManagement.Data.Projects
{
    public class Project : ITrackable
    {
        public Guid Id { get; set; }

        [MaxLength(Metadata.NameLength)]
        public string Name { get; set; } = string.Empty;

        public Instant CreatedTimestamp { get; set; }

        public string CreatedBy { get; set; } = null!;

        public Instant ModifiedTimestamp { get; set; }

        public string ModifiedBy { get; set; } = null!;

        public static class Metadata
        {
            public const int NameLength = 120;
        }
    }
}

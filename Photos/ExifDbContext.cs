using System.Data.Entity;

namespace Photos
{
    /// <summary>
    /// The exif db context.
    /// </summary>
    public class ExifDbContext : DbContext
    {
        public DbSet<Exif> Exifs { get; set; }
    }
}

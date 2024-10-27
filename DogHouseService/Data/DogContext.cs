using Microsoft.EntityFrameworkCore;

public class DogContext : DbContext
{
    public DogContext(DbContextOptions<DogContext> options) : base(options) {}

    public DbSet<Dog> Dogs { get; set; }
}

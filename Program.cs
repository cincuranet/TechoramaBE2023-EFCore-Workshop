using Microsoft.EntityFrameworkCore;

namespace Test;

class Program
{
    static void Main()
    {
        using var db = new MyContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }
}

class MyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=workshop;User Id=sa;Password=test;ConnectRetryCount=0;TrustServerCertificate=true");
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Dog> Dogs => Set<Dog>();
}

class Owner
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Dog> Dogs { get; set; }
}
class Dog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }
    public Owner Owner { get; set; }
    public int OwnerId { get; set; }
}
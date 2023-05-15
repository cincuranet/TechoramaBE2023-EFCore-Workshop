using Microsoft.EntityFrameworkCore;

namespace Test;

class Program
{
    static void Main()
    {

    }
}

class MyContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Owner> Owners { get; set; }
    public DbSet<Dog> Dogs { get; set; }
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

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

        modelBuilder.ApplyConfiguration(new OwnerConfiguration());

    }

    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<Dog> Dogs => Set<Dog>();
}

class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("People");
        builder.Property(x => x.LastName)
            .IsRequired()
            .IsFixedLength(false)
            .IsUnicode()
            .HasMaxLength(50)
            .HasColumnName("Surname");
    }
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
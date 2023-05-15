using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Sockets;

namespace Test;

class Program
{
    static void Main()
    {
        using var db = new MyContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        //db.Owners.Add(new Owner() { LastName = "Test" });
        //db.SaveChanges();
        //db.Dogs.Where(x => EF.Property<DateTimeOffset>(x, "LastUpdated") == DateTimeOffset.Now).Load();
        //db.Set<Dictionary<string, object>>("Foo").Add(new Dictionary<string, object>());
        //db.SaveChanges();
        db.Dogs
            //.IgnoreQueryFilters()
            .Where(x => x.DateOfBirth.Year == 2000)
            .ToList();
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
        modelBuilder.ApplyConfiguration(new DogConfiguration());
        modelBuilder.HasSequence("cdssdcds").IsCyclic()
            .IncrementsBy(7839);

        //modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Foo", b =>
        //{
        //    b.Property<int>("Id");
        //    b.Property<string>("Name");
        //    b.Property<int>("Age");
        //    b.Property<double>("Car");
        //});
        //modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Bar", b =>
        //{
        //    b.Property<int>("Id");
        //    b.Property<string>("Foo");
        //});
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
        builder.HasMany(x => x.Dogs)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Id)/*.UseHiLo()*/;
        builder.OwnsOne(x => x.ShippingAddress);
        builder.OwnsOne(x => x.InvoicingAddress);
    }
}
class Owner
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Dog> Dogs { get; set; }
    public Address ShippingAddress { get; set; }
    public Address InvoicingAddress { get; set; }
}
//class Name
//{
//    public string FirstName { get; set; }
//    public string LastName { get; set; }
//}
class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

class DogConfiguration : IEntityTypeConfiguration<Dog>
{
    public void Configure(EntityTypeBuilder<Dog> builder)
    {
        builder.Property(x => x.DateOfBirth)
            .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction)
            .HasField("dob");
        builder.Property<DateTimeOffset>("LastUpdated");
        builder.HasQueryFilter(x => x.Active);
    }
}
class Dog
{
    private DateTimeOffset dob;

    public int Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset DateOfBirth
    {
        get => dob;
        set
        {
            Validate(value);
            dob = value;

            static void Validate(DateTimeOffset dto) { }
        }
    }
    public Owner Owner { get; set; }
    public int OwnerId { get; set; }
    public bool Active { get; set; }
}
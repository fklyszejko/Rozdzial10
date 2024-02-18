using Microsoft.EntityFrameworkCore;
using static System.Console;

namespace BibliotekaWspolna;

public class Northwind : DbContext
{
    // te właściwości odwzorowują tabele z bazy danych
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        if (StaleProjektu.DostawcaDanych == "SQLite")
        {
            string sciezka = Path.Combine(Environment.CurrentDirectory, "Northwind.db");

            WriteLine($"Używam pliku bazy danych {sciezka}.");
            optionsBuilder.UseSqlite($"Filename={sciezka}");
        }
        else
        {
            string polaczenie = "Data Source = .;" + "Initial Catalog=Northwind;" + "Integrated Security = true;" + "MultipleActiveResultSets=true;";

            optionsBuilder.UseSqlServer(polaczenie);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // przykład użycia płynnego API zamiast atrybutów,
        // aby ograniczyć długość nazwy kategorii do 40 znaków
        modelBuilder.Entity<Category>()
            .Property(category => category.CategoryName)
            .IsRequired() // Not Null
            .HasMaxLength(15);

            if(StaleProjektu.DostawcaDanych == "SQLite")
        {
            // dodanie w celu "naprawienia" braku typu decimal w SQLite
            modelBuilder.Entity<Product>()
                .Property(product => product.Koszt)
                .HasConversion<double>();
        }
        // globalny filtr usuwający produkty nieprodukowane
        modelBuilder.Entity<Product>().
                HasQueryFilter(p => !p.Discontinued);
    }
}
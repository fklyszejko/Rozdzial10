using Microsoft.EntityFrameworkCore;

using static System.Console;

namespace UczniowieIKursy;
public class Akademia: DbContext
{
    public DbSet<Uczen>? Uczniowie { get; set; }
    public DbSet<Kurs> Kurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /*        string sciezka = Path.Combine(Environment.CurrentDirectory, "Akademia.db");
                WriteLine($"Używam pliku bazy danych {sciezka}.");

                optionsBuilder.UseSqlite($"Filename={sciezka}");*/

        optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=Akademia;Integrated Security=true;MultipleActiveResultSets=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Reguły sprawdzenia poprawności w płynnym API
        modelBuilder.Entity<Uczen>()
            .Property(s => s.Nazwisko).HasMaxLength(30).IsRequired();

        // Wypełnienie bazy przykładowymi danymi
        Uczen alicja = new()
        {
            UczenId = 1,
            Imie = "Alicja",
            Nazwisko = "Nowak"
        };
        Uczen bartek = new()
        {
            UczenId = 2,
            Imie = "Bartek",
            Nazwisko = "Kowalski"
        };
        Uczen celina = new()
        {
            UczenId = 3,
            Imie = "Celina",
            Nazwisko = "Poranna"
        };

        Kurs csharp = new()
        {
            KursId = 1,
            Nazwa = "C# 10 i .NET 6"
        };
        Kurs webdev = new()
        {
            KursId = 2,
            Nazwa = "Tworzenie stron WWW"
        };
        Kurs python = new()
        {
            KursId = 3,
            Nazwa = "Python dla początkujących"
        };

        modelBuilder.Entity<Uczen>()
            .HasData(alicja, bartek, celina);

        modelBuilder.Entity<Kurs>()
            .HasData(csharp, webdev, python);

        modelBuilder.Entity<Kurs>()
            .HasMany(c => c.Uczniowie)
            .WithMany(u => u.Kursy)
            .UsingEntity(e => e.HasData(
                // Wszyscy uczniowie zapisani na kurs C#
                new { KursyKursId = 1, UczniowieUczenId = 1 },
                new { KursyKursId = 1, UczniowieUczenId = 2 },
                new { KursyKursId = 1, UczniowieUczenId = 3},
                // Tylko Bartek zapisał się na kurs tworzenia stron WWW
                new { KursyKursId = 2, UczniowieUczenId = 2 },
                // Tylko Celina zapisała się na kurs Pythona
                new { KursyKursId = 3, UczniowieUczenId = 3 }
                ));
    }
}
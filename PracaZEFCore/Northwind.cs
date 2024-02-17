using Microsoft.EntityFrameworkCore;
using static System.Console;

namespace BibliotekaWspolna;

public class Northwind : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (StaleProjektu.DostawcaDanych == "SQLite")
        {
            string sciezka = Path.Combine(Environment.CurrentDirectory, "Northwind.db");

            WriteLine($"Używam pliku bazy danych {sciezka}.");
            optionsBuilder.UseSqlite($"Filename=sciezka");
        }
        else
        {
            string polaczenie = "Data Source = .;" + "Initial Catalog=Northwind;" + "Integrated Security = true;" + "MultipleActiveResultSets=true;";

            optionsBuilder.UseSqlServer(polaczenie);
        }
    }
}
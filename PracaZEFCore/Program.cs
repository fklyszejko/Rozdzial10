using BibliotekaWspolna;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using static System.Console;

WriteLine($"Używam dostawcy danych {StaleProjektu.DostawcaDanych}");
//ZapytanieOKategorie();
//FiltrowanieDolaczen();
//ZapytanieOProdukty();
//ZapytanieZLike();

/*if (DodajProdukt(6, "Burgery Boba", 500M))
{
    WriteLine("Dodano nowy produkt.");
}*/

/*if (ZwiekszCeneProduktu(poczatekNazwy: "Burg", kwota: 20M))
{
    WriteLine("Zaktualizowano cenę produktu.");
}*/
int usuniete = UsunProdukty("Burg");
WriteLine($"{usuniete} produktów zostało usuniętych.");
WypiszProdukty();

static void ZapytanieOKategorie()
{
    using (Northwind db = new Northwind())
    {
        var fabrykaProtokolu = db.GetService<ILoggerFactory>();
        fabrykaProtokolu.AddProvider(new DostawcaProkoluKonsoli());
        WriteLine("Lista kategorii i liczba przypisanych im produktów:");

        // zapytanie pobiera wszystkie kategorie i związane z nimi produkty
        IQueryable<Category> kategorie;
        //= db.Categories;
        //.Include(c => c.Products);

        db.ChangeTracker.LazyLoadingEnabled = false;

        Write("Włączyć łaodwanie chętne? (T/N): ");
        bool ladowanieChetne = (ReadKey().Key == ConsoleKey.T);
        bool ladowanieJawne = false;
        WriteLine();

        if (ladowanieChetne)
        {
            kategorie = db.Categories.Include(c => c.Products);
        }
        else
        {
            kategorie = db.Categories;

            Write("Włączyć ładowanie jawne? (T/N): ");
            ladowanieJawne = (ReadKey().Key == ConsoleKey.T);
            WriteLine();
        }

        if (kategorie is null)
        {
            WriteLine("Nie znaleziono żadnych kategorii.");
            return;
        }
        // wykonaj zapytanie i przejrzyj wyniki
        foreach (Category k in kategorie)
        {
            if (ladowanieJawne)
            {
                Write($"Jawnie załadować produkty z kategorii {k.CategoryName}? (T/N): ");
                ConsoleKeyInfo key = ReadKey();
                WriteLine();
                if (key.Key == ConsoleKey.T)
                {
                    CollectionEntry<Category, Product> products = db.Entry(k).Collection(k2 => k2.Products);
                    if (!products.IsLoaded) products.Load();
                }
            }
            WriteLine($"Kategoria {k.CategoryName} ma {k.Products.Count} produktów.");
        }
    }
}

static void FiltrowanieDolaczen()
{
    using (Northwind db = new Northwind())
    {
        WriteLine("Podaj minimalną liczbę sztuk w magazynie: ");
        string sztukWMagazynie = ReadLine() ?? "10";
        int sztuki = int.Parse(sztukWMagazynie);

        IQueryable<Category> kategorie = db.Categories?
            .Include(c => c.Products.Where(p => p.WMagazynie >= sztuki));

        if (kategorie is null)
        {
            WriteLine("Nie znaleziono kategorii.");
            return;
        }
        WriteLine($"ToQueryString: {kategorie.ToQueryString()}");
        foreach (Category c in kategorie)
        {
            WriteLine($"Kategoria {c.CategoryName} ma {c.Products.Count} produktów z przynajmniej {sztuki} sztukami w magazynie.");
            foreach (Product p in c.Products)
            {
                WriteLine($"Produkt {p.ProductName}: {p.WMagazynie} sztuk");
            }
        }
    }
}

static void ZapytanieOProdukty()
{
    using (Northwind db = new())
    {
        var fabrykaProtokolu = db.GetService<ILoggerFactory>();
        fabrykaProtokolu.AddProvider(new DostawcaProkoluKonsoli());

        WriteLine("Produkty kosztujące więcej niż podana cena; posortowane:");
        string? wejscie;
        decimal cena;

        do
        {
            Write("Podaj cenę produktu: ");
            wejscie = ReadLine();
        } while (!decimal.TryParse(wejscie, out cena));

        IQueryable<Product>? produkty = db.Products?
            .Where(produkt => produkt.Koszt > cena)
            .OrderByDescending(produkt => produkt.Koszt);

        if (produkty is null)
        {
            WriteLine("Nie znaleziono produktów.");
            return;
        }

        foreach (Product produkt in produkty)
        {
            WriteLine("{0} : {1} kosztuje {2:$#,##0.00}. W magazynie jest {3} sztuk.",
                produkt.ProductID, produkt.ProductName, produkt.Koszt, produkt.WMagazynie);
        }
    }
}

static void ZapytanieZLike()
{
    using (Northwind db = new())
    {
        ILoggerFactory fabrykaProtokolu = db.GetService<ILoggerFactory>();
        fabrykaProtokolu.AddProvider(new DostawcaProkoluKonsoli());

        Write("Wprowadź część nazwy produktu: ");
        string input = ReadLine();

        IQueryable<Product> Products = db.Products?
            .Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));

        if (Products is null)
        {
            WriteLine("Nie znaleziono produktów.");
            return;
        }
        foreach (Product Product in Products)
        {
            WriteLine("{0}: w magazynie jest {1} sztuk. Produkt nie jest już wytwarzany? {2}",
                Product.ProductName, Product.WMagazynie, Product.Discontinued);
        }
    }
}

static bool DodajProdukt(int idKategorii, string nazwaProduktu, decimal? cena)
{
    using (Northwind db = new())
    {
        Product nowyProdukt = new()
        {
            CategoryID = idKategorii,
            ProductName = nazwaProduktu,
            Koszt = cena
        };

        // oznacz produkt jako dodany w systemie śledzenia zmian
        db.Products.Add(nowyProdukt);

        // zapisz wszystkie zmiany w bazie
        int zmienione = db.SaveChanges();
        return (zmienione == 1);
    }
}

static void WypiszProdukty()
{
    using (Northwind db = new())
    {
        WriteLine("{0,-3} {1,-35} {2,8} {3,5} {4}",
            "ID", "Nazwa", "Koszt", "Stan", "Nieprod.");

        foreach (Product pozycja in db.Products
            .OrderByDescending(p => p.Koszt))
        {
            WriteLine("{0:000} {1,-35} {2,8:$#,##0.00} {3,5} {4}",
                pozycja.ProductID, pozycja.ProductName, pozycja.Koszt, pozycja.WMagazynie, pozycja.Discontinued);
        }
    }
}

static bool ZwiekszCeneProduktu(string poczatekNazwy, decimal kwota)
{
    using (var db = new Northwind())
    {
        // pobierz pierwszy produkt, którego nazwa zaczyna się od wartości parametru nazwa
        Product produktDoAktualizacji = db.Products.First(p => p.ProductName.StartsWith(poczatekNazwy));

        produktDoAktualizacji.Koszt += kwota;

        int zmienione = db.SaveChanges();
        return (zmienione == 1);
    }
}

static int UsunProdukty(string poczatekNazwy)
{
    using(Northwind db = new())
    {
        using (IDbContextTransaction t = db.Database.BeginTransaction())
        {
            WriteLine("Transakcja uruchomiona z poziomem izolacji: {0}",
                arg0: t.GetDbTransaction().IsolationLevel);

            IQueryable<Product>? products = db.Products?.Where(p => p.ProductName.StartsWith(poczatekNazwy));

            if (products is null)
            {
                WriteLine("Nie znaleziono produktów do usunięcia.");
                return 0;
            }
            else
            {
                db.Products.RemoveRange(products);
            }

            int zmienione = db.SaveChanges();
            t.Commit();
            return zmienione;
        }
    }
}
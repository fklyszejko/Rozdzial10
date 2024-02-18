namespace UczniowieIKursy;

public class Uczen
{
    public int UczenId { get; set; }
    public string? Imie { get; set; }
    public string? Nazwisko { get; set; }
    public ICollection<Kurs>? Kursy { get; set; }
}

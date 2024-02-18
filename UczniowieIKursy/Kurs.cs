using System.ComponentModel.DataAnnotations;

namespace UczniowieIKursy;

public class Kurs
{
    public int KursId { get; set; }

    [Required]
    [StringLength(60)]
    public string? Nazwa { get; set; }
    public ICollection<Uczen> Uczniowie { get; set; }
}
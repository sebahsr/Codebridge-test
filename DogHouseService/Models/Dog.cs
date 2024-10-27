using System.ComponentModel.DataAnnotations;

public class Dog
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Color { get; set; }
    
    [Range(0, int.MaxValue)]
    public int TailLength { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Weight { get; set; }
}

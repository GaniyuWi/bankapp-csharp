
public class UpdatePinDto
{
    public long AccountNumber { get; set; }
    public string OldPin { get; set; } = null!;
    public string NewPin { get; set; } = null!;
}
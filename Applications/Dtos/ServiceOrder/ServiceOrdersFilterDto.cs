namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderFilterDto
{
    public string? Status { get; set; }       // "Production", "TryIn", "Finished"
    public int? ClientId { get; set; }
    public DateTime? Start { get; set; }      // data inicial (inclusive)
    public DateTime? End { get; set; }        // data final (inclusive)
}

}

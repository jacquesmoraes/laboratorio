namespace Applications.Dtos.ServiceOrder
{
    public class ServiceOrderFilterDto
{
    public string? Status { get; set; }       
    public int? ClientId { get; set; }
    public DateTime? Start { get; set; }      
    public DateTime? End { get; set; }        
}

}

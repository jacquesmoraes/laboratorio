namespace Applications.Dtos.ServiceOrder
{
    public class FinishOrderDto
    {
        public List<int> ServiceOrderIds { get; set; } = [];
        public DateTime DateOut { get; set; }
    }
}

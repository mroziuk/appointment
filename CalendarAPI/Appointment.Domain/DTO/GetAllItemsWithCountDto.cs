namespace Appointment.Domain.DTO;

public class GetAllItemsWithCountDto<T>
{
    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public List<T> Items { get; set; }
}

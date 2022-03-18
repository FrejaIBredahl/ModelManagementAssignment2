namespace ModelManagementAssignment2.ViewModels
{
    public class UpdateJobDTO
    {
        public long JobId { get; set; }
        public string? Customer { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public int Days { get; set; }
        public string? Location { get; set; }
        public string? Comments { get; set; }
    }
}

﻿using ModelManagementAssignment2.Models;

namespace ModelManagementAssignment2.ViewModels
{
    public class GetJobViewModel
    {
        public long JobId { get; set; }
        public string? Customer { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public int Days { get; set; }
        public string? Location { get; set; }
        public string? Comments { get; set; }
        public List<Model>? Models { get; set; }
    }
}
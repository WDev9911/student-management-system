using StudentManagementSystem.BLL.DTOs.Attendance;

namespace StudentManagementSystem.Web.ViewModels.Attendance
{
    public class StudentAttendanceViewModel
    {
        public DateTime SelectedDate { get; set; }
        public int? SelectedClassId { get; set; }
        public List<StudentAttendanceClassDto> Classes { get; set; } = new();
        public List<StudentAttendanceDto> Items { get; set; } = new();
    }
}
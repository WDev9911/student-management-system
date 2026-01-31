using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.DTOs.Attendance;

namespace StudentManagementSystem.Web.ViewModels.Attendance
{
    public class InstructorAttendanceViewModel
    {
        public List<ClassDto> Classes { get; set; } = new();
        public AttendanceSessionDto? Session { get; set; }
        public int? SelectedClassId { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public List<InstructorAttendanceSessionDto> Sessions { get; set; } = new();
    }
}
namespace StudentManagementSystem.BLL.DTOs.Attendance
{
    public class AttendanceSaveRequest
    {
        public int ClassId { get; set; }
        public DateTime SessionDate { get; set; }
        public List<AttendanceSaveItem> Students { get; set; } = new();
    }

    public class AttendanceSaveItem
    {
        public int StudentId { get; set; }
        public string Status { get; set; } = "Present"; // Present | Absent
    }
}
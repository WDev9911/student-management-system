using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            
            return courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Credits = c.Credits,
                TuitionFee = c.TuitionFee,
                Description = c.Description,
                MajorId = c.MajorId,
                MajorName = c.Major.MajorName,
                SemesterNumber = c.SemesterNumber,
                PrerequisiteCourseId = c.PrerequisiteCourseId,
                PrerequisiteCourseName = c.PrerequisiteCourse?.CourseName
            }).ToList();
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return null;
            }

            return new CourseDto
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                TuitionFee = course.TuitionFee,
                Description = course.Description,
                MajorId = course.MajorId,
                MajorName = course.Major.MajorName,
                SemesterNumber = course.SemesterNumber,
                PrerequisiteCourseId = course.PrerequisiteCourseId,
                PrerequisiteCourseName = course.PrerequisiteCourse?.CourseName
            };
        }

        public async Task<List<CourseDto>> GetCoursesByMajorAndSemesterAsync(int majorId, int semesterNumber)
        {
            var courses = await _courseRepository.GetCoursesByMajorAndSemesterAsync(majorId, semesterNumber);
            
            return courses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Credits = c.Credits,
                TuitionFee = c.TuitionFee,
                Description = c.Description,
                MajorId = c.MajorId,
                MajorName = c.Major.MajorName,
                SemesterNumber = c.SemesterNumber,
                PrerequisiteCourseId = c.PrerequisiteCourseId,
                PrerequisiteCourseName = c.PrerequisiteCourse?.CourseName
            }).ToList();
        }
    }
}
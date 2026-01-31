using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class GradeService : IGradeService
    {
        private readonly IClassRepository _classRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IPendingEnrollmentRepository _pendingRepo;
        private readonly ICourseRepository _courseRepo;

        public GradeService(
            IClassRepository classRepo,
            IEnrollmentRepository enrollmentRepo,
            IGradeRepository gradeRepo,
            IPendingEnrollmentRepository pendingRepo,
            ICourseRepository courseRepo)
        {
            _classRepo = classRepo;
            _enrollmentRepo = enrollmentRepo;
            _gradeRepo = gradeRepo;
            _pendingRepo = pendingRepo;
            _courseRepo = courseRepo;
        }

        public async Task<GradeEntryDto?> GetGradeEntryAsync(int classId, int instructorId)
        {
            var cls = await _classRepo.GetByIdAsync(classId);
            if (cls == null || cls.InstructorId != instructorId) return null;

            var enrollments = await _enrollmentRepo.GetEnrollmentsByClassIdAsync(classId);
            var grades = await _gradeRepo.GetByEnrollmentIdsAsync(enrollments.Select(e => e.EnrollmentId).ToList());
            var gradeMap = grades.ToDictionary(g => g.EnrollmentId);

            var model = new GradeEntryDto
            {
                ClassId = cls.ClassId,
                ClassName = cls.ClassName,
                CourseCode = cls.Course.CourseCode,
                CourseName = cls.Course.CourseName,
                SemesterName = cls.Semester.SemesterName,
                Schedule = cls.Schedule,
                Room = cls.Room,
                IsPublished = grades.Any() && grades.All(g => g.IsPublished)
            };

            foreach (var e in enrollments)
            {
                gradeMap.TryGetValue(e.EnrollmentId, out var g);

                model.Students.Add(new StudentGradeEntryDto
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    StudentCode = e.Student.StudentCode,
                    FullName = e.Student.FullName,
                    QuizScore = g?.QuizScore ?? 0,
                    MidtermScore = g?.MidtermScore ?? 0,
                    AssignmentScore = g?.AssignmentScore ?? 0,
                    FinalExamScore = g?.FinalExamScore ?? 0,
                    FinalScore = g?.FinalScore ?? 0,
                    IsPassed = g?.IsPassed ?? false
                });
            }

            return model;
        }

        public async Task SaveGradesAsync(GradeEntryDto model, string action)
        {
            var enrollments = await _enrollmentRepo.GetEnrollmentsByClassIdAsync(model.ClassId);
            var enrollMap = enrollments.ToDictionary(e => e.EnrollmentId);
            var grades = await _gradeRepo.GetByEnrollmentIdsAsync(enrollMap.Keys.ToList());
            var gradeMap = grades.ToDictionary(g => g.EnrollmentId);

            var upserts = new List<Grade>();

            foreach (var row in model.Students)
            {
                if (!enrollMap.TryGetValue(row.EnrollmentId, out var enrollment)) continue;

                var quiz = Math.Clamp(row.QuizScore, 0, 10);
                var mid = Math.Clamp(row.MidtermScore, 0, 10);
                var ass = Math.Clamp(row.AssignmentScore, 0, 10);
                var fin = Math.Clamp(row.FinalExamScore, 0, 10);

                var finalScore = Math.Round(0.1m * quiz + 0.2m * mid + 0.2m * ass + 0.5m * fin, 2);
                var isPassed = finalScore >= 5;

                gradeMap.TryGetValue(row.EnrollmentId, out var grade);

                grade ??= new Grade
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    StudentId = enrollment.StudentId,
                    CourseId = enrollment.CourseId,
                    CreatedDate = DateTime.Now
                };

                grade.QuizScore = quiz;
                grade.MidtermScore = mid;
                grade.AssignmentScore = ass;
                grade.FinalExamScore = fin;
                grade.FinalScore = finalScore;
                grade.IsPassed = isPassed;
                grade.LetterGrade = null;
                grade.GradePoint = 0;
                grade.GradedDate = DateTime.Now;

                if (action == "publish") grade.IsPublished = true;
                if (action == "unpublish") grade.IsPublished = false;

                upserts.Add(grade);
            }

            await _gradeRepo.UpsertGradesAsync(upserts);

            if (action is "publish" or "unpublish")
            {
                await SyncPrerequisitePendingAsync(enrollments, upserts, action);
            }
        }

        private async Task SyncPrerequisitePendingAsync(
            List<Enrollment> enrollments,
            List<Grade> grades,
            string action)
        {
            var enrollMap = enrollments.ToDictionary(e => e.EnrollmentId);
            var coursesByMajorCache = new Dictionary<int, List<Course>>();
            var pendingByStudentCache = new Dictionary<int, List<PendingEnrollment>>();

            foreach (var grade in grades)
            {
                if (!enrollMap.TryGetValue(grade.EnrollmentId, out var enrollment)) continue;

                var studentId = enrollment.StudentId;
                var majorId = enrollment.Student.MajorId;
                var openSemester = enrollment.Student.CurrentSemester;

                if (!coursesByMajorCache.TryGetValue(majorId, out var majorCourses))
                {
                    majorCourses = await _courseRepo.GetCoursesByMajorAsync(majorId);
                    coursesByMajorCache[majorId] = majorCourses;
                }

                if (!pendingByStudentCache.TryGetValue(studentId, out var pendingList))
                {
                    pendingList = await _pendingRepo.GetPendingByStudentIdAsync(studentId);
                    pendingByStudentCache[studentId] = pendingList;
                }

                var blockedCourses = majorCourses
                    .Where(c => c.PrerequisiteCourseId == grade.CourseId)
                    .ToList();

                foreach (var course in blockedCourses)
                {
                    var existing = pendingList
                        .FirstOrDefault(p => p.CourseId == course.CourseId && !p.IsRetake && p.Status == "Pending");

                    if (action == "publish")
                    {
                        if (grade.IsPassed)
                        {
                            if (existing != null)
                            {
                                existing.Status = "Cancelled";
                                await _pendingRepo.UpdateAsync(existing);
                            }

                            continue;
                        }

                        if (existing != null) continue;

                        await _pendingRepo.AddAsync(new PendingEnrollment
                        {
                            StudentId = studentId,
                            CourseId = course.CourseId,
                            SemesterNumber = openSemester,
                            IsRetake = false,
                            RequiredFee = Math.Round(course.TuitionFee, 0),
                            Reason = "Prerequisite not met",
                            Status = "Pending",
                            CreatedDate = DateTime.Now
                        });
                    }

                    if (action == "unpublish")
                    {
                        if (existing == null) continue;
                        if (existing.IsRetake) continue;
                        if (existing.Reason != "Prerequisite not met") continue;

                        existing.Status = "Cancelled";
                        await _pendingRepo.UpdateAsync(existing);
                    }
                }
            }

            await _pendingRepo.SaveChangesAsync();
        }
    }
}
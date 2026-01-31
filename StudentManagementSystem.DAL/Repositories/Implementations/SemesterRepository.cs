using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public SemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Semester>> GetAllSemestersAsync()
        {
            return await _context.Semesters
                .OrderBy(s => s.SemesterNumber)
                .ToListAsync();
        }

        public async Task<Semester?> GetSemesterByIdAsync(int id)
        {
            return await _context.Semesters.FindAsync(id);
        }

        public async Task<Semester?> GetCurrentSemesterAsync()
        {
            return await _context.Semesters
                .Where(s => s.Status == "Active")
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Semester?> GetSemesterByNumberAsync(int semesterNumber)
        {
            return await _context.Semesters
                .Where(s => s.SemesterNumber == semesterNumber)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateSemesterAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }
    }
}
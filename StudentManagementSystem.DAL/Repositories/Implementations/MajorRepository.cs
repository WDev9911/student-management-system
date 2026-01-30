using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class MajorRepository : IMajorRepository
    {
        private readonly ApplicationDbContext _context;

        public MajorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Major>> GetAllMajorsAsync()
        {
            return await _context.Majors
                .OrderBy(m => m.MajorCode)
                .ToListAsync();
        }

        public async Task<Major?> GetMajorByIdAsync(int id)
        {
            return await _context.Majors.FindAsync(id);
        }

        public async Task<Major?> GetMajorByCodeAsync(string code)
        {
            return await _context.Majors
                .FirstOrDefaultAsync(m => m.MajorCode == code);
        }
    }
}
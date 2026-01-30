using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly ApplicationDbContext _context;

        public InstructorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Instructor>> GetAllInstructorsAsync()
        {
            return await _context.Instructors
                .OrderBy(i => i.FullName)
                .ToListAsync();
        }

        public async Task<Instructor?> GetInstructorByIdAsync(int id)
        {
            return await _context.Instructors.FindAsync(id);
        }

        public async Task<Instructor?> GetInstructorByCodeAsync(string code)
        {
            return await _context.Instructors
                .FirstOrDefaultAsync(i => i.InstructorCode == code);
        }

        public async Task<Instructor?> GetInstructorByEmailAsync(string email)
        {
            return await _context.Instructors
                .FirstOrDefaultAsync(i => i.Email == email);
        }

        public async Task<Instructor> AddInstructorAsync(Instructor instructor)
        {
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
            return instructor;
        }

        public async Task<Instructor> UpdateInstructorAsync(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
            await _context.SaveChangesAsync();
            return instructor;
        }

        public async Task<bool> DeleteInstructorAsync(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return false;
            }

            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InstructorCodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _context.Instructors.Where(i => i.InstructorCode == code);
            
            if (excludeId.HasValue)
            {
                query = query.Where(i => i.InstructorId != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Instructors.Where(i => i.Email == email);
            
            if (excludeId.HasValue)
            {
                query = query.Where(i => i.InstructorId != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}
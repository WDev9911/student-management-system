namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task<int> GetCompletedCreditsAsync(int studentId);
    }
}
namespace _232127DelaCruzIPTCRUD.Models
{
    public class _232127DelaCruzDashboardModel
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalUsers { get; set; }
        public List<GenderStatistic>? GenderDistribution { get; set; }
        public List<_232127DelaCruzCourseModel>? CourseDistribution { get; set; }
        public List<_232127DelaCruzStudentModel>? RecentStudents { get; set; }
        public List<_232127DelaCruzStudentModel>? UpcomingBirthdays { get; set; }
    }

    public class GenderStatistic
    {
        public string Gender { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}

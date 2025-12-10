namespace _232127DelaCruzIPTCRUD.Models
{
    public class _232127DelaCruzStudentModel
    {
        public int STUDENTID { get; set; }
        public string FNAME { get; set; } = string.Empty;
        public string LNAME { get; set; } = string.Empty;
        public string? MNAME { get; set; }
        public DateTime BDAY { get; set; }
        public string GENDER { get; set; } = string.Empty;
        public int COURSEID { get; set; }
        public string? CourseName { get; set; }
    }
}

namespace UniversityTasksDbFirstApi.Models;

public partial class Student
{
    public string FullName => $"{FirstName} {LastName}";

    public bool HasAcademicEmail()
    {
        return Email.EndsWith("@pjswtk.edu.pl", StringComparison.OrdinalIgnoreCase);
    }
}

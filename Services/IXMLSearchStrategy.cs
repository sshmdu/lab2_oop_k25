using lab2.Models;
namespace lab2.Services;

public class SearchFilter
{
    public string Faculty { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string DepartmentKeyword { get; set; } = string.Empty;
    public string SelectedAttributeValue { get; set; } = string.Empty;
    public string Describe()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Faculty)) parts.Add($"faculty = {Faculty}");
        if (!string.IsNullOrWhiteSpace(Course)) parts.Add($"course = {Course}");
        if (!string.IsNullOrWhiteSpace(DepartmentKeyword)) parts.Add($"department contains {DepartmentKeyword}");
        if (!string.IsNullOrWhiteSpace(SelectedAttributeValue)) parts.Add(SelectedAttributeValue);
        return parts.Count > 0 ? string.Join(" AND ", parts) : "all students";
    }
}
public interface IXMLSearchStrategy
{
    string Name { get; }
    string LastQuery { get; }
    List<StudentModel> Analyze(string xmlPath, SearchFilter filter);
}
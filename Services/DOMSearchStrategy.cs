using System.Xml;
using lab2.Models;

namespace lab2.Services;

public class DomSearchStrategy : IXMLSearchStrategy
{
    public string LastQuery { get; private set; } = string.Empty;
    public string Name => "DOM (XmlDocument)";

    public List<StudentModel> Analyze(string xmlPath, SearchFilter filter)
    {
        var results = new List<StudentModel>();
        var doc = new XmlDocument();
        doc.Load(xmlPath);

        var conditions = new List<string>();
        if (!string.IsNullOrWhiteSpace(filter.Faculty))
            conditions.Add($"@faculty='{filter.Faculty}'");
        if (!string.IsNullOrWhiteSpace(filter.Course))
            conditions.Add($"@course='{filter.Course}'");
        if (!string.IsNullOrWhiteSpace(filter.DepartmentKeyword))
            conditions.Add($"contains(Department,'{filter.DepartmentKeyword}')");

        string xpath = "//Student";
        if (conditions.Count > 0)
            xpath += "[" + string.Join(" and ", conditions) + "]";

        LastQuery = xpath;
        XmlNodeList? studentNodes = doc.SelectNodes(xpath);
        if (studentNodes == null) return results;

        foreach (XmlNode node in studentNodes)
        {
            var student = new StudentModel
            {
                Faculty = node.Attributes?["faculty"]?.Value ?? "",
                Course = node.Attributes?["course"]?.Value ?? "",
                FullName = node.SelectSingleNode("FullName")?.InnerText ?? "",
                Department = node.SelectSingleNode("Department")?.InnerText ?? ""
            };

            var roomNode = node.SelectSingleNode("Residence/Room");
            if (roomNode != null && roomNode.Attributes != null)
            {
                student.RoomNumber = roomNode.Attributes["number"]?.Value ?? "";
                student.CheckInDate = roomNode.Attributes["checkIn"]?.Value ?? "";
                student.CheckOutDate = roomNode.Attributes["checkOut"]?.Value ?? "";
            }

            var attrs = new List<string>();
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    attrs.Add($"{attr.Name}='{attr.Value}'");
                }
            }
            student.AttributesSummary = string.Join(", ", attrs);

            if (IsMatch(student, filter))
            {
                results.Add(student);
            }
        }
        return results;
    }

    private static bool IsMatch(StudentModel s, SearchFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Faculty) && !s.Faculty.Equals(filter.Faculty, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(filter.Course) && !s.Course.Equals(filter.Course, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(filter.DepartmentKeyword) && !s.Department.Contains(filter.DepartmentKeyword, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(filter.SelectedAttributeValue) && !s.AttributesSummary.Contains(filter.SelectedAttributeValue, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }
}
using System.Xml.Linq;
using lab2.Models;

namespace lab2.Services;

public class LinqToXmlSearchStrategy : IXMLSearchStrategy
{
    public string LastQuery { get; private set; } = string.Empty;
    public string Name => "LINQ to XML (XDocument)";

    public List<StudentModel> Analyze(string xmlPath, SearchFilter filter)
    {
        LastQuery = "LINQ where: " + filter.Describe();
        var doc = XDocument.Load(xmlPath);

        var query = from student in doc.Descendants("Student")
                    let faculty = (string?)student.Attribute("faculty") ?? ""
                    let course = (string?)student.Attribute("course") ?? ""
                    let fullName = (string?)student.Element("FullName") ?? ""
                    let department = (string?)student.Element("Department") ?? ""
                    let room = student.Element("Residence")?.Element("Room")
                    let roomNum = (string?)room?.Attribute("number") ?? ""
                    let checkIn = (string?)room?.Attribute("checkIn") ?? ""
                    let checkOut = (string?)room?.Attribute("checkOut") ?? ""
                    let attrs = string.Join(", ", student.Attributes().Select(a => $"{a.Name}='{a.Value}'"))
                    select new StudentModel
                    {
                        Faculty = faculty,
                        Course = course,
                        FullName = fullName,
                        Department = department,
                        RoomNumber = roomNum,
                        CheckInDate = checkIn,
                        CheckOutDate = checkOut,
                        AttributesSummary = attrs
                    };

        return query.Where(s =>
            (string.IsNullOrWhiteSpace(filter.Faculty) || s.Faculty.Equals(filter.Faculty, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(filter.Course) || s.Course.Equals(filter.Course, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(filter.DepartmentKeyword) || s.Department.Contains(filter.DepartmentKeyword, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(filter.SelectedAttributeValue) || s.AttributesSummary.Contains(filter.SelectedAttributeValue, StringComparison.OrdinalIgnoreCase))
        ).ToList();
    }
}
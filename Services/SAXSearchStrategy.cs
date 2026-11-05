using System.Xml;
using lab2.Models;
namespace lab2.Services;

public class SaxSearchStrategy : IXMLSearchStrategy
{
    public string LastQuery { get; private set; } = string.Empty;
    public string Name => "SAX (XmlReader)";

    public List<StudentModel> Analyze(string xmlPath, SearchFilter filter)
    {
        LastQuery = "SAX filter: " + filter.Describe();
        var results = new List<StudentModel>();
        using var reader = XmlReader.Create(xmlPath);

        StudentModel? currentStudent = null;
        string currentElement = string.Empty;
        bool matchesFilter = false;

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    currentElement = reader.Name;
                    if (currentElement == "Student")
                    {
                        currentStudent = new StudentModel();
                        string faculty = reader.GetAttribute("faculty") ?? "";
                        string course = reader.GetAttribute("course") ?? "";
                        currentStudent.Faculty = faculty;
                        currentStudent.Course = course;

                        var attributesList = new List<string>();
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                attributesList.Add($"{reader.Name}='{reader.Value}'");
                            }
                            reader.MoveToElement();
                        }
                        currentStudent.AttributesSummary = string.Join(", ", attributesList);
                    }
                    else if (currentElement == "Room" && currentStudent != null)
                    {
                        currentStudent.RoomNumber = reader.GetAttribute("number") ?? "";
                        currentStudent.CheckInDate = reader.GetAttribute("checkIn") ?? "";
                        currentStudent.CheckOutDate = reader.GetAttribute("checkOut") ?? "";
                    }
                    break;

                case XmlNodeType.Text:
                    if (currentStudent != null)
                    {
                        if (currentElement == "FullName") currentStudent.FullName = reader.Value;
                        if (currentElement == "Department") currentStudent.Department = reader.Value;
                    }
                    break;

                case XmlNodeType.EndElement:
                    if (reader.Name == "Student" && currentStudent != null)
                    {
                        if (IsMatch(currentStudent, filter))
                        {
                            results.Add(currentStudent);
                        }
                        currentStudent = null;
                    }
                    break;
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
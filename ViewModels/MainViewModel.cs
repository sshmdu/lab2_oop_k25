using lab2.Models;
using lab2.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;

namespace lab2.ViewModels;

public class MainViewModel : BindableObject
{
    private string _xmlPath = string.Empty;
    private string _xslPath = string.Empty;
    private string _selectedFaculty = string.Empty;
    private string _selectedCourse = string.Empty;
    private string _departmentKeyword = string.Empty;
    private string _selectedDynamicAttribute = string.Empty;
    private IXMLSearchStrategy _selectedStrategy;

    public ObservableCollection<IXMLSearchStrategy> Strategies { get; }
    public ObservableCollection<string> DynamicAttributes { get; } = new();
    public ObservableCollection<StudentModel> SearchResults { get; } = new();

    public string XmlPath
    {
        get => _xmlPath;
        set { _xmlPath = value; OnPropertyChanged(); }
    }

    public string XslPath
    {
        get => _xslPath;
        set { _xslPath = value; OnPropertyChanged(); }
    }

    public string SelectedFaculty
    {
        get => _selectedFaculty;
        set { _selectedFaculty = value; OnPropertyChanged(); }
    }
    private string _generatedQuery = string.Empty;
    public string GeneratedQuery
    {
        get => _generatedQuery;
        set { _generatedQuery = value; OnPropertyChanged(); }
    }
    public string SelectedCourse
    {
        get => _selectedCourse;
        set { _selectedCourse = value; OnPropertyChanged(); }
    }

    public string DepartmentKeyword
    {
        get => _departmentKeyword;
        set { _departmentKeyword = value; OnPropertyChanged(); }
    }

    public string SelectedDynamicAttribute
    {
        get => _selectedDynamicAttribute;
        set { _selectedDynamicAttribute = value; OnPropertyChanged(); }
    }

    public IXMLSearchStrategy SelectedStrategy
    {
        get => _selectedStrategy;
        set { _selectedStrategy = value; OnPropertyChanged(); }
    }

    public ICommand SelectXmlFileCommand { get; }
    public ICommand SelectXslFileCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand TransformToHtmlCommand { get; }
    public ICommand ExitCommand { get; }

    public MainViewModel()
    {
        Strategies = new ObservableCollection<IXMLSearchStrategy>
        {
            new SaxSearchStrategy(),
            new DomSearchStrategy(),
            new LinqToXmlSearchStrategy()
        };
        _selectedStrategy = Strategies[0];

        SelectXmlFileCommand = new Command(async () => await PickXmlFile());
        SelectXslFileCommand = new Command(async () => await PickXslFile());
        SearchCommand = new Command(PerformSearch);
        ClearCommand = new Command(ClearFields);
        TransformToHtmlCommand = new Command(async () => await TransformHtml());
        ExitCommand = new Command(async () => await ConfirmAndExit());
    }

    private async Task PickXmlFile()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Choose XML-file of dormitory"
        });

        if (result != null)
        {
            XmlPath = result.FullPath;
            LoadDynamicAttributes(XmlPath);
        }
    }

    private async Task PickXslFile()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Choose XSL-file of transformation"
        });

        if (result != null)
        {
            XslPath = result.FullPath;
        }
    }

    private void LoadDynamicAttributes(string path)
    {
        DynamicAttributes.Clear();
        try
        {
            var doc = XDocument.Load(path);
            var attributes = doc.Descendants("Student")
                                .Attributes()
                                .Select(a => $"{a.Name.LocalName}='{a.Value}'")
                                .Distinct();

            foreach (var attr in attributes)
            {
                DynamicAttributes.Add(attr);
            }
        }
        catch (Exception ex)
        {
            Application.Current?.MainPage?.DisplayAlert("Error", $"Error with reading attributes: {ex.Message}", "OK");
        }
    }

    private void PerformSearch()
    {
        if (string.IsNullOrEmpty(XmlPath) || !File.Exists(XmlPath))
        {
            Application.Current?.MainPage?.DisplayAlert("Attention", "Please, choose correct XML file.", "OK");
            return;
        }

        var filter = new SearchFilter
        {
            Faculty = SelectedFaculty,
            Course = SelectedCourse,
            DepartmentKeyword = DepartmentKeyword,
            SelectedAttributeValue = SelectedDynamicAttribute
        };

        var results = SelectedStrategy.Analyze(XmlPath, filter);
        GeneratedQuery = SelectedStrategy.LastQuery;

        SearchResults.Clear();
        foreach (var item in results)
        {
            SearchResults.Add(item);
        }
    }

    private void ClearFields()
    {
        SelectedFaculty = string.Empty;
        SelectedCourse = string.Empty;
        DepartmentKeyword = string.Empty;
        SelectedDynamicAttribute = string.Empty;
        SearchResults.Clear();
        GeneratedQuery = string.Empty;
    }

    private async Task TransformHtml()
    {
        if (string.IsNullOrEmpty(XmlPath) || string.IsNullOrEmpty(XslPath))
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", "Enter the paths to XML and XSL files!", "OK");
            return;
        }

        try
        {
            string htmlPath = XMLTransformService.TransformToHtml(XmlPath, XslPath);
            await Application.Current!.MainPage!.DisplayAlert("Success", $"HTML file generated successfully:\n{htmlPath}", "OK");
            await Launcher.Default.OpenAsync(new OpenFileRequest("View HTML", new ReadOnlyFile(htmlPath)));
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Transformation error", ex.Message, "OK");
        }
    }

    private async Task ConfirmAndExit()
    {
        bool answer = await Application.Current!.MainPage!.DisplayAlert(
            "Exit confirmation",
            "Do you really want to exit the program?",
            "Yes",
            "No");

        if (answer)
        {
            Application.Current.Quit();
        }
    }
}
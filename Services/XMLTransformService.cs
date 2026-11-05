using System.Xml.Xsl;

namespace lab2.Services;

public class XMLTransformService
{
    public static string TransformToHtml(string xmlPath, string xslPath)
    {
        var transform = new XslCompiledTransform();
        transform.Load(xslPath);

        string htmlPath = Path.ChangeExtension(xmlPath, ".html");
        using (var writer = new StreamWriter(htmlPath))
        {
            transform.Transform(xmlPath, null, writer);
        }
        return htmlPath;
    }
}
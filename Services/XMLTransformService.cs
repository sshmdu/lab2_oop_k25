using System.Xml.Xsl;

namespace lab2.Services;

public class XMLTransformService
{
    public static string TransformToHtml(string xmlPath, string xslPath, IEnumerable<string> ids, string query)
    {
        var transform = new XslCompiledTransform();
        transform.Load(xslPath);

        var args = new XsltArgumentList();
        args.AddParam("ids", "", "," + string.Join(",", ids) + ",");
        args.AddParam("query", "", query);

        string htmlPath = Path.ChangeExtension(xmlPath, ".html");
        using (var writer = new StreamWriter(htmlPath))
        {
            transform.Transform(xmlPath, args, writer);
        }
        return htmlPath;
    }
}
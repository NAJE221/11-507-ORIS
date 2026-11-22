using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.UnitTests.Renders;

[TestClass]
public class RenderToFileTests
{
    private static IHtmlTemplateRenderer _renderer = null!;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        _renderer = new HtmlTemplateRenderer();
    }

    [TestMethod]
    public void RenderToFile_ValidTemplate_WritesRenderedContent()
    {
        // Arrange
        var from = Path.Combine(Directory.GetCurrentDirectory(), "templateFrom.html");
        var to = Path.Combine(Directory.GetCurrentDirectory(), "templateTo.html");

        File.WriteAllText(from, "<h1>Привет, ${user.Name}</h1>");

        // Act
        _renderer.RenderToFile(from, to, new { Name = "Alex" });

        // Assert
        Assert.AreEqual("<h1>Привет, Alex</h1>", File.ReadAllText(to));
    }

    [TestMethod]
    public void RenderToFile_ExistingTarget_OverwritesContent()
    {
        // Arrange
        var from = Path.Combine(Directory.GetCurrentDirectory(), "overwriteFrom.html");
        var to = Path.Combine(Directory.GetCurrentDirectory(), "overwriteTo.html");

        File.WriteAllText(from, "<h1>${user.Name}</h1>");
        File.WriteAllText(to, "старое содержимое, которое должно исчезнуть");

        // Act
        _renderer.RenderToFile(from, to, new { Name = "Alex" });

        // Assert
        Assert.AreEqual("<h1>Alex</h1>", File.ReadAllText(to));
    }

    [TestMethod]
    public void RenderToFile_SourceNotFound_ThrowsException()
    {
        // Arrange
        var from = Path.Combine(Directory.GetCurrentDirectory(), "missingSource.html");
        var to = Path.Combine(Directory.GetCurrentDirectory(), "missingTarget.html");

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(
            () => _renderer.RenderToFile(from, to, new { Name = "Alex" }));
    }

    [TestCleanup]
    public void Cleanup()
    {
        foreach (var name in new[]
                 {
                     "templateFrom.html", "templateTo.html",
                     "overwriteFrom.html", "overwriteTo.html",
                     "missingTarget.html"
                 })
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), name);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.UnitTests.Renders;

[TestClass]
public class RenderFromFileTests
{
    private static IHtmlTemplateRenderer _renderer = null!;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        _renderer = new HtmlTemplateRenderer();
    }
    
    [TestMethod]
    public void RenderFromFile_ValidFile_RendersTemplate()
    {        
        // Arrange
        var stringPath = Path.Combine(Directory.GetCurrentDirectory(), "template.html");
        
        File.WriteAllText(stringPath, "<h1>Привет, ${user.Name}</h1>");
        
        var user = new
        {
            Name = "Artem"
        };
        
        // Act
        var result = _renderer.RenderFromFile(stringPath, user);
        
        // Assert
        Assert.AreEqual("<h1>Привет, Artem</h1>" , result);
    }
    
    [TestMethod]
    public void RenderFromFile_FileNotFound_ThrowsException()
    {
        // Arrange
        var path = Path.Combine(Directory.GetCurrentDirectory(), "not_exists.html");

        // Act & Assert
        Assert.ThrowsExactly<FileNotFoundException>(
            () => _renderer.RenderFromFile(path, new { Name = "Artem" }));
    }
    
    [TestMethod]
    public void RenderFromFile_ComplexTemplate_RendersIfAndForeach()
    {
        // Arrange3
        var path = Path.Combine(Directory.GetCurrentDirectory(), "complex.html");
        File.WriteAllText(path,
            "$if(model.IsLoggedIn)$foreach(var card in model.Cards)<p>${card.Name}</p>$endfor$endif");

        var model = new
        {
            IsLoggedIn = true,
            Cards = new[] { new { Name = "Мурлок" }, new { Name = "Йети" } }
        };

        // Act
        var result = _renderer.RenderFromFile(path, model);

        // Assert
        Assert.AreEqual("<p>Мурлок</p><p>Йети</p>", result);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        foreach (var name in new[] { 
                     "template.html", "complex.html"})
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), name);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.UnitTests.Renders;

[TestClass]
public class RenderFromStringTests
{
    private static IHtmlTemplateRenderer _renderer = null!;

    [ClassInitialize]
    public static void Setup(TestContext context)
    {
        _renderer = new HtmlTemplateRenderer();
    }

    [TestMethod]
    public void RenderFromString_ValidProperty_ReplacesPlaceholder()
    {
        // Arrange
        var template = "<div>${car.Name}</div>";
        var car = new { Name = "Car" };

        // Act
        var result = _renderer.RenderFromString(template, car);

        // Assert
        Assert.AreEqual("<div>Car</div>", result);
    }


    [TestMethod]
    public void RenderFromString_MultipleProperties_ReplacesAllPlaceholders()
    {
        // Arrange
        var template = "<div>${car.Name} отличная машина за ${car.Cost} руб</div>";

        var car = new { Name = "Mazda", Cost = 450000 };

        // Act
        var result = _renderer.RenderFromString(template, car);

        // Assert
        Assert.AreEqual("<div>Mazda отличная машина за 450000 руб</div>", result);
    }

    [TestMethod]
    public void RenderFromString_RepeatedProperty_ReplacesAllOccurrences()
    {
        // Arrange
        var template = "<div>${user.Name} лучший игрок. ${user.Name} выиграл!!!</div>";
        var user = new { Name = "Umar" };

        // Act
        var result = _renderer.RenderFromString(template, user);

        Assert.AreEqual("<div>Umar лучший игрок. Umar выиграл!!!</div>", result);
    }

    [TestMethod]
    public void RenderFromString_NestedProperties_ReplacesPlaceholder()
    {
        // Arrange
        var template =
            "<div>${model.Title}. Автор: ${model.Author.Name}. Город: ${model.Author.Address.City}</div>";

        var model = new
        {
            Title = "Чистейший код",
            Author = new
            {
                Name = "Umar",
                Address = new { City = "Kazan" }
            }
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<div>Чистейший код. Автор: Umar. Город: Kazan</div>", result);
    }

    [TestMethod]
    public void RenderFromString_EmptyTemplate_ReturnsEmptyString()
    {
        // Arrange
        string template = "";
        var model = new { Name = "Car" };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void RenderFromString_TrueCondition_RendersIfBlock()
    {
        // Arrange
        var template = "$if(model.IsAvailable) Доступно$endif";

        var model = new { IsAvailable = true };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("Доступно", result);
    }

    [TestMethod]
    public void RenderFromString_FalseCondition_SkipsIfBlock()
    {
        // Arrange
        var template = "$if(model.IsAvailable)Доступно$endif";

        var model = new { IsAvailable = false };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void RenderFromString_TrueConditionWithElse_RendersIfBranch()
    {
        // Arrange
        var template = "$if(model.IsAdmin)Администратор$else Пользователь$endif";

        var model = new { IsAdmin = true };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("Администратор", result);
    }

    [TestMethod]
    public void RenderFromString_FalseConditionWithElse_RendersElseBranch()
    {
        // Arrange
        var template = "$if(model.IsAdmin)Администратор$else Пользователь$endif";

        var model = new { IsAdmin = false };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("Пользователь", result);
    }

    [TestMethod]
    public void RenderFromString_ForeachWithMultipleItems_RendersAllItems()
    {
        // Arrange
        var template = "$foreach(var card in model.Cards)<div>${card.Name}</div>$endfor";

        var model = new
        {
            Cards = new[]
            {
                new { Name = "Огненный шар" },
                new { Name = "Ледяная стрела" },
                new { Name = "Огненный элементаль" }
            }
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<div>Огненный шар</div><div>Ледяная стрела</div><div>Огненный элементаль</div>", result);
    }

    [TestMethod]
    public void RenderFromString_ForeachWithEmptyCollection_RendersNoItems()
    {
        // Arrange
        var template = "$foreach(var card in model.Cards)<div>${card.Name}</div>$endfor";

        var model = new
        {
            Cards = Array.Empty<object>()
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void RenderFromString_NestedIfConditions_RendersCorrectContent()
    {
        // Arrange
        var template = "$if(model.IsLoggedIn)$if(model.IsAdmin)Панель администратора$endif$endif";

        var model = new
        {
            IsLoggedIn = true,
            IsAdmin = true
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("Панель администратора", result);
    }

    [TestMethod]
    public void RenderFromString_NestedForeach_RendersAllItems()
    {
        // Arrange
        var template =
            "$foreach(var category in model.Categories)<h2>${category.Name}</h2>$foreach(var card in category.Cards)<p>${card.Name}</p>$endfor$endfor";
        ;

        var model = new
        {
            Categories = new[]
            {
                new
                {
                    Name = "Заклинания",
                    Cards = new[]
                    {
                        new { Name = "Огненный шар" },
                        new { Name = "Ледяная стрела" }
                    }
                },
                new
                {
                    Name = "Существа",
                    Cards = new[]
                    {
                        new { Name = "Мурлок" },
                        new { Name = "Йети" }
                    }
                }
            }
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual(
            "<h2>Заклинания</h2><p>Огненный шар</p><p>Ледяная стрела</p><h2>Существа</h2><p>Мурлок</p><p>Йети</p>",
            result);
    }

    [TestMethod]
    public void RenderFromString_IfWithForeach_RendersCorrectContent()
    {
        // Arrange
        var template = "$if(model.IsLoggedIn)$foreach(var card in model.Cards)<p>${card.Name}</p>$endfor$endif";

        var model = new
        {
            IsLoggedIn = true,

            Cards = new[]
            {
                new { Name = "Огненный шар" },
                new { Name = "Ледяная стрела" },
                new { Name = "Огненный элементаль" }
            }
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("<p>Огненный шар</p><p>Ледяная стрела</p><p>Огненный элементаль</p>", result);
    }

    [TestMethod]
    public void RenderFromString_IfNotWithForeach_RendersCorrectContent()
    {
        // Arrange
        var template = "$if(model.IsLoggedIn)$foreach(var card in model.Cards)<p>${model.Name}</p>$endfor$endif";

        var model = new
        {
            IsLoggedIn = false,
            Cards = Array.Empty<object>()
        };

        // Act
        var result = _renderer.RenderFromString(template, model);

        // Assert
        Assert.AreEqual("", result);
    }
}
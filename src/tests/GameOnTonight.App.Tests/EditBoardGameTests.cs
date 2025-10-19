using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using GameOnTonight.App.Pages;
using GameOnTonight.App.Services;
using GameOnTonight.RestClient.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GameOnTonight.App.Tests;

public class EditBoardGameTests : TestContext
{
    private static void RegisterCommonServices(TestContext ctx, Mock<IBoardGamesService> boardGamesMock)
    {
        ctx.Services.AddSingleton<IErrorService, ErrorService>();
        ctx.Services.AddSingleton<IBoardGamesService>(boardGamesMock.Object);
        // NavigationManager is provided by bUnit automatically.
    }

    private static void FillValidForm(IRenderedComponent<EditBoardGame> cut)
    {
        // Name
        cut.InvokeAsync(() => cut.Find("input").Change("7 Wonders"));

        // MinPlayers
        cut.InvokeAsync(() => cut.FindAll("input")[1].Change("2"));

        // MaxPlayers
        cut.InvokeAsync(() => cut.FindAll("input")[2].Change("5"));

        // DurationMinutes
        cut.InvokeAsync(() => cut.FindAll("input")[3].Change("30"));

        // GameType
        cut.InvokeAsync(() => cut.FindAll("input")[4].Change("Stratégie"));
    }

    [Fact]
    public async Task When_server_returns_validation_errors_they_are_shown_under_fields()
    {
        // Arrange
        var svcMock = new Mock<IBoardGamesService>(MockBehavior.Strict);
        svcMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateBoardGameCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(BuildValidationException());

        RegisterCommonServices(this, svcMock);

        var cut = RenderComponent<EditBoardGame>();
        FillValidForm(cut);

        // Act
        await cut.Find("form").SubmitAsync();

        // Assert
        var messages = cut.FindAll(".validation-message").Select(e => e.TextContent.Trim()).ToList();
        Assert.Contains("Le nom doit être unique.", messages);
        Assert.Contains("Le maximum doit être supérieur ou égal au minimum.", messages);
        // No global alert-danger should be shown when field errors exist
        Assert.Empty(cut.FindAll(".alert.alert-danger"));

        svcMock.VerifyAll();
    }

    [Fact]
    public async Task When_server_call_succeeds_no_field_errors_are_displayed()
    {
        // Arrange
        var svcMock = new Mock<IBoardGamesService>(MockBehavior.Strict);
        svcMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateBoardGameCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BoardGameViewModel { Name = "7 Wonders", MinPlayers = 2, MaxPlayers = 5, DurationMinutes = 30, GameType = "Stratégie" });

        RegisterCommonServices(this, svcMock);

        var cut = RenderComponent<EditBoardGame>();
        FillValidForm(cut);

        // Act
        await cut.Find("form").SubmitAsync();

        // Assert
        Assert.Empty(cut.FindAll(".validation-message"));
        Assert.Empty(cut.FindAll(".alert.alert-danger"));

        svcMock.VerifyAll();
    }

    private static HttpValidationProblemDetails BuildValidationException()
    {
        var ex = new HttpValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = new HttpValidationProblemDetails_errors()
        };

        // The Kiota-generated model uses AdditionalData to carry the error dictionary
        ex.Errors!.AdditionalData["Name"] = new object[] { "Le nom doit être unique." };
        ex.Errors!.AdditionalData["MaxPlayers"] = new object[] { "Le maximum doit être supérieur ou égal au minimum." };
        return ex;
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using Bunit.Rendering;
using GameOnTonight.App.Pages.Library;
using GameOnTonight.App.Services;
using GameOnTonight.RestClient.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using Xunit;

namespace GameOnTonight.App.Tests;

public class EditBoardGameTests : BunitContext, IAsyncLifetime
{
    public EditBoardGameTests()
    {
        // Configure JSInterop to ignore all unhandled MudBlazor JS calls
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }

    private void RegisterCommonServices(Mock<IBoardGamesService> boardGamesMock)
    {
        Services.AddMudServices();
        Services.AddSingleton<IErrorService, ErrorService>();
        Services.AddSingleton<IBoardGamesService>(boardGamesMock.Object);
        // NavigationManager is provided by bUnit automatically.
    }

    private IRenderedComponent<ContainerFragment> RenderWithMudProviders()
    {
        // Render MudPopoverProvider first, then EditBoardGame
        return Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<EditBoardGame>(1);
            builder.CloseComponent();
        });
    }

    [Fact]
    public void EditBoardGame_renders_form_with_all_required_fields()
    {
        // Arrange
        var svcMock = new Mock<IBoardGamesService>();
        svcMock
            .Setup(s => s.GetGameTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "Stratégie", "Famille", "Ambiance" });

        RegisterCommonServices(svcMock);

        // Act
        var cut = RenderWithMudProviders();

        // Assert - All expected form elements should be rendered
        Assert.NotNull(cut.Find("form"));
        
        // MudTextField for Name
        var nameField = cut.FindAll(".mud-input-control").FirstOrDefault(e => e.TextContent.Contains("Nom du jeu"));
        Assert.NotNull(nameField);
        
        // MudNumericField for MinPlayers and MaxPlayers
        var minPlayersField = cut.FindAll(".mud-input-control").FirstOrDefault(e => e.TextContent.Contains("Joueurs min"));
        Assert.NotNull(minPlayersField);
        
        var maxPlayersField = cut.FindAll(".mud-input-control").FirstOrDefault(e => e.TextContent.Contains("Joueurs max"));
        Assert.NotNull(maxPlayersField);
        
        // MudNumericField for Duration
        var durationField = cut.FindAll(".mud-input-control").FirstOrDefault(e => e.TextContent.Contains("Durée"));
        Assert.NotNull(durationField);
        
        // GameTypes section (now multiple selection) - check for the label text
        var gameTypesLabel = cut.Markup.Contains("Types / Catégories");
        Assert.True(gameTypesLabel);
        
        // Submit button
        var submitButton = cut.Find("button[type='submit']");
        Assert.NotNull(submitButton);
    }

    [Fact]
    public void EditBoardGame_shows_error_alert_when_errorMessage_is_set()
    {
        // Arrange
        var svcMock = new Mock<IBoardGamesService>();
        svcMock
            .Setup(s => s.GetGameTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string>());
        // Simulate a network error when loading by ID
        svcMock
            .Setup(s => s.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("Network error"));

        RegisterCommonServices(svcMock);

        // Act - Render with an ID to trigger loading which will fail
        var cut = Render(builder =>
        {
            builder.OpenComponent<MudPopoverProvider>(0);
            builder.CloseComponent();
            builder.OpenComponent<EditBoardGame>(1);
            builder.AddAttribute(2, "Id", 123);
            builder.CloseComponent();
        });

        // Wait for the async operation to complete and error alert to appear
        cut.WaitForAssertion(() =>
        {
            var errorAlert = cut.FindAll(".mud-alert").FirstOrDefault();
            Assert.NotNull(errorAlert);
        });
    }

    [Fact]
    public void ErrorService_GetFieldErrors_extracts_validation_errors_correctly()
    {
        // Arrange
        var errorService = new ErrorService();
        var validationException = BuildValidationException();

        // Act
        var fieldErrors = errorService.GetFieldErrors(validationException);

        // Assert
        Assert.True(fieldErrors.ContainsKey("Name"));
        Assert.Contains("Le nom doit être unique.", fieldErrors["Name"]);
        Assert.True(fieldErrors.ContainsKey("MaxPlayers"));
        Assert.Contains("Le maximum doit être supérieur ou égal au minimum.", fieldErrors["MaxPlayers"]);
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

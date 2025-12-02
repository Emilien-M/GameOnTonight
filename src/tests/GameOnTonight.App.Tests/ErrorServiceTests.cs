using System;
using System.Collections.Generic;
using GameOnTonight.App.Services;
using GameOnTonight.RestClient.Models;
using Xunit;

namespace GameOnTonight.App.Tests;

public class ErrorServiceTests
{
    private static ErrorService CreateService() => new();

    [Fact]
    public void GetErrorMessage_WithHttpValidationProblemDetails_NoErrors_ReturnsTitleOrMessage()
    {
        // Arrange
        var ex = new HttpValidationProblemDetails
        {
            Title = "A validation error occurred"
            // Errors is null by default
        };
        var service = CreateService();

        // Act
        var message = service.GetErrorMessage(ex);

        // Assert
        Assert.Equal("A validation error occurred", message);
    }

    [Fact]
    public void GetErrorMessage_WithMultipleErrors_ReturnsConcatenatedLines()
    {
        // Arrange
        var ex = new HttpValidationProblemDetails
        {
            Title = "Ignored when errors exist",
            Errors = new HttpValidationProblemDetails_errors()
        };
        // Simulate backend error payload structure
        ex.Errors!.AdditionalData["Name"] = new[] { "Name is required", "Name must be at least 3 characters" };
        ex.Errors!.AdditionalData["Email"] = new List<string> { "Email is invalid" };
        var service = CreateService();

        // Act
        var message = service.GetErrorMessage(ex);

        // Assert - lines are concatenated with \n in the order added
        var expected = string.Join("\n", new[]
        {
            "Name is required",
            "Name must be at least 3 characters",
            "Email is invalid"
        });
        Assert.Equal(expected, message);
    }

    [Fact]
    public void GetFieldErrors_BuildsCaseInsensitiveDictionary()
    {
        // Arrange
        var ex = new HttpValidationProblemDetails
        {
            Errors = new HttpValidationProblemDetails_errors()
        };
        ex.Errors!.AdditionalData["Email"] = new[] { "Email is required", "Email format is invalid" };
        ex.Errors!.AdditionalData["Password"] = new List<object> { "Password is too short" };
        var service = CreateService();

        // Act
        var fieldErrors = service.GetFieldErrors(ex);

        // Assert
        Assert.True(fieldErrors.TryGetValue("email", out var emailErrors)); // case-insensitive key lookup
        Assert.NotNull(emailErrors);
        Assert.Equal(2, emailErrors!.Count);
        Assert.Contains("Email is required", emailErrors);
        Assert.Contains("Email format is invalid", emailErrors);

        Assert.True(fieldErrors.TryGetValue("PASSWORD", out var passwordErrors));
        Assert.Single(passwordErrors!);
        Assert.Equal("Password is too short", passwordErrors![0]);
    }
}

using GameOnTonight.Domain.Exceptions;
using Xunit;

namespace GameOnTonight.Domain.Tests;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessageAndPropertyName_ShouldCreateSingleError()
    {
        // Arrange & Act
        var exception = new DomainException("Value is required", "Name");

        // Assert
        Assert.Single(exception.Errors);
        Assert.Equal("Value is required", exception.Errors[0].Message);
        Assert.Equal("Name", exception.Errors[0].Name);
    }

    [Fact]
    public void Constructor_WithDomainError_ShouldCreateSingleError()
    {
        // Arrange
        var error = new DomainError("Invalid value", "Property");
        
        // Act
        var exception = new DomainException(error);

        // Assert
        Assert.Single(exception.Errors);
        Assert.Equal("Invalid value", exception.Errors[0].Message);
        Assert.Equal("Property", exception.Errors[0].Name);
    }

    [Fact]
    public void Constructor_WithMultipleDomainErrors_ShouldCreateMultipleErrors()
    {
        // Arrange
        var errors = new[]
        {
            new DomainError("Error 1", "Prop1"),
            new DomainError("Error 2", "Prop2"),
            new DomainError("Error 3", "Prop3")
        };
        
        // Act
        var exception = new DomainException(errors);

        // Assert
        Assert.Equal(3, exception.Errors.Count);
        Assert.Contains(exception.Errors, e => e.Name == "Prop1" && e.Message == "Error 1");
        Assert.Contains(exception.Errors, e => e.Name == "Prop2" && e.Message == "Error 2");
        Assert.Contains(exception.Errors, e => e.Name == "Prop3" && e.Message == "Error 3");
    }

    [Fact]
    public void Constructor_WithTuples_ShouldCreateMultipleErrors()
    {
        // Arrange
        var errors = new[]
        {
            ("Prop1", "Error 1"),
            ("Prop2", "Error 2")
        };
        
        // Act
        var exception = new DomainException(errors);

        // Assert
        Assert.Equal(2, exception.Errors.Count);
    }
}

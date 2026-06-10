using FluentAssertions;
using UsersApi.Domain.ValueObjects;

namespace UsersApi.Domain.Tests.ValueObjects;

public class UsernameTests
{
    [Theory]
    [InlineData("user123")]
    [InlineData("john_doe")]
    [InlineData("test-user")]
    [InlineData("abc")]
    public void Constructor_ShouldCreate_WhenValid(string validUsername)
    {
        var username = new Username(validUsername);
        username.Value.Should().Be(validUsername);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenEmpty(string? invalidUsername)
    {
        Action act = () => new Username(invalidUsername!);
        act.Should().Throw<ArgumentException>().WithMessage("*Username cannot be empty*");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Constructor_ShouldThrow_WhenTooShort(string shortUsername)
    {
        Action act = () => new Username(shortUsername);
        act.Should().Throw<ArgumentException>().WithMessage("*at least 3*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTooLong()
    {
        var longUsername = new string('a', 51);
        Action act = () => new Username(longUsername);
        act.Should().Throw<ArgumentException>().WithMessage("*cannot exceed 50*");
    }

    [Theory]
    [InlineData("user name")]
    [InlineData("user!name")]
    [InlineData("user.name")]
    [InlineData("user@name")]
    public void Constructor_ShouldThrow_WhenInvalidCharacters(string invalidUsername)
    {
        Action act = () => new Username(invalidUsername);
        act.Should().Throw<ArgumentException>().WithMessage("*letters, numbers*");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        var username = new Username("testuser");
        username.ToString().Should().Be("testuser");
    }
}

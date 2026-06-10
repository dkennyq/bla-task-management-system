using FluentAssertions;
using UsersApi.Domain.ValueObjects;

namespace UsersApi.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co.uk")]
    [InlineData("a@b.cd")]
    public void Constructor_ShouldCreate_WhenValid(string validEmail)
    {
        var email = new Email(validEmail);
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ShouldThrow_WhenEmpty(string? invalidEmail)
    {
        Action act = () => new Email(invalidEmail!);
        act.Should().Throw<ArgumentException>().WithMessage("*Email cannot be empty*");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    public void Constructor_ShouldThrow_WhenInvalidFormat(string invalidEmail)
    {
        Action act = () => new Email(invalidEmail);
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid email format*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTooLong()
    {
        var longEmail = new string('a', 250) + "@b.com";
        Action act = () => new Email(longEmail);
        act.Should().Throw<ArgumentException>().WithMessage("*cannot exceed 255*");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        var email = new Email("test@example.com");
        email.ToString().Should().Be("test@example.com");
    }
}

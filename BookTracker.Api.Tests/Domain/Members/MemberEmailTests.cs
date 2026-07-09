using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain;

public class MemberEmailTests
{
    [Fact]
    public void MemberEmailAcceptsValidName()
    {
        var email = new MemberEmail("Freddy.mercury@hotmail.com");

        Assert.Equal("Freddy.mercury@hotmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailTrimsValue()
    {
        var email = new MemberEmail("  Freddy.mercury@hotmail.com  ");

        Assert.Equal("Freddy.mercury@hotmail.com", email.Value);
    }

    [Fact]
    public void MemberEmailRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("   "));

        Assert.Equal("Email is required.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsEmailLongerThan200Characters()
    {
        var tooLong = new string('x', 201);

        var exception = Assert.Throws<DomainException>(() => new MemberEmail(tooLong));

        Assert.Equal("Email cannot be longer than 200 characters.", exception.Message);
    }

    [Fact]
    public void MemberEmailRejectsEmailWithoutApenstaartje()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberEmail("Freddy.mercury#hotmail.com"));

        Assert.Equal("Email is invalid.", exception.Message);
    }
}
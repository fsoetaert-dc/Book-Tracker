using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.Domain;

public class MemberNameTests
{
    [Fact]
    public void MemberNameAcceptsValidName()
    {
        var name = new MemberName("Freddy Mercury");

        Assert.Equal("Freddy Mercury", name.Value);
    }

    [Fact]
    public void MemberNameTrimsValue()
    {
        var name = new MemberName("  Dan Brown  ");

        Assert.Equal("Dan Brown", name.Value);
    }

    [Fact]
    public void MemberNameRejectsWhitespace()
    {
        var exception = Assert.Throws<DomainException>(() => new MemberName("   "));

        Assert.Equal("Membername is required.", exception.Message);
    }

    [Fact]
    public void MemberNameRejectsNameLongerThan100Characters()
    {
        var tooLong = new string('x', 101);

        var exception = Assert.Throws<DomainException>(() => new MemberName(tooLong));

        Assert.Equal("Membername cannot be longer than 100 characters.", exception.Message);
    }
}
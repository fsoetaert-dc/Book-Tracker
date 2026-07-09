using System.Net;
using BookTracker.Api.Application.GetMemberDetails;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.GetMemberDetails;

public class GetMemberDetailsTests : IntegrationTest
{

    [Fact]
    public async Task GetMemberDetailsReturnsBook()
    {
        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Frank Herbert"),
                    Email = new MemberEmail("Frank.Herbert@hotmail.com")
                });
        });

        var response = await Client.GetAsync("/members/1");

        var member = await response.ReadJsonAs<GetMemberDetailsResponse>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(member);
        Assert.Equal(1, member.Id);
        Assert.Equal("Frank Herbert", member.Name);
        Assert.Equal("Frank.Herbert@hotmail.com", member.Email);
    }

    [Fact]
    public async Task GetMemberDetailsReturnsNotFoundWhenMemberDoesNotExist()
    {
        var response = await Client.GetAsync("/members/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
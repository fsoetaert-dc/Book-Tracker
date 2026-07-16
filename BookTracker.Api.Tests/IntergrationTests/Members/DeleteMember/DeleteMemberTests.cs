using System.Net;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.DeleteMember;

public class DeleteMemberTests : IntegrationTest
{

    [Fact]
    public async Task DeleteMemberRemovesMember()
    {
        var memberId = await AuthenticateAsMember();

        var response = await Client.DeleteAsync($"/members/{memberId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var member = Reader.Query(db => db.Members.Find(memberId));

        Assert.Null(member);
    }

    [Fact]
    public async Task DeleteMemberReturnsForbiddenWhenMemberDoesNotExist() //Change NotFound to Forbidden bc Member can only delete itself
    {
        await AuthenticateAsMember();

        var response = await Client.DeleteAsync("/members/9999");

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }
}
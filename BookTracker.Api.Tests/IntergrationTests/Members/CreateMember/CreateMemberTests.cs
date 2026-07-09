using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.CreateMember;

public class CreateMemberTests : IntegrationTest
{

    [Fact]
    public async Task PostMemberCreatesMember()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Christina Segura",
                Email = "Carson.McCullers@hotmail.com"

            };
        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Christina Segura", created.Name);
        Assert.Equal("Carson.McCullers@hotmail.com", created.Email);

        var member = Reader.Query(context => context.Find<Member>(created.Id));

        Assert.NotNull(member);
        Assert.Equal("Christina Segura", member.Name.Value);
        Assert.Equal("Carson.McCullers@hotmail.com", member.Email.Value);

    }
    [Fact]
    public async Task PostMemberReturnsBadRequestWhenNameIsWhitespace()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "   ",
                Email = "Carson.McCullers@hotmail.com",

            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenEmailIsWhitespace()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Carson McCullers",
                Email = "     "
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                Email = "Carson.McCullers@hotmail.com",
                Password = "1234abcd"
            };
        var response = await Client.PostAsJsonAsync("/members", request);

        var created = await response.ReadJsonAs<CreateMemberResponse>(HttpStatusCode.Created);
        var member = Reader.Query(db =>
            db.Members.Single(current => current.Id == created.Id));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Christina Segura", created.Name);
        Assert.Equal("carson.mccullers@hotmail.com", created.Email);
        Assert.NotEqual("1234abcd", member.PasswordHash);

        var passwordHasher = new PasswordHasher<Member>();

        var result = passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            "1234abcd");

        Assert.Equal(PasswordVerificationResult.Success, result);

        Assert.NotNull(member);
        Assert.Equal("Christina Segura", member.Name.Value);
        Assert.Equal("carson.mccullers@hotmail.com", member.Email.Value);

    }
    [Fact]
    public async Task PostMemberReturnsBadRequestWhenNameIsWhitespace()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "   ",
                Email = "Carson.McCullers@hotmail.com",
                Password = "1234abcd"
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
                Email = "     ",
                Password = "1234abcd"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenPasswordIsWhitespace()
   {
        var request =
            new CreateMemberRequest
            {
                Name = "Carson McCullers",
                Email = "Carson.mccullers@hotmail.com",
                Password = "    "
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostMemberReturnsBadRequestWhenPasswordIsToShort()
    {
        var request =
            new CreateMemberRequest
            {
                Name = "Carson McCullers",
                Email = "Carson.mccullers@hotmail.com",
                Password = "123abc"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostMemberReturnsConflictWhenEmailAlreadyExists()
    {
        var member =
            new CreateMemberRequest
            {
                Name = "Carson McCullers",
                Email = "Carson.mccullers@hotmail.com",
                Password = "1234abcd"
            };

        await Client.PostAsJsonAsync("/members", member);


        var request =
            new CreateMemberRequest
            {
                Name = "Superman",
                Email = "Carson.Mccullers@hotmail.com",
                Password = "abcd1234"
            };

        var response = await Client.PostAsJsonAsync("/members", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
public async Task CreateMemberCreatesRegularMember()
{
    var request =
        new CreateMemberRequest
        {
            Name = "Grace Hopper",
            Email = "grace@example.com",
            Password = "debugging-moth"
        };

    var response =
        await Client.PostAsJsonAsync(
            "/members",
            request);

    var created =
        await response
            .ReadJsonAs<CreateMemberResponse>(
                HttpStatusCode.Created);

    var member =
        Reader.Query(db =>
            db.Members.Find(created.Id));

    Assert.NotNull(member);

    Assert.Equal(
        MemberRole.Member,
        member.Role);
}
}
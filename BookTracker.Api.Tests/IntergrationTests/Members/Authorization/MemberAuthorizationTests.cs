using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Tests.IntegrationTests;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Tests.IntegrationTests.Members.Authorization;

public class MemberAuthorizationTest : IntegrationTest
{
    private Member SeedMember(
    string password = "analytical-engine")
    {
        var member =
            new Member
            {
                Name = new MemberName("Magic Mike"),
                Email = new MemberEmail("magicmike@example.com"),
                PasswordHash = string.Empty
            };

        var passwordHasher = new PasswordHasher<Member>();

        member.PasswordHash =
            passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));
        return member;
    }

    [Fact]
    public async Task CreateMemberDoesNotRequireAuthentication()
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

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Created);
    }

    [Fact]
    public async Task UpdateMemberRequiresAuthentication()
    {
        var member = SeedMember();
        var memberId = member.Id;

        var request =
            new UpdateMemberRequest
            {
                Name = "Ada Byron",
                Email = "ada.byron@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteMemberRequiresAuthentication()
    {
        var member = SeedMember();
        var memberId = member.Id;

        var response =
            await Client.DeleteAsync(
                $"/members/{memberId}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MemberCanUpdateOwnAccount()
    {
        var memberId = await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = "Ada Byron",
                Email = "ada.byron@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{memberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task MemberCannotUpdateAnotherMember()
    {
        var currentMemberId =
            await AuthenticateAsMember();

        var otherMember = SeedMember();
        var otherMemberId = otherMember.Id;

        var request =
            new UpdateMemberRequest
            {
                Name = "Changed Name",
                Email = "changed@example.com"
            };

        var response =
            await Client.PutAsJsonAsync(
                $"/members/{otherMemberId}",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var member =
            Reader.Query(db =>
                db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Magic Mike", member.Name.Value);
        Assert.Equal("magicmike@example.com", member.Email.Value);
    }

    [Fact]
    public async Task MemberCannotDeleteAnotherMember()
    {
        var currentMemberId =
            await AuthenticateAsMember();

        var otherMember = SeedMember();
        var otherMemberId = otherMember.Id;

        var response =
            await Client.DeleteAsync(
                $"/members/{otherMemberId}");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var member =
            Reader.Query(db =>
                db.Members.Find(otherMemberId));

        Assert.NotNull(member);
        Assert.Equal("Magic Mike", member.Name.Value);
        Assert.Equal("magicmike@example.com", member.Email.Value);
    }
}
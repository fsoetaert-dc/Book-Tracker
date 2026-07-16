using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace BookTracker.Api.Tests.IntegrationTests.UpdateMember;

public class UpdateMemberTests : IntegrationTest
{

    [Fact]
    public async Task PutMemberUpdatesMember()
    {
        var memberId = await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = new MemberName("Lisa Kudrow"),
                Email = new MemberEmail("Lisa.kudrow@hotmail.com")
            };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); // mag in principe weg want lijn erboven checkt met een duidelijke foutmelding te geven

        var member = Reader.Query(db => db.Members.Find(memberId));

        Assert.NotNull(member);
        Assert.Equal("Lisa Kudrow", member.Name.Value);
        Assert.Equal("lisa.kudrow@hotmail.com", member.Email.Value);
    }

    [Fact]
    public async Task PutMemberReturnsNotFoundWhenMemberDoesNotExist()
    {
        var memberId = await AuthenticateAsMember();

        var request =
            new UpdateMemberRequest
            {
                Name = new MemberName("Lisa Kudrow"),
                Email = new MemberEmail("Lisa.kudrow@hotmail.com")
            };

        var response = await Client.PutAsJsonAsync("/members/9999", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PutMemberReturnBadRequestWhenEmailIsInvalid()
    {
        var memberId = await AuthenticateAsMember();

        Writer.Seed(db =>
        {
            db.Members.Add(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            });
        });

        var request =
            new UpdateMemberRequest
            {
                Name = "Lisa Kudrow", // door EFCore is dit anders dan normaal ...
                Email = "      "
            };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutMemberReturnBadRequestWhenEmailAlreadyExists()
    {
        var memberId = await AuthenticateAsMember("Cane Oldman", "Cane.oldman@hotmail.com", "analytical-engine");

        Writer.Seed(db =>
        {
            db.Members.Add(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            }
            );
        });

        var request =
            new UpdateMemberRequest
            {
                Name = new MemberName("Cane Oldman"),
                Email = "Cannery.row@hotmail.com"
            };

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task PutMemberReturnsNothingWhenEmailDoesNotChangeAsync()
    {
        var memberId = await AuthenticateAsMember("Cannery Row", "Cannery.row@hotmail.com", "analytical-engine");

        var request =
            new UpdateMemberRequest
            {
                Name = new MemberName("Cane Oldman"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            };

        var updatedMember = Reader.Query(db =>
            db.Members.First(m => m.Id == 1));

        var response = await Client.PutAsJsonAsync($"/members/{memberId}", request);

        Assert.Equal("cannery.row@hotmail.com", updatedMember.Email.Value);
    }

}

using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.GetMemberSummaries;

public class GetAllGetMembersPaged : IntegrationTest
{
    [Fact]
    public async Task GetMemberSummariesReturnsRequestedPage()
    {
        await AuthenticateAsMember(
            role: MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.AddRange(
                new Member
                {
                    Name = new MemberName("Mark Meyers"),
                    Email = new MemberEmail("Mark.meyers@hotmail.com")

                },
                new Member
                {
                    Name = new MemberName("Const Blyat"),
                    Email = new MemberEmail("Const.blyat@hotmail.com")
                },
                new Member
                {
                    Name = new MemberName("Freddy Soet"),
                    Email = new MemberEmail("Freddy.Soet@hotmail.com")
                });
        });

        var result = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>("/members?page=3&pageSize=1");

        Assert.NotNull(result);

        var member = Assert.Single(result.Items);

        Assert.Equal("Const Blyat", member.Name);
        Assert.Equal(3, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(4, result.TotalItems);
        Assert.Equal(4, result.TotalPages);
    }
    [Fact]
    public async Task GetMembersSummariesReturnsEmptyItemsWhenPageIsTooHigh()
    {
        await AuthenticateAsMember(
            role: MemberRole.Administrator);

        Writer.Seed(db =>
        {
            db.Members.Add(
                new Member
                {
                    Name = new MemberName("Mark Meyers"),
                    Email = new MemberEmail("Mark.meyers@hotmail.com")
                });
        });

        var result = await Client.GetFromJsonAsync<PagedResult<MemberSummary>>("/members?page=99&pageSize=10");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(99, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }
}
using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.GetMemberSummaries;

public class GetMemberSummariesTests : IntegrationTest
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetMemberSummariesReturnsMemberSummaries()
    {

        Writer.Seed(db => db.Members.Add(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            }
        ));

        var response = await Client.GetAsync("/members");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(result);

        var memberDetails = Assert.Single(result.Items);
        Assert.Equal("Cannery Row", memberDetails.Name);
        Assert.Equal("Cannery.row@hotmail.com", memberDetails.Email);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesCanSearchByName()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(

            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Belly Row"),
                Email = new MemberEmail("Belly.row@hotmail.com")
            });
        });

        var response = await Client.GetAsync("/members?search=cannery");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Cannery Row", member.Name);
        Assert.Equal("Cannery.row@hotmail.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMembersSummariesCanSearchByEmail()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Belly Row"),
                Email = new MemberEmail("Belly.row@hotmail.com")
            });
        });

        var response = await Client.GetAsync("/members?search=Belly.");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Belly Row", member.Name);
        Assert.Equal("Belly.row@hotmail.com", member.Email);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetMemberSummariesApplyPagingAfterSearch()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Belly Row"),
                Email = new MemberEmail("Belly.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Saint Rows"),
                Email = new MemberEmail("Saint.rows@hotmail.com")
            });
        });

        var response = await Client.GetAsync("/members?search=row@hotmail&page=2&pageSize=1");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        var member = Assert.Single(result.Items);

        Assert.Equal("Belly Row", member.Name);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetMembersSummariesAppliesPagingAfterSearchWithoutSearchResults()
    {
        Writer.Seed(db =>
        {
            db.Members.AddRange(
            new Member
            {
                Name = new MemberName("Cannery Row"),
                Email = new MemberEmail("Cannery.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Belly Row"),
                Email = new MemberEmail("Belly.row@hotmail.com")
            },
            new Member
            {
                Name = new MemberName("Saint Rows"),
                Email = new MemberEmail("Saint.rows@hotmail.com")
            });
        });

        var response = await Client.GetAsync("/books?search=opera&page=2&pageSize=1");

        var result = await response.ReadJsonAs<PagedResult<MemberSummary>>(HttpStatusCode.OK);

        Assert.Equal(2, result.Page); // opgegeven maximum pages
        Assert.Equal(1, result.PageSize); // opgegeven pagesize
        Assert.Equal(0, result.TotalItems); // want geen items voldoen aan de search query
        Assert.Equal(0, result.TotalPages); // want geen items gevonden
    }
}
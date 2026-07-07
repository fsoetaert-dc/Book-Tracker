using System.Net;
using BookTracker.Api.Application;
using BookTracker.Api.Application.GetBookSummaries;
using BookTracker.Api.Domain;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBookSummariesReturnsBookSummaries()
    {

        Writer.Seed(db => db.Books.Add(
            new Book
            {
                Title = new BookTitle("Cannery Row"),
                Author = new AuthorName("John Steinbeck"),
                Year = 1945
            }
        ));

        var response = await Client.GetAsync("/books");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(result);

        var bookInfo = Assert.Single(result.Items);
        Assert.Equal("Cannery Row", bookInfo.Title);
        Assert.Equal("John Steinbeck", bookInfo.Author);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBookSummariesCanSearchByTitle()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=dune");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        var book = Assert.Single(result.Items);

        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBooksSummariesCanSearchByAuthor()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=frank");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        var book = Assert.Single(result.Items);

        Assert.Equal("Dune", book.Title);
        Assert.Equal("Frank Herbert", book.Author);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetBookSummariesApplyPagingAfterSearch()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("Dune Messiah"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1969
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=dune&page=2&pageSize=1");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        var book = Assert.Single(result.Items);

        Assert.Equal("Dune Messiah", book.Title);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetBooksSummariesAppliesPagingAfterSearchWithoutSearchResults()
    {
        Writer.Seed(db =>
        {
            db.Books.AddRange(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                },
                new Book
                {
                    Title = new BookTitle("Dune Messiah"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1969
                },
                new Book
                {
                    Title = new BookTitle("The Big Sleep"),
                    Author = new AuthorName("Raymond Chandler"),
                    Year = 1939
                });
        });

        var response = await Client.GetAsync("/books?search=opera&page=2&pageSize=1");

        var result = await response.ReadJsonAs<PagedResult<BookSummary>>(HttpStatusCode.OK);

        Assert.Equal(2, result.Page); // opgegeven maximum pages
        Assert.Equal(1, result.PageSize); // opgegeven pagesize
        Assert.Equal(0, result.TotalItems); // want geen items voldoen aan de search query
        Assert.Equal(0, result.TotalPages); // want geen items gevonden
    }
}
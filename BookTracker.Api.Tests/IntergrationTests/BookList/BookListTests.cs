using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application;
using BookTracker.Api.Application.BookList;
using BookTracker.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookTracker.Api.Tests.IntegrationTests.BookList;

public class BookListTests : IntegrationTest
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task GetBooksReturnsBooks()
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
        var result = await Client.GetFromJsonAsync<PagedResult<BookInfo>>("/books");

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
}
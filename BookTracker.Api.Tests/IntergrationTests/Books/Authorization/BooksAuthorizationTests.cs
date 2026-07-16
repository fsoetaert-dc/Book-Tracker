using System.Net;
using System.Net.Http.Json;
using BookTracker.Api.Application.CreateBook;
using BookTracker.Api.Application.UpdateBook;
using BookTracker.Api.Domain;
using BookTracker.Api.Tests.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class BookAuthorizationTest : IntegrationTest
{

    [Fact]
    public async Task CreateBookRequiresAuthentication()
    {
        var request =
            new CreateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var response =
            await Client.PostAsJsonAsync(
                "/books",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var count =
            Reader.Query(db => db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task UpdateBookRequiresAuthentication()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var request =
            new UpdateBookRequest
            {
                Title = "Dune",
                Author = "Frank Fuqboi",
                Year = 2006
            };

        var response =
            await Client.PutAsJsonAsync("/books/1", request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var book =
            await Reader.Query(db => db.Books.Where(
                book => book.Title == "Dune").FirstOrDefaultAsync());

        Assert.Equal("Dune", book?.Title.Value);
        Assert.Equal("Frank Herbert", book?.Author.Value);
        Assert.Equal(1965, book?.Year);
    }

    [Fact]
    public async Task DeleteBookRequiresAuthentication()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var response = await Client.DeleteAsync("/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Unauthorized);

        var count =
            Reader.Query(db => db.Books.Count());

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetBooksDoesNotRequireAuthentication()
    {
        var response = await Client.GetAsync("/books");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBookDetailsDoesNotRequireAuthentication()
    {
        Writer.Seed(db =>
        {
            db.Books.Add(
                new Book
                {
                    Title = new BookTitle("Dune"),
                    Author = new AuthorName("Frank Herbert"),
                    Year = 1965
                });
        });

        var response = await Client.GetAsync("/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegularMemberCannotCreateBook()
    {
        await AuthenticateAsMember();

        var request =
            new CreateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var response =
            await Client.PostAsJsonAsync(
                "/books",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var count =
            Reader.Query(db =>
                db.Books.Count());

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task RegularMemberCannotUpdateBook()
    {
        await AuthenticateAsMember();

        Writer.Seed(db =>
            {
                db.Books.Add(
                    new Book
                    {
                        Title = new BookTitle("Space"),
                        Author = new AuthorName("Lance Armstrong"),
                        Year = 1900
                    });
            });

        var request =
            new UpdateBookRequest
            {
                Title = "Dune",
                Author = "Frank Herbert",
                Year = 1965
            };

        var response =
            await Client.PutAsJsonAsync(
                "/books/1",
                request);

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var book =
            await Reader.Query(db =>
                db.Books.SingleOrDefaultAsync(b => b.Title == "Space"));

        Assert.Equal("Lance Armstrong", book?.Author.Value);
        Assert.Equal(1900, book?.Year);
    }

    [Fact]
    public async Task RegularMemberCannotDeleteBook()
    {
        await AuthenticateAsMember();

        Writer.Seed(db =>
            {
                db.Books.Add(
                    new Book
                    {
                        Title = new BookTitle("Space"),
                        Author = new AuthorName("Lance Armstrong"),
                        Year = 1900
                    });
            });

        var response =
            await Client.DeleteAsync(
                "/books/1");

        await response.ShouldHaveStatusCode(
            HttpStatusCode.Forbidden);

        var count =
            Reader.Query(db => db.Books.Count());
        Assert.Equal(1, count);
    }
}
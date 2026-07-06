using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.BookList;

public class GetBookListQuery(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<BookInfo>> Execute()
    {
        return await dbContext.Books
            .AsNoTracking() // dit zorgt dat EF Core de data niet bijhoud om er later iets mee te doen, enkel lezen is hier voldoende
            .Select(book =>
                new BookInfo
                {
                    Id = book.Id,
                    Title = book.Title.Value,
                    Author = book.Author.Value
                })
            .ToListAsync();
    }
}
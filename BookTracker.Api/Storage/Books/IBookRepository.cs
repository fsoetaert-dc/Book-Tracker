using BookTracker.Api.Domain;
using BookTracker.Api.Storage.Books;

namespace BookTracker.Api.Storage;


public interface IBookRepository
{
    Task<UpdateBookResult> UpdateAsync(Book book, Guid expectedVersion);
    Task<Book> AddAsync(Book book);
    Task<bool> DeleteAsync(int id);
}
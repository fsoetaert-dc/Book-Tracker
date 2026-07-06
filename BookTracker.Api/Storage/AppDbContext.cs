using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options) // erft van de base classe
{
    public DbSet<Book> Books => Set<Book>(); // books is een tabel van book entiteiten in de database

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(book =>
        {
            book.Property(b => b.Title)
                .HasConversion(
                    title => title.Value,
                    value => new BookTitle(value))
                .HasMaxLength(BookTitle.MaxLength);

            book.Property(b => b.Author)
                .HasConversion(
                    author => author.Value,
                    value => new AuthorName(value))
                .HasMaxLength(AuthorName.MaxLength);
                
        });
    }
}
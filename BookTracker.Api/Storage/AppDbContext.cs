using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options) // erft van de base classe
{
    public DbSet<Book> Books => Set<Book>(); // books is een tabel van book entiteiten in de database
    public DbSet<Member> Members => Set<Member>(); // idem books

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

        modelBuilder.Entity<Member>(member =>
        {
            member.Property(b => b.Name)
                .HasConversion(
                    name => name.Value,
                    value => new MemberName(value))
                .HasMaxLength(MemberName.MaxLength);

            member.Property(b => b.Email)
                .HasConversion(
                    email => email.Value,
                    value => new MemberEmail(value))
                .HasMaxLength(MemberEmail.MaxLength);
            member.HasIndex(current => current.Email)
                .IsUnique();
        });
    }
}
using BookTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Storage;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options) // erft van de base classe
{
    public DbSet<Book> Books => Set<Book>(); // books is een tabel van book entiteiten in de database
}
namespace BookTracker.Api.Tests.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;

public abstract class IntegrationTest : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();

    protected HttpClient Client { get; }

    protected EfReader Reader { get; }

    protected EfWriter Writer { get; }

    protected IntegrationTest()
    {
        Client = factory.CreateClient();
        Reader = factory.GetReader();
        Writer = factory.GetWriter();
    }

    public void Dispose()
    {
        Client.Dispose();
        factory.Dispose();
    }

    protected async Task<int> AuthenticateAsMember(
    string name = "Ada Lovelace",
    string email = "ada@example.com",
    string password = "analytical-engine",
    MemberRole role = MemberRole.Member)
    {
        var member =
            new Member
            {
                Name = new MemberName(name),
                Email = new MemberEmail(email),
                PasswordHash = string.Empty,
                Role = role
            };

        var passwordHasher = new PasswordHasher<Member>();

        member.PasswordHash =
            passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));

        var request =
            new LoginRequest
            {
                Email = email,
                Password = password
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        var login =
            await response.ReadJsonAs<LoginResponse>(
                HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return member.Id;
    }
}
using BookTracker.Api.Application.GetMemberSummaries;
using BookTracker.Api.Application.CreateMember;
using BookTracker.Api.Application.DeleteMember;
using BookTracker.Api.Application.GetMemberDetails;
using BookTracker.Api.Application.UpdateMember;
using BookTracker.Api.Domain;
using BookTracker.Api.Application.Members;
using System.Security.Claims;
using BookTracker.Api.Security;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Domain.Actors;

namespace BookTracker.Api.Endpoints;

public static class MemberEndpoints
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/members", GetMemberSummaries)
            .RequireAuthorization();

        app.MapGet("/members/{id:int}", GetMemberDetails)
            .RequireAuthorization();

        app.MapPost("/members", CreateMember);

        app.MapPut("/members/{id:int}", UpdateMember).RequireAuthorization();

        app.MapDelete("/members/{id:int}", DeleteMember).RequireAuthorization();

        return app;
    }

    public static async Task<IResult> GetMemberSummaries(
        [AsParameters]
        GetMemberSummariesRequest request,
        ClaimsPrincipal principal,
        GetMemberSummariesQueryHandler query)
    {
        try
        {
            var actor = principal.ToActor();

            var response = await query.Execute(actor, request);

            return Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    public static async Task<IResult> GetMemberDetails(
        ClaimsPrincipal principal,
        int id,
        GetMemberDetailsQueryHandler query)
    {
        try
        {
            var actor = principal.ToActor();

            var response = await query.Execute(actor, id);

            if (response is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(response);
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }

    public static async Task<IResult> CreateMember(CreateMemberRequest request, CreateMemberCommandHandler handler)
    {
        try
        {
            var response = await handler.Execute(request);
            return Results.Created($"/members/{response.Id}", response);
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
    }

    public static async Task<IResult> UpdateMember(
        int id,
        UpdateMemberRequest request,
        ClaimsPrincipal principal,
        UpdateMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var updated = await handler.Execute(actor, id, request);
            if (!updated)
            {
                return Results.NotFound();
            }
            return Results.NoContent();
        }
        catch (MemberEmailAlreadyExistsException exception)
        {
            return Results.Conflict(new { error = exception.Message });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(new { error = exception.Message });
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }

    }

    public static async Task<IResult> DeleteMember(
        int id,
        ClaimsPrincipal principal,
        DeleteMemberCommandHandler handler)
    {
        try
        {
            var actor = principal.ToActor();
            var deleted = await handler.Execute(actor, id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (ForbiddenOperationException)
        {
            return Results.Forbid();
        }
    }
}
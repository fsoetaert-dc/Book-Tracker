using BookTracker.Api.Application.Members;
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage;

namespace BookTracker.Api.Application.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateMemberRequest request)
    {
        MemberPermissions.EnsureCanManage(actor, id);

        var name = new MemberName(request.Name);
        var email = new MemberEmail(request.Email);

        var member =
            new Member
            {
                Id = id,
                Name = name,
                Email = email,
            };

        if (await memberRepository.EmailExistsAsync(email, id))
        {
            throw new MemberEmailAlreadyExistsException();
        }

        return await memberRepository.UpdateAsync(member);
    }
}
using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Storage;


public interface IMemberRepository
{
    Task<bool> UpdateAsync(Member member);
    Task<Member> AddAsync(Member member);
    Task<bool> DeleteAsync(int id);
}
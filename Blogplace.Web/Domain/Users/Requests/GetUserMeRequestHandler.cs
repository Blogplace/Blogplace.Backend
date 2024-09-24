using Blogplace.Web.Auth;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Users.Requests;

public record GetUserMeResponse(UserDto User);
public record GetUserMeRequest() : IRequest<GetUserMeResponse>;
public class GetUserMeRequestHandler(IUsersRepository repository, ISessionStorage sessionStorage) : IRequestHandler<GetUserMeRequest, GetUserMeResponse>
{
    public async Task<GetUserMeResponse> Handle(GetUserMeRequest request, CancellationToken cancellationToken)
    {
        var id = sessionStorage.UserId;
        var user = await repository.Get(id);
        var dto = new UserDto(user.Id, user.Username, user.CreatedAt);
        return new GetUserMeResponse(dto);
    }
}

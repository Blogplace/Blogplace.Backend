using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Users.Requests;

public record GetUserByEmailResponse(UserDto? User);
public record GetUserByEmailRequest(string Email) : IRequest<GetUserByEmailResponse>;

public class GetUserByEmailRequestHandler(IUsersRepository repository) : IRequestHandler<GetUserByEmailRequest, GetUserByEmailResponse>
{
    public async Task<GetUserByEmailResponse> Handle(GetUserByEmailRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByEmail(request.Email);
        var dto = user == null ? null : new UserDto(user.Id, user.Username, user.CreatedAt);
        return new GetUserByEmailResponse(dto);
    }
}

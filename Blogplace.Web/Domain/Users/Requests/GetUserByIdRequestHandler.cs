using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Users.Requests;

public record GetUserByIdResponse(UserDto User);
public record GetUserByIdRequest(Guid Id) : IRequest<GetUserByIdResponse>;
public class GetUserByIdRequestHandler(IUsersRepository repository) : IRequestHandler<GetUserByIdRequest, GetUserByIdResponse>
{
    public async Task<GetUserByIdResponse> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.Get(request.Id);
        var dto = new UserDto(user.Id, user.Username, user.CreatedAt);
        return new GetUserByIdResponse(dto);
    }
}

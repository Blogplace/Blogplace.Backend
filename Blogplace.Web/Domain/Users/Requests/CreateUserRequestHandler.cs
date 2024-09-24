using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Users.Requests;

public record CreateUserResponse(Guid Id);
public record CreateUserRequest(string Email) : IRequest<CreateUserResponse>;
public class CreateUserRequestHandler(IUsersRepository repository, IEventLogger logger) : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email);
        await repository.Add(user);
        logger.UserCreated(user.Id);
        return new CreateUserResponse(user.Id);
    }
}

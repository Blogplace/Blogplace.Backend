using Blogplace.Web.Commons;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Configuration;
using Blogplace.Web.Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Options;

namespace Blogplace.Web.Domain.Users.Requests;

public record CreateUserResponse(Guid Id);
public record CreateUserRequest(string Email) : IRequest<CreateUserResponse>;
public class CreateUserRequestHandler(
    IUsersRepository repository,
    IEventLogger logger,
    IOptions<PermissionsOptions> permissionsOptions
    ) : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var defaultPermissions = permissionsOptions.Value.GetDefault();
        var user = new User(request.Email, defaultPermissions);
        await repository.Add(user);
        logger.UserCreated(user.Id);
        return new CreateUserResponse(user.Id);
    }
}

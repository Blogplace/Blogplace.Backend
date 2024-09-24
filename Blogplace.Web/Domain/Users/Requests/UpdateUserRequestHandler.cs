using Blogplace.Web.Auth;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Infrastructure.Database;
using MediatR;

namespace Blogplace.Web.Domain.Users.Requests;

public record UpdateUserRequest(string? NewUsername) : IRequest;
public class UpdateUserRequestHandler(ISessionStorage sessionStorage, IUsersRepository repository, IEventLogger logger) : IRequestHandler<UpdateUserRequest>
{
    public async Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var isChanged = false;

        var id = sessionStorage.UserId;
        var user = await repository.Get(id);

        if (!string.IsNullOrWhiteSpace(request.NewUsername))
        {
            user.Username = request.NewUsername;
            isChanged = true;
        }

        if (isChanged)
        {
            await repository.Update(user);
            logger.UserUpdatedProfile(user.Id);
        }
    }
}
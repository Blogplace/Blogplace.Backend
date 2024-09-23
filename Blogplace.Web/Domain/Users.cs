using Blogplace.Web.Auth;
using Blogplace.Web.Infrastructure.Database;
using MediatR;
using System.Text.RegularExpressions;

namespace Blogplace.Web.Domain;

public class User(string email)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Email { get; } = email;
    //Default username = part of email before last @ sign
    //testuser@example.com => testuser
    public string Username { get; set; } = Regex.Match(email, @"^(?<Name>.*)@[^@]+$").Groups["Name"].Value;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public record UserDto(Guid Id, string Username, DateTime CreatedAt);

public record CreateUserResponse(Guid Id);
public record CreateUserRequest(string Email) : IRequest<CreateUserResponse>;
public class CreateUserRequestHandler(IUsersRepository repository) : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email);
        await repository.Add(user);
        return new CreateUserResponse(user.Id);
    }
}

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

public record UpdateUserRequest(string? NewUsername) : IRequest;
public class UpdateUserRequestHandler(ISessionStorage sessionStorage, IUsersRepository repository) : IRequestHandler<UpdateUserRequest>
{
    public async Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var isChanged = false;

        var id = sessionStorage.UserId;
        var user = await repository.Get(id);

        if(!string.IsNullOrWhiteSpace(request.NewUsername))
        {
            user.Username = request.NewUsername;
            isChanged = true;
        }

        if(isChanged)
        {
            await repository.Update(user);
        }
    }
}
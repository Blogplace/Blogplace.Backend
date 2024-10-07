namespace Blogplace.Web.Domain.Articles;

public record Tag
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }

    public Tag(string name)
    {
        //todo validation instead formatting
        this.Name = name.ToLower().Replace(" ", string.Empty);
    }
}

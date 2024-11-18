namespace Blogplace.Web.Background.Jobs;

public class ImportBlogArticlesJob : IJob
{
    public Task Run()
    {
        return Task.CompletedTask;
    }
}

public interface IJob
{
    Task Run();
}
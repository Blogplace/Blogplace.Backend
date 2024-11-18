namespace Blogplace.Web.Background.Jobs;

public interface IJob
{
    Task Run();
}
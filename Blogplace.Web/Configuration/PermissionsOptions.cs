using Blogplace.Web.Commons;

namespace Blogplace.Web.Configuration;

public class PermissionsOptions
{
    public List<CommonPermissionsEnum> DefaultPermissions { get; set; } = [];
}

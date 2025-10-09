using Krafter.UI.Web.Client.Infrastructure.Services;

namespace Krafter.UI.Web.Services
{
    public class FormFactorServer : IFormFactor
    {
        public string GetFormFactor()
        {
            return "Web";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}

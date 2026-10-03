using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;

namespace GymAssistant_API.Model.Entities.User
{
    public class AppUser : IdentityUser
    {
        public string PreferredLanguage { get; set; } = "en";
        public ClientProfile? Profile { get; set; }
    }
}

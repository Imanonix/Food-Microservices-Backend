using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.services.interfaces
{
    public interface ICookieService
    {
        (string? email, string? refreshToken, string? token) GetAuthCookies();
        void SetAuthCookies(string token, string refreshToken, string email);
        void DeleteAuthCookies();
    }
}

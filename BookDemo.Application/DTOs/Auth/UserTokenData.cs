using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
    public record UserTokenData(
     string UserId,
     string Email,
     IEnumerable<string> Roles);
}

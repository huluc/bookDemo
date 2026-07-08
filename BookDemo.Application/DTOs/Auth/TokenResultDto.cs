using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.DTOs.Auth
{
    public record TokenResultDto(
        string Token,
        DateTime Expires);
}

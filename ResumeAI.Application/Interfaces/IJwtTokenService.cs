using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeAI.Application.Interfaces
{
    public interface IJwtTokenService
    {
       string GenerateToken(
       Guid userId,
       string email,
       IEnumerable<string> roles);
    }
}

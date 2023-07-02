using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GameManagement.Api.Authorization
{
    public class QualifiedUserRequirement : IAuthorizationRequirement
    {
    }
}

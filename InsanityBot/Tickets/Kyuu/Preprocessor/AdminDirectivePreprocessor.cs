using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsanityBot.Utility.Permissions.Data;

namespace InsanityBot.Tickets.Kyuu.Preprocessor
{
    public static class AdminDirectivePreprocessor
    {
        public static Task<Boolean> ProcessDirective(KyuuPreprocessorContext context)
        {
            UserPermissions permissions = InsanityBot.PermissionEngine.GetUserPermissions(context.Message.Author.Id);
            return Task.FromResult(permissions.IsAdministrator);
        }
    }
}

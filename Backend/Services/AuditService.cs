using Serilog;
using System;

namespace Demo_Backend.Services
{
    public class AuditService
    {
        public void LogAction(string user, string action, string details = "")
        {
           
            Log.ForContext("Audit", true)
               .Information("User {User} performed {Action}. Details: {Details} at {Time}",
                   user ?? "Unknown",
                   action,
                   details,
                   DateTime.Now);
        }
    }
}

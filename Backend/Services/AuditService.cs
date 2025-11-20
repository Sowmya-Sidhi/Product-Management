/*
  AuditService.cs
  - Records audit events for important user actions (create/update/delete) using Serilog.
  - Adds an "Audit" property to log context so Serilog's configuration in Program.cs can route
    audit entries to a separate audit file.
  - Controllers call `AuditService.LogAction(...)` with user id and a short message.
*/
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

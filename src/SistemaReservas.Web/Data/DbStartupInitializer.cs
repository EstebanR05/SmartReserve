using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SistemaReservas.Web.Data;

public static class DbStartupInitializer
{
    private static readonly Regex GoBatchRegex = new(@"^\s*GO\s*($|--.*$)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static async Task InitializeAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        var scriptPath = ResolveStoredProceduresPath(environment.ContentRootPath);

        if (scriptPath is null || !File.Exists(scriptPath))
        {
            return;
        }

        var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
        var batches = GoBatchRegex.Split(script)
            .Select(batch => batch.Trim())
            .Where(batch => !string.IsNullOrWhiteSpace(batch));

        foreach (var batch in batches)
        {
            await dbContext.Database.ExecuteSqlRawAsync(batch, cancellationToken);
        }
    }

    private static string? ResolveStoredProceduresPath(string contentRootPath)
    {
        var candidates = new[]
        {
            Path.Combine(contentRootPath, "..", "database", "stored-procedures.sql"),
            Path.Combine(contentRootPath, "database", "stored-procedures.sql"),
            Path.Combine(contentRootPath, "Data", "Sql", "stored-procedures.sql")
        };

        return candidates
            .Select(Path.GetFullPath)
            .FirstOrDefault(File.Exists);
    }
}

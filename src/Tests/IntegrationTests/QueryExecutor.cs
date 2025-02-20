static class QueryExecutor
{
    public static async Task<string> ExecuteQuery<TDbContext>(
        string query,
        ServiceCollection services,
        TDbContext data,
        Inputs? inputs,
        Filters? filters,
        bool disableTracking,
        bool disableAsync)
        where TDbContext : DbContext
    {
        query = query.Replace("'", "\"");
        EfGraphQLConventions.RegisterInContainer(
            services,
            _ => data,
            data.Model,
            _ => filters,
            disableTracking,
            disableAsync);
        await using var provider = services.BuildServiceProvider();
        using var schema = new Schema(provider);
        var executer = new EfDocumentExecuter();

        var options = new ExecutionOptions
        {
            Schema = schema,
            Query = query,
            ThrowOnUnhandledException = true,
            UserContext = new UserContextSingleDb<TDbContext>(data),
            Variables = inputs,
        };

        var result = await executer.ExecuteWithErrorCheck(options);
        return await result.Serialize();
    }
}
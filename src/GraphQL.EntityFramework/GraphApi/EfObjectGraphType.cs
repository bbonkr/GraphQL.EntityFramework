namespace GraphQL.EntityFramework;

public class EfObjectGraphType<TDbContext, TSource> :
    ObjectGraphType<TSource>
    where TDbContext : DbContext
{
    IReadOnlyList<string>? exclusions;
    public IEfGraphQLService<TDbContext> GraphQlService { get; }

    /// <param name="exclusions">A list of property names to exclude from mapping.</param>
    public EfObjectGraphType(IEfGraphQLService<TDbContext> graphQlService, IReadOnlyList<string>? exclusions = null)
    {
        this.exclusions = exclusions;
        GraphQlService = graphQlService;
    }

    public override void Initialize(ISchema schema)
    {
        Mapper<TDbContext>.AutoMap(this, GraphQlService, exclusions);
        base.Initialize(schema);
    }

    public ConnectionBuilder<TSource> AddNavigationConnectionField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IEnumerable<TReturn>>? resolve = null,
        Type? graphType = null,
        IEnumerable<string>? includeNames = null)
        where TReturn : class =>
        GraphQlService.AddNavigationConnectionField(this, name, resolve, graphType, includeNames);

    public FieldBuilder<TSource, TReturn> AddNavigationField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, TReturn?>? resolve = null,
        Type? graphType = null,
        IEnumerable<string>? includeNames = null)
        where TReturn : class =>
        GraphQlService.AddNavigationField(this, name, resolve, graphType, includeNames);

    public FieldBuilder<TSource, TReturn> AddNavigationListField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IEnumerable<TReturn>>? resolve = null,
        Type? graphType = null,
        IEnumerable<string>? includeNames = null)
        where TReturn : class =>
        GraphQlService.AddNavigationListField(this, name, resolve, graphType, includeNames);

    public ConnectionBuilder<TSource> AddQueryConnectionField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IQueryable<TReturn>> resolve,
        Type? graphType = null)
        where TReturn : class =>
        GraphQlService.AddQueryConnectionField(this, name, resolve, graphType);

    public FieldBuilder<TSource, TReturn> AddQueryField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IQueryable<TReturn>> resolve,
        Type? graphType = null)
        where TReturn : class =>
        GraphQlService.AddQueryField(this, name, resolve, graphType);

    public TDbContext ResolveDbContext(IResolveFieldContext<TSource> context) =>
        GraphQlService.ResolveDbContext(context);

    public TDbContext ResolveDbContext(IResolveFieldContext context) =>
        GraphQlService.ResolveDbContext(context);

    public FieldBuilder<TSource, TReturn> AddSingleField<TReturn>(
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IQueryable<TReturn>> resolve,
        Func<ResolveEfFieldContext<TDbContext, TSource>, TReturn, Task>? mutate = null,
        Type? graphType = null,
        bool nullable = false)
        where TReturn : class =>
        GraphQlService.AddSingleField(this, name, resolve, mutate, graphType,  nullable);
}
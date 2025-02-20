﻿namespace GraphQL.EntityFramework;

partial class EfGraphQLService<TDbContext>
    where TDbContext : DbContext
{
    static MethodInfo addEnumerableConnection = typeof(EfGraphQLService<TDbContext>)
        .GetMethod("AddEnumerableConnection", BindingFlags.Instance| BindingFlags.NonPublic)!;

    public ConnectionBuilder<TSource> AddNavigationConnectionField<TSource, TReturn>(
        ComplexGraphType<TSource> graph,
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IEnumerable<TReturn>>? resolve = null,
        Type? itemGraphType = null,
        IEnumerable<string>? includeNames = null)
        where TReturn : class
    {
        Guard.AgainstWhiteSpace(nameof(name), name);

        itemGraphType ??= GraphTypeFinder.FindGraphType<TReturn>();

        var addConnectionT = addEnumerableConnection.MakeGenericMethod(typeof(TSource), itemGraphType, typeof(TReturn));
        return (ConnectionBuilder<TSource>)addConnectionT.Invoke(this, new object?[] { graph, name, resolve, includeNames })!;
    }

    ConnectionBuilder<TSource> AddEnumerableConnection<TSource, TGraph, TReturn>(
        ComplexGraphType<TSource> graph,
        string name,
        Func<ResolveEfFieldContext<TDbContext, TSource>, IEnumerable<TReturn>>? resolve,
        IEnumerable<string>? includeNames)
        where TGraph : IGraphType
        where TReturn : class
    {
        var builder = ConnectionBuilderEx<TSource>.Build<TGraph>(name);

        IncludeAppender.SetIncludeMetadata(builder.FieldType, name, includeNames);

        var hasId = keyNames.ContainsKey(typeof(TReturn));
        if (resolve is not null)
        {
            builder.ResolveAsync(async context =>
            {
                var efFieldContext = BuildContext(context);

                var enumerable = resolve(efFieldContext);

                enumerable = enumerable.ApplyGraphQlArguments(hasId, context);
                enumerable = await efFieldContext.Filters.ApplyFilter(enumerable, context.UserContext, context.User);
                var page = enumerable.ToList();

                return ConnectionConverter.ApplyConnectionContext(
                    page,
                    context.First,
                    context.After,
                    context.Last,
                    context.Before);
            });
        }

        //TODO: works around https://github.com/graphql-dotnet/graphql-dotnet/pull/2581/
        builder.FieldType.Type = typeof(NonNullGraphType<ConnectionType<TGraph, EdgeType<TGraph>>>);
        var field = graph.AddField(builder.FieldType);

        field.AddWhereArgument(hasId);
        return builder;
    }
}
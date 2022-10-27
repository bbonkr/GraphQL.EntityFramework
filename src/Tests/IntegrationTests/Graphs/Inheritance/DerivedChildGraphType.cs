public class DerivedChildGraphType :
    EfObjectGraphType<IntegrationDbContext, DerivedChildEntity>
{
    public DerivedChildGraphType(IEfGraphQLService<IntegrationDbContext> graphQlService) :
        base(graphQlService, new List<string> { "Parent", "TypedParent" })
    {
    }
}
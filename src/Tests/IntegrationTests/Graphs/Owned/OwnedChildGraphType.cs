﻿public class OwnedChildGraphType :
    EfObjectGraphType<IntegrationDbContext, OwnedChild>
{
    public OwnedChildGraphType(IEfGraphQLService<IntegrationDbContext> graphQlService) :
        base(graphQlService)
    {
    }
}
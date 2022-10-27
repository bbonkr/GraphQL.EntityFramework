﻿public class DerivedGraphType :
    EfObjectGraphType<IntegrationDbContext, DerivedEntity>
{
    public DerivedGraphType(IEfGraphQLService<IntegrationDbContext> graphQlService) :
        base(graphQlService)
    {
        AddNavigationConnectionField(
            name: "childrenFromInterface",
            e => e.Source.ChildrenFromBase);
        Interface<InterfaceGraphType>();
        IsTypeOf = obj => obj is DerivedEntity;
    }
}
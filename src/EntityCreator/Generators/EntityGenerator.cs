using EntityCreator.Models;

namespace EntityCreator.Generators;

public class EntityGenerator
{
    public bool Generate(EntityModel entity)
    {
        // Domain ------------------------------

        //Create Domain Model
        ModelCreator modelCreator = new(entity);
        modelCreator.Create();

        //Create IRepositry Contract
        RepositoryContractCreator repositoryContractCreator = new(entity);
        repositoryContractCreator.Create();

        //Create Localization terms
        var localizationUpdater = new LocalizationUpdater(entity);
        localizationUpdater.Update();

        // EF ------------------------------

        //Update EntityFramework Context
        EFContextUpdater efContextUpdater = new(entity);
        efContextUpdater.Update();

        //Create Repositry 
        EFRepositoryCreator efRepositoryCreator = new(entity);
        efRepositoryCreator.Create();

        // Contracts ------------------------------

        //Create Dtos
        DtosCreator dtosCreator = new(entity);
        dtosCreator.Create();

        //Create IAppService Contract 
        AppServiceContractCreator appServiceContractCreator = new(entity);
        appServiceContractCreator.Create();

        //Create Permission Contract 
        PermissionUpdater permissionUpdater = new(entity);
        permissionUpdater.Update();

        //Define Permissions in provider
        PermissionProviderUpdater permissionProviderUpdater = new(entity);
        permissionProviderUpdater.Update();

        // Application ------------------------------

        //Create AppService (Application)
        AppServiceCreator appServiceCreator = new(entity);
        appServiceCreator.Create();

        //Create AutoMapper (Application)
        AppMapperUpdater appMapperUpdater = new(entity);
        appMapperUpdater.Update();

        // Mvc ------------------------------

        //Menus
        MvcMenuUpdater mvcMenuUpdater = new(entity);
        mvcMenuUpdater.Update();

        //Create View Models
        MvcViewModelCreator mvcViewModelCreator = new(entity);
        mvcViewModelCreator.Create();

        //Create Index Page 
        MvcIndexPageCreator indexPageCreator = new(entity);
        indexPageCreator.Create();

        //Create EditModal 
        MvcEditModalCreator editModalCreator = new(entity);
        editModalCreator.Create();

        //Create CreateModal 
        MvcCreateModalCreator createModalCreator = new(entity);
        createModalCreator.Create();

        return true;
    }
}

using EntityCreator.Models;

namespace EntityCreator.Generators;

public class EntityGenerator
{
    public bool Generate(EntityModel entity)
    {
        // Domain ------------------------------

        //Create Domain Model
        ModelCreator modelCreator = new(entity);
        modelCreator.Handle();

        //Create IRepositry Contract
        RepositoryContractCreator repositoryContractCreator = new(entity);
        repositoryContractCreator.Handle();

        //Create Localization terms
        var localizationUpdater = new LocalizationUpdater(entity);
        localizationUpdater.Handle();

        // EF ------------------------------

        //Update EntityFramework Context
        EFContextUpdater efContextUpdater = new(entity);
        efContextUpdater.Handle();

        //Create Repository 
        EFRepositoryCreator efRepositoryCreator = new(entity);
        efRepositoryCreator.Handle();

        // Contracts ------------------------------

        //Create Dtos
        DtosCreator dtosCreator = new(entity);
        dtosCreator.Handle();

        //Create IAppService Contract 
        AppServiceContractCreator appServiceContractCreator = new(entity);
        appServiceContractCreator.Handle();

        //Create Permission Contract 
        PermissionUpdater permissionUpdater = new(entity);
        permissionUpdater.Handle();

        //Define Permissions in provider
        PermissionProviderUpdater permissionProviderUpdater = new(entity);
        permissionProviderUpdater.Handle();

        // Application ------------------------------

        //Create AppService (Application)
        AppServiceCreator appServiceCreator = new(entity);
        appServiceCreator.Handle();

        //Create AutoMapper (Application)
        AppMapperUpdater appMapperUpdater = new(entity);
        appMapperUpdater.Handle();

        // Mvc ------------------------------

        //Menus
        MvcMenuUpdater mvcMenuUpdater = new(entity);
        mvcMenuUpdater.Handle();

        //Create View Models
        MvcViewModelCreator mvcViewModelCreator = new(entity);
        mvcViewModelCreator.Handle();

        //Create Index Page 
        MvcIndexPageCreator indexPageCreator = new(entity);
        indexPageCreator.Handle();

        //Create EditModal 
        MvcEditModalCreator editModalCreator = new(entity);
        editModalCreator.Handle();

        //Create CreateModal 
        MvcCreateModalCreator createModalCreator = new(entity);
        createModalCreator.Handle();

        return true;
    }
}

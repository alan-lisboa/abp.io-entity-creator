using EntityCreator.Models;

namespace EntityCreator.Generators;

public class EntityGenerator
{
    public static bool Generate(EntityModel entity)
    {
        bool result;

        // Domain ------------------------------

        //Create Domain Model
        ModelCreator modelCreator = new(entity);
        result = modelCreator.Handle();
        if (!result) return result;

        //Create IRepositry Contract
        RepositoryContractCreator repositoryContractCreator = new(entity);
        result = repositoryContractCreator.Handle();
        if (!result) return result;

        //Create Localization terms
        var localizationUpdater = new LocalizationUpdater(entity);
        result = localizationUpdater.Handle();
        if (!result) return result;

        // EF ------------------------------

        //Update EntityFramework Context
        EFContextUpdater efContextUpdater = new(entity);
        result = efContextUpdater.Handle();
        if (!result) return result;

        //Create Repository 
        EFRepositoryCreator efRepositoryCreator = new(entity);
        result = efRepositoryCreator.Handle();
        if (!result) return result;

        // Contracts ------------------------------

        //Create Dtos
        DtosCreator dtosCreator = new(entity);
        result = dtosCreator.Handle();
        if (!result) return result;

        //Create IAppService Contract 
        AppServiceContractCreator appServiceContractCreator = new(entity);
        result = appServiceContractCreator.Handle();
        if (!result) return result;

        //Create Permission Contract 
        PermissionUpdater permissionUpdater = new(entity);
        result = permissionUpdater.Handle();
        if (!result) return result;

        //Define Permissions in provider
        PermissionProviderUpdater permissionProviderUpdater = new(entity);
        result = permissionProviderUpdater.Handle();
        if (!result) return result;

        // Application ------------------------------

        //Create AppService (Application)
        AppServiceCreator appServiceCreator = new(entity);
        result = appServiceCreator.Handle();
        if (!result) return result;

        //Create AutoMapper (Application)
        AppMapperUpdater appMapperUpdater = new(entity);
        result = appMapperUpdater.Handle();
        if (!result) return result;

        // Mvc ------------------------------

        //Menus
        MvcMenuUpdater mvcMenuUpdater = new(entity);
        result = mvcMenuUpdater.Handle();
        if (!result) return result;

        //Create View Models
        MvcViewModelCreator mvcViewModelCreator = new(entity);
        result = mvcViewModelCreator.Handle();
        if (!result) return result;

        //Create Index Page 
        MvcIndexPageCreator indexPageCreator = new(entity);
        result = indexPageCreator.Handle();
        if (!result) return result;

        //Create EditModal 
        MvcEditModalCreator editModalCreator = new(entity);
        result = editModalCreator.Handle();
        if (!result) return result;

        //Create CreateModal 
        MvcCreateModalCreator createModalCreator = new(entity);
        result = createModalCreator.Handle();
        if (!result) return result;

        // Migrations ------------------------------
        if (entity.RunMigrations)
            RunMigrations(entity);

        return true;
    }

    public static void RunMigrations(EntityModel entity)
    {
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.EntityFrameworkCore";

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"ef migrations add Added_{entity.Name} --project {folder}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        process.Start();
        process.WaitForExit();
    }
}

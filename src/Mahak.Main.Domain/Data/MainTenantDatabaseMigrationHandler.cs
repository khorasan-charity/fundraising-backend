using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Mahak.Main.Data;

public class MainTenantDatabaseMigrationHandler :
    IDistributedEventHandler<TenantCreatedEto>,
    IDistributedEventHandler<TenantConnectionStringUpdatedEto>,
    IDistributedEventHandler<ApplyDatabaseMigrationsEto>,
    ITransientDependency
{
    private readonly IEnumerable<IMainDbSchemaMigrator> _dbSchemaMigrators;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IDataSeeder _dataSeeder;
    private readonly ITenantStore _tenantStore;
    private readonly ILogger<MainTenantDatabaseMigrationHandler> _logger;

    public MainTenantDatabaseMigrationHandler(
        IEnumerable<IMainDbSchemaMigrator> dbSchemaMigrators,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        IDataSeeder dataSeeder,
        ITenantStore tenantStore,
        ILogger<MainTenantDatabaseMigrationHandler> logger)
    {
        _dbSchemaMigrators = dbSchemaMigrators;
        _currentTenant = currentTenant;
        _unitOfWorkManager = unitOfWorkManager;
        _dataSeeder = dataSeeder;
        _tenantStore = tenantStore;
        _logger = logger;
    }

    public async Task HandleEventAsync(TenantCreatedEto eventData)
    {
        await MigrateAndSeedForTenantAsync(
            eventData.Id,
            eventData.Properties.GetOrDefault("AdminEmail") ?? MainConsts.AdminEmailDefaultValue,
            eventData.Properties.GetOrDefault("AdminPassword") ?? MainConsts.AdminPasswordDefaultValue
        );
    }

    public async Task HandleEventAsync(TenantConnectionStringUpdatedEto eventData)
    {
        if (eventData.ConnectionStringName != ConnectionStrings.DefaultConnectionStringName ||
            eventData.NewValue.IsNullOrWhiteSpace())
        {
            return;
        }

        await MigrateAndSeedForTenantAsync(
            eventData.Id,
            MainConsts.AdminEmailDefaultValue,
            MainConsts.AdminPasswordDefaultValue
        );

        /* You may want to move your data from the old database to the new database!
         * It is up to you. If you don't make it, new database will be empty
         * (and tenant's admin password is reset to 1q2w3E*).
         */
    }

    public async Task HandleEventAsync(ApplyDatabaseMigrationsEto eventData)
    {
        if (eventData.TenantId == null)
        {
            return;
        }

        await MigrateAndSeedForTenantAsync(
            eventData.TenantId.Value,
            MainConsts.AdminEmailDefaultValue,
            MainConsts.AdminPasswordDefaultValue
        );
    }

    private async Task MigrateAndSeedForTenantAsync(
        Guid tenantId,
        string adminEmail,
        string adminPassword)
    {
        try
        {
            using (_currentTenant.Change(tenantId))
            {
                // Create database tables if needed
                using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
                {
                    var tenantConfiguration = await _tenantStore.FindAsync(tenantId);
                    if (tenantConfiguration?.ConnectionStrings != null &&
                        !tenantConfiguration.ConnectionStrings.Default.IsNullOrWhiteSpace())
                    {
                        foreach (var migrator in _dbSchemaMigrators)
                        {
                            await migrator.MigrateAsync();
                        }
                    }

                    await uow.CompleteAsync();
                }

                // Seed data
                using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
                {
                    await _dataSeeder.SeedAsync(
                        new DataSeedContext(tenantId)
                            .WithProperty(IdentityDataSeedContributor.AdminEmailPropertyName, adminEmail)
                            .WithProperty(IdentityDataSeedContributor.AdminPasswordPropertyName, adminPassword)
                    );

                    await uow.CompleteAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }
}

using Mahak.Main.Attributes;
using Mahak.Main.Campaigns;
using Mahak.Main.Donations;
using Mahak.Main.Files;
using Mahak.Main.Payments;
using Mahak.Main.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace Mahak.Main.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class MainDbContext :
    AbpDbContext<MainDbContext>,
    IIdentityDbContext
{
    public DbSet<File> Files { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<CampaignItem> CampaignItems { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();

        /* Configure your own tables/entities inside here */
        
        builder.Entity<File>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Files", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired();
            b.Property(x => x.ContentType)
                .IsRequired();
            b.Property(x => x.Extension)
                .IsRequired();
            b.Property(x => x.Size)
                .IsRequired();

            b.HasIndex(x => x.Extension);
        });

        builder.Entity<Campaign>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Campaigns", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Type)
                .IsRequired();
            b.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();
            b.Property(x => x.CoverImageFileId)
                .IsRequired(false);
            b.Property(x => x.ThumbnailImageFileId)
                .IsRequired(false);
            b.Property(x => x.Description)
                .IsRequired(false);
            b.Property(x => x.StartDateTime)
                .IsRequired(false);
            b.Property(x => x.EndDateTime)
                .IsRequired(false);
            b.Property(x => x.TargetAmount)
                .HasPrecision(18, 2)
                .IsRequired(false);
            b.Property(x => x.RaisedAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            b.Property(x => x.RaiseCount)
                .IsRequired();
            b.Property(x => x.IsVisible)
                .IsRequired();
            b.Property(x => x.IsActive)
                .IsRequired();
        });

        builder.Entity<CampaignItem>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "CampaignItems", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Campaign)
                .WithMany(x => x.CampaignItems)
                .HasForeignKey(x => x.CampaignId)
                .IsRequired();
            b.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();
            b.Property(x => x.ImageFileId)
                .IsRequired(false);
            b.Property(x => x.Description)
                .IsRequired(false);
            b.Property(x => x.TargetAmount)
                .HasPrecision(18, 2)
                .IsRequired(false);
            b.Property(x => x.RaisedAmount)
                .HasPrecision(18, 2)
                .IsRequired();
            b.Property(x => x.RaiseCount)
                .IsRequired();
        });

        builder.Entity<Attribute>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Attributes", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
                .IsRequired();
            b.Property(x => x.ValueType)
                .IsRequired(false);
            b.Property(x => x.ValueTypeTitle)
                .IsRequired(false);
            b.Property(x => x.Description)
                .IsRequired(false);
        });

        builder.Entity<CampaignItemAttribute>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "CampaignItemAttributes", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.CampaignItem)
                .
        });

        builder.Entity<Donation>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Donations", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne(x => x.Campaign)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.CampaignId)
                .IsRequired();
            b.HasOne(x => x.CampaignItem)
                .WithMany(x => x.Donations)
                .HasForeignKey(x => x.CampaignItemId)
                .IsRequired(false);
            b.HasOne(x => x.Payment)
                .WithMany()
                .HasForeignKey(x => x.PaymentId)
                .IsRequired(false);
            b.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
            b.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(false);
            b.Property(x => x.Message)
                .HasMaxLength(300)
                .IsRequired(false);
            b.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);
        });

        builder.Entity<Payment>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Payments", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.TrackingNumber)
                .IsRequired();
            b.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
            b.Property(x => x.Token)
                .IsRequired(false);
            b.Property(x => x.TransactionCode)
                .IsRequired(false);
            b.Property(x => x.GatewayName)
                .HasMaxLength(20)
                .IsRequired();
            b.Property(x => x.GatewayAccountName)
                .IsRequired(false);
            b.Property(x => x.IsCompleted)
                .IsRequired();
            b.Property(x => x.IsPaid)
                .IsRequired();

            b.HasIndex(x => x.TrackingNumber)
                .IsUnique();
            b.HasIndex(x => x.Token)
                .IsUnique();
        });

        builder.Entity<Transaction>(b =>
        {
            b.ToTable(MainConsts.DbTablePrefix + "Transactions", MainConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne<Payment>()
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
            b.Property(x => x.Type)
                .IsRequired();
            b.Property(x => x.IsSucceed)
                .IsRequired();
            b.Property(x => x.Message)
                .IsRequired(false);
            b.Property(x => x.AdditionalData)
                .IsRequired(false);
        });
    }
}
using Xunit;

namespace Mahak.Main.EntityFrameworkCore;

[CollectionDefinition(MainTestConsts.CollectionDefinitionName)]
public class MainEntityFrameworkCoreCollection : ICollectionFixture<MainEntityFrameworkCoreFixture>
{

}

using Mahak.Main.Samples;
using Xunit;

namespace Mahak.Main.EntityFrameworkCore.Applications;

[Collection(MainTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MainEntityFrameworkCoreTestModule>
{

}

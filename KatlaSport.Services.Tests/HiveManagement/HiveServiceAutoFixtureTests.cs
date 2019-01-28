using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveServiceAutoFixtureTests
    {
        public HiveServiceAutoFixtureTests()
        {
            var mapper = AutoMapperMappingProfile.Instance;
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHivesAsync_CollectionWithThreeElements_ThreeElementsListReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(3).ToArray();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());

            var list = await service.GetHivesAsync();

            list.Should().HaveCount(3);
            list.Should().Contain(h => h.Id == hives[0].Id);
            list.Should().Contain(h => h.Id == hives[1].Id);
            list.Should().Contain(h => h.Id == hives[2].Id);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetHiveAsync_CollectionWithTwoElements_FirstReturned([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(2).ToArray();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);

            var hive = await service.GetHiveAsync(hives[0].Id);

            hive.Id.Should().Be(hives[0].Id);
        }

        [Theory]
        [AutoMoqData]
        public async Task CreateHiveAsync_EmptyCollection_NewHiveReturned([Frozen] Mock<IProductStoreHiveContext> context, UpdateHiveRequest request, HiveService service)
        {
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());

            var hive = await service.CreateHiveAsync(request);

            Assert.Equal(request.Code, hive.Code);
            Assert.Equal(request.Name, hive.Name);
            Assert.Equal(request.Address, hive.Address);
        }

        [Theory]
        [AutoMoqData]
        public async Task UpdateHiveAsync_ChangeHiveProperties_UpdatedHiveReturned([Frozen] Mock<IProductStoreHiveContext> context, UpdateHiveRequest request, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(2).ToArray();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);

            var hive = await service.UpdateHiveAsync(hives[1].Id, request);

            Assert.Equal(request.Code, hive.Code);
            Assert.Equal(request.Name, hive.Name);
            Assert.Equal(request.Address, hive.Address);
        }

        [Theory]
        [AutoMoqData]
        public async Task DeleteHiveAsync_HiveWithTrueStatus_HiveDeleted([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(1).ToList();
            hives[0].IsDeleted = true;
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);

            await service.DeleteHiveAsync(hives[0].Id);
            var list = await service.GetHivesAsync();

            list.Should().BeEmpty();
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_ChangeStatusFromTrueToFalse_StatusIsFalse([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(2).ToArray();
            hives[0].IsDeleted = true;
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);

            await service.SetStatusAsync(hives[0].Id, false);
            var hive = await service.GetHiveAsync(hives[0].Id);

            hive.IsDeleted.Should().Be(false);
        }

        [Theory]
        [AutoMoqData]
        public async Task SetStatusAsync_ChangeStatusFromFalseToFalse_StatusIsFalse([Frozen] Mock<IProductStoreHiveContext> context, HiveService service, IFixture fixture)
        {
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var hives = fixture.CreateMany<StoreHive>(2).ToArray();
            hives[1].IsDeleted = true;
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);

            await service.SetStatusAsync(hives[1].Id, false);
            var hive = await service.GetHiveAsync(hives[1].Id);

            hive.IsDeleted.Should().Be(false);
        }
    }
}

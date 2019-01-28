using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveServiceTests
    {
        public HiveServiceTests()
        {
            var mapper = AutoMapperMappingProfile.Instance;
        }

        [Fact]
        public void Ctor_ProductStoreHiveContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new HiveService(null, new UserContext()));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var exception = Assert.Throws<ArgumentNullException>(() => new HiveService(context.Object, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public async Task GetHivesAsync_EmptyCollection_EmptyListReturned()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveService(context.Object, new UserContext());

            var list = await service.GetHivesAsync();

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetHivesAsync_CollectionWithThreeElements_ThreeElementsListReturned()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0 },
                new StoreHive() { Id = 1 },
                new StoreHive() { Id = 2 }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveService(context.Object, new UserContext());

            var list = await service.GetHivesAsync();

            Assert.Equal(3, list.Count);
            Assert.Contains(list, h => h.Id == 0);
            Assert.Contains(list, h => h.Id == 1);
            Assert.Contains(list, h => h.Id == 2);
        }

        [Fact]
        public async Task GetHiveAsync_EmptyCollection_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async() =>
            {
                await service.GetHiveAsync(0);
            });
        }

        [Fact]
        public async Task GetHiveAsync_CollectionWithTwoElements_FirstReturned()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0 },
                new StoreHive() { Id = 1 }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());

            var hive = await service.GetHiveAsync(0);

            Assert.Equal(0, hive.Id);
        }

        [Fact]
        public async Task CreateHiveAsync_CreateHiveWithExistingHiveCode_ExceptionThrown()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, Code = "CODE1" },
                new StoreHive() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());
            var request = new UpdateHiveRequest { Code = "CODE1" };

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.CreateHiveAsync(request);
            });
        }

        [Fact]
        public async Task CreateHiveAsync_EmptyCollection_NewHiveReturned()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, new UserContext());
            var request = new UpdateHiveRequest { Code = "CODE1", Name = "Hive", Address = "Test" };

            var hive = await service.CreateHiveAsync(request);

            Assert.Equal(request.Code, hive.Code);
            Assert.Equal(request.Name, hive.Name);
            Assert.Equal(request.Address, hive.Address);
        }

        [Fact]
        public async Task UpdateHiveAsync_UpdateNonExistentHive_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, new UserContext());
            var request = new UpdateHiveRequest();

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.UpdateHiveAsync(0, request);
            });
        }

        [Fact]
        public async Task UpdateHiveAsync_ChangeCodeToExistingHiveCode_ExceptionThrown()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, Code = "CODE1" },
                new StoreHive() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());
            var request = new UpdateHiveRequest { Code = "CODE1" };

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.UpdateHiveAsync(1, request);
            });
        }

        [Fact]
        public async Task UpdateHiveAsync_ChangeHiveProperties_UpdatedHiveReturned()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, Code = "CODE1" },
                new StoreHive() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());
            var request = new UpdateHiveRequest { Code = "CODE3", Name = "New Name", Address = "New Address" };

            var hive = await service.UpdateHiveAsync(1, request);

            Assert.Equal(request.Code, hive.Code);
            Assert.Equal(request.Name, hive.Name);
            Assert.Equal(request.Address, hive.Address);
        }

        [Fact]
        public async Task DeleteHiveAsync_DeleteNonExistentHive_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.DeleteHiveAsync(0);
            });
        }

        [Fact]
        public async Task DeleteHiveAsync_HiveWithFalseStatus_ExceptionThrown()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, IsDeleted = true },
                new StoreHive() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.DeleteHiveAsync(1);
            });
        }

        [Fact]
        public async Task DeleteHiveAsync_HiveWithTrueStatus_HiveDeleted()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, IsDeleted = true },
                new StoreHive() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());

            await service.DeleteHiveAsync(0);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveAsync(0));
        }

        [Fact]
        public async Task SetStatusAsync_UpdateNonExistentHive_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
                {
                    await service.SetStatusAsync(1, true);
                });
        }

        [Fact]
        public async Task SetStatusAsync_ChangeStatusFromFalseToTrue_StatusIsTrue()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, IsDeleted = true },
                new StoreHive() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());

            await service.SetStatusAsync(1, true);
            var hive = await service.GetHiveAsync(1);

            Assert.True(hive.IsDeleted);
        }

        [Fact]
        public async Task SetStatusAsync_ChangeStatusFromTrueToTrue_StatusIsTrue()
        {
            var hives = new List<StoreHive>()
            {
                new StoreHive() { Id = 0, IsDeleted = true },
                new StoreHive() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(hives);
            var service = new HiveService(context.Object, new UserContext());

            await service.SetStatusAsync(0, true);
            var hive = await service.GetHiveAsync(0);

            Assert.True(hive.IsDeleted);
        }
    }
}

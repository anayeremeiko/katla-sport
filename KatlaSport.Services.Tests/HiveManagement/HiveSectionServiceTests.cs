using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveSectionServiceTests
    {
        public HiveSectionServiceTests()
        {
            var mapper = AutoMapperMappingProfile.Instance;
        }

        [Fact]
        public void Ctor_ProductStoreHiveContextIsNull_ExceptionThrown()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new HiveSectionService(null, new UserContext()));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public void Ctor_UserContextIsNull_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var exception = Assert.Throws<ArgumentNullException>(() => new HiveSectionService(context.Object, null));

            Assert.Equal(typeof(ArgumentNullException), exception.GetType());
        }

        [Fact]
        public async Task GetHiveSectionsAsync_EmptyCollection_EmptyListReturned()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());

            var list = await service.GetHiveSectionsAsync();

            Assert.Empty(list);
        }

        [Fact]
        public async Task GetHiveSectionsAsync_CollectionWithThreeElements_ThreeElementsListReturned()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0 },
                new StoreHiveSection() { Id = 1 },
                new StoreHiveSection() { Id = 2 }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            var list = await service.GetHiveSectionsAsync();

            Assert.Equal(3, list.Count);
            Assert.Contains(list, h => h.Id == 0);
            Assert.Contains(list, h => h.Id == 1);
            Assert.Contains(list, h => h.Id == 2);
        }

        [Fact]
        public async Task GetHiveSectionAsync_EmptyCollection_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.GetHiveSectionAsync(0);
            });
        }

        [Fact]
        public async Task GetHiveSectionAsync_CollectionWithTwoElements_FirstReturned()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0 },
                new StoreHiveSection() { Id = 1 }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            var section = await service.GetHiveSectionAsync(0);

            Assert.Equal(0, section.Id);
        }

        [Fact]
        public async Task CreateHiveSectionAsync_CreateSectionWithExistingSectionCode_ExceptionThrown()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, Code = "CODE1" },
                new StoreHiveSection() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());
            var request = new UpdateHiveSectionRequest { Code = "CODE1" };

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.CreateHiveSectionAsync(request);
            });
        }

        [Fact]
        public async Task CreateHiveSectionAsync_EmptyCollection_NewSectionReturned()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());
            var request = new UpdateHiveSectionRequest { Code = "CODE1", Name = "Section" };

            var section = await service.CreateHiveSectionAsync(request);

            Assert.Equal(request.Code, section.Code);
            Assert.Equal(request.Name, section.Name);
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_UpdateNonExistentSection_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());
            var request = new UpdateHiveSectionRequest();

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.UpdateHiveSectionAsync(0, request);
            });
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_ChangeCodeToExistingSectionCode_ExceptionThrown()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, Code = "CODE1" },
                new StoreHiveSection() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());
            var request = new UpdateHiveSectionRequest { Code = "CODE1" };

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.UpdateHiveSectionAsync(1, request);
            });
        }

        [Fact]
        public async Task UpdateHiveSectionAsync_ChangeSectionProperties_UpdatedSectionReturned()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, Code = "CODE1" },
                new StoreHiveSection() { Id = 1, Code = "CODE2" }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());
            var request = new UpdateHiveSectionRequest { Code = "CODE3", Name = "New Name" };

            var section = await service.UpdateHiveSectionAsync(1, request);

            Assert.Equal(request.Code, section.Code);
            Assert.Equal(request.Name, section.Name);
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_DeleteNonExistentSection_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.DeleteHiveSectionAsync(0);
            });
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_SectionWithFalseStatus_ExceptionThrown()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, IsDeleted = true },
                new StoreHiveSection() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () =>
            {
                await service.DeleteHiveSectionAsync(1);
            });
        }

        [Fact]
        public async Task DeleteHiveSectionAsync_SectionWithTrueStatus_SectionDeleted()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, IsDeleted = true },
                new StoreHiveSection() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            await service.DeleteHiveSectionAsync(0);

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(() => service.GetHiveSectionAsync(0));
        }

        [Fact]
        public async Task SetStatusAsync_UpdateNonExistentSection_ExceptionThrown()
        {
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, new UserContext());

            await Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () =>
            {
                await service.SetStatusAsync(1, true);
            });
        }

        [Fact]
        public async Task SetStatusAsync_ChangeStatusFromFalseToTrue_StatusIsTrue()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, IsDeleted = true },
                new StoreHiveSection() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            await service.SetStatusAsync(1, true);
            var section = await service.GetHiveSectionAsync(1);

            Assert.True(section.IsDeleted);
        }

        [Fact]
        public async Task SetStatusAsync_ChangeStatusFromTrueToTrue_StatusIsTrue()
        {
            var sections = new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 0, IsDeleted = true },
                new StoreHiveSection() { Id = 1, IsDeleted = false }
            };
            var context = new Mock<IProductStoreHiveContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(sections);
            var service = new HiveSectionService(context.Object, new UserContext());

            await service.SetStatusAsync(0, true);
            var section = await service.GetHiveSectionAsync(0);

            Assert.True(section.IsDeleted);
        }
    }
}

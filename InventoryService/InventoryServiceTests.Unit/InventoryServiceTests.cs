using FluentAssertions;
using InventoryService.Exceptions;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Moq;
using Xunit;

namespace InventoryServiceTests.Unit
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _mockInventoryRepo;
        private readonly InventoryService.Services.InventoryService _sut;

        public InventoryServiceTests()
        {
            _mockInventoryRepo = new Mock<IInventoryRepository>();
            _sut = new InventoryService.Services.InventoryService(_mockInventoryRepo.Object);
        }

        [Fact]
        public async Task AddItem_ThrowsException_WhenUpcIsNull()
        {
            var testInventoryItem = new Inventory();

            await _sut.Invoking(m => m.AddInventoryItem(testInventoryItem))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage("InventoryItem parameter is invalid for AddItem command.");
        }

        [Fact]
        public async Task AddItem_ThrowsException_WhenUpcAlreadyExists()
        {
            var testInventoryItem = new Inventory
            {
                InventoryId = 1,
                Upc = "11223344"
            };

            _mockInventoryRepo.Setup(m => m.GetItemByUpc(testInventoryItem.Upc))
                .ReturnsAsync(testInventoryItem);

            await _sut.Invoking(m => m.AddInventoryItem(testInventoryItem))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage($"{testInventoryItem.Upc} already exists in the Inventory Database.");
        }

        [Fact]
        public async Task AddItem_ReturnsAddedItem()
        {
            var testAddInventoryItem = new Inventory { Upc = "11223344" };
            var testResultInventoryItem = new Inventory { InventoryId = 11, Upc = "11223344" };

            _mockInventoryRepo.SetupSequence(m => m.GetItemByUpc(testAddInventoryItem.Upc))
                .ReturnsAsync((Inventory)null)
                .ReturnsAsync(testResultInventoryItem);

            _mockInventoryRepo.Setup(m => m.AddItem(testAddInventoryItem));

            var actual = await _sut.AddInventoryItem(testAddInventoryItem);

            actual.Should().NotBeNull();
            actual.InventoryId.Should().Be(11);
            actual.Upc.Should().BeEquivalentTo(testAddInventoryItem.Upc);
        }

        [Fact]
        public async Task GetAllItems_ReturnsListOfInventoryItems()
        {
            _mockInventoryRepo.Setup(m => m.GetAll())
                .ReturnsAsync(new List<Inventory>{
                    new Inventory
                    {
                        InventoryId = 12,
                        Upc="12223242",
                        Description="TestItem1"
                    },
                    new Inventory
                    {
                        InventoryId = 13,
                        Upc="13233343",
                        Description="TestItem2"
                    } });

            var actual = await _sut.GetAllInventoryItems();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem_WhenInventoryIdIsNotNull()
        {
            _mockInventoryRepo.Setup(m => m.GetItemByInventoryId(14))
                .ReturnsAsync(new Inventory
                {
                    InventoryId = 14,
                    Upc = "14243444",
                    Description = "TestItem4"
                });

            var actual = await _sut.GetSingleInventoryItem(14, null);

            actual.Should().NotBeNull();
            actual.Upc.Should().BeEquivalentTo("14243444");

        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem_WhenUpcIsNotNull()
        {
            _mockInventoryRepo.Setup(m => m.GetItemByUpc("15253545"))
                .ReturnsAsync(new Inventory
                {
                    InventoryId = 15,
                    Upc = "15253545",
                    Description = "TestItem5"
                });

            var actual = await _sut.GetSingleInventoryItem(null, "15253545");

            actual.Should().NotBeNull();
            actual.InventoryId.Should().Be(15);
        }

        [Fact]
        public async Task GetItem_ThrowsException_WhenBothUpc_AndInventoryId_AreNull()
        {
            await _sut.Invoking(m => m.GetSingleInventoryItem(null, null))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage("You must supply an inventoryId or a Upc to select a single InventoryItem.");
        }
    }
}
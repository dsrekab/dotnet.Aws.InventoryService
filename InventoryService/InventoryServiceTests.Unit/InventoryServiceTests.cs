using FluentAssertions;
using InventoryService.Exceptions;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventoryServiceTests.Unit
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _mockInventoryRepo;
        private readonly Mock<ILogger<IInventoryService>> _mockLogger;
        private readonly InventoryService.Services.InventoryService _sut;

        public InventoryServiceTests()
        {
            _mockInventoryRepo = new Mock<IInventoryRepository>();
            _mockLogger = new Mock<ILogger<IInventoryService>>();

            _sut = new InventoryService.Services.InventoryService(_mockInventoryRepo.Object, _mockLogger.Object);
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
                InventoryItemId = 1,
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
            var testResultInventoryItem = new Inventory { InventoryItemId = 11, Upc = "11223344" };

            _mockInventoryRepo.SetupSequence(m => m.GetItemByUpc(testAddInventoryItem.Upc))
                .ReturnsAsync((Inventory)null)
                .ReturnsAsync(testResultInventoryItem);

            _mockInventoryRepo.Setup(m => m.AddItem(testAddInventoryItem));

            var actual = await _sut.AddInventoryItem(testAddInventoryItem);

            actual.Should().NotBeNull();
            actual.InventoryItemId.Should().Be(11);
            actual.Upc.Should().BeEquivalentTo(testAddInventoryItem.Upc);
        }

        [Fact]
        public async Task GetAllItems_ReturnsListOfInventoryItems()
        {
            _mockInventoryRepo.Setup(m => m.GetAll())
                .ReturnsAsync(new List<Inventory>{
                    new Inventory
                    {
                        InventoryItemId = 12,
                        Upc="12223242",
                        Description="TestItem1"
                    },
                    new Inventory
                    {
                        InventoryItemId = 13,
                        Upc="13233343",
                        Description="TestItem2"
                    } });

            var actual = await _sut.GetAllInventoryItems();

            actual.Should().NotBeNull();
            actual.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem_WhenInventoryItemIdIsNotNull()
        {
            _mockInventoryRepo.Setup(m => m.GetItemByInventoryItemId(14))
                .ReturnsAsync(new Inventory
                {
                    InventoryItemId = 14,
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
                    InventoryItemId = 15,
                    Upc = "15253545",
                    Description = "TestItem5"
                });

            var actual = await _sut.GetSingleInventoryItem(null, "15253545");

            actual.Should().NotBeNull();
            actual.InventoryItemId.Should().Be(15);
        }

        [Fact]
        public async Task GetItem_ThrowsException_WhenBothUpc_AndInventoryItemId_AreNull()
        {
            await _sut.Invoking(m => m.GetSingleInventoryItem(null, null))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage("You must supply an InventoryItemId or a Upc to select a single InventoryItem.");
        }

        [Fact]
        public async Task UpdateItem_ThrowsException_WhenRequestedItemIsInvalid()
        {
            await _sut.Invoking(m => m.UpdateInventoryItem(new Inventory()))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage("You must provide an Inventory Item to Update");
        }

        [Fact]
        public async Task UpdateItem_ThrowsException_WhenRequestedItemDoesNotExist()
        {
            _mockInventoryRepo.Setup(m => m.GetItemByUpc("16263646"))
                .ReturnsAsync((Inventory)null);

            await _sut.Invoking(m => m.UpdateInventoryItem(new Inventory { Upc = "16263646" }))
                .Should().ThrowAsync<InventoryServiceException>()
                .WithMessage("Upc 16263646 does not exist in the inventory database.");
        }

        [Fact]
        public async Task UpdateItem_SetsQuantity_WhenRequestedItemIsValid()
        {
            var testInventoryItem = new Inventory
            {
                InventoryItemId = 17,
                Upc = "17273747",
                Quantity = 7
            };

            _mockInventoryRepo.Setup(m => m.GetItemByUpc("17273747"))
                .ReturnsAsync(testInventoryItem);

            testInventoryItem.Quantity = 12;

            _mockInventoryRepo.Setup(m => m.UpdateItem(testInventoryItem));

            var actual = await _sut.UpdateInventoryItem(testInventoryItem);

            actual.Should().NotBeNull();
            actual.InventoryItemId.Should().Be(17);
            actual.Quantity.Should().Be(12);
        }

        [Fact]
        public async Task DeleteItem_Deletes()
        {
            _mockInventoryRepo.Setup(m => m.DeleteItem(18));

            await _sut.DeleteInventoryItem(18);

            _mockInventoryRepo.Verify();
        }
    }
}
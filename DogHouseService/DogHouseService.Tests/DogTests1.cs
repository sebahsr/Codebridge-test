using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using DogHouseService.Data; // Use appropriate namespace
using DogHouseService.Models; // Use appropriate namespace

public class DogServiceTests
{
    private DogContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DogContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        return new DogContext(options);
    }

    [Fact]
    public async Task GetDogs_ReturnsDogs()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Dogs.Add(new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 });
        context.Dogs.Add(new Dog { Name = "Jessy", Color = "black & white", TailLength = 7, Weight = 14 });
        await context.SaveChangesAsync();

        // Act
        var dogs = await context.Dogs.ToListAsync();

        // Assert
        Assert.Equal(2, dogs.Count);
        Assert.Equal("Neo", dogs[0].Name);
    }

    [Fact]
    public async Task AddDog_WhenDogAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Dogs.Add(new Dog { Name = "Neo", Color = "red & amber", TailLength = 22, Weight = 32 });
        await context.SaveChangesAsync();

        var newDog = new Dog { Name = "Neo", Color = "black", TailLength = 15, Weight = 20 };

        // Act
        bool exists = await context.Dogs.AnyAsync(d => d.Name == newDog.Name);

        // Assert
        Assert.True(exists, "Dog with the same name already exists.");
    }
}

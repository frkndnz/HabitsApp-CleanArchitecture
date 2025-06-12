using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HabitsApp.Application.Services;
using HabitsApp.Application.Stats;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using Moq;
using Xunit;
using Xunit.Abstractions;
namespace HabitsApp.Application.UnitTests.Stats;
public class GetCategoryStatsQueryHandlerTests
{
    private readonly Mock<IHabitRepository> habitRepositoryMock;
    private readonly Mock<IHabitLogRepository> logRepositoryMock;
    private readonly Mock<ICategoryRepository> categoryRepositoryMock;
    private readonly Mock<ICurrentUserService> userServiceMock;
    private readonly GetCategoryStatsQueryHandler handler;
    private readonly ITestOutputHelper _output;

    public GetCategoryStatsQueryHandlerTests(ITestOutputHelper output)
    {
        habitRepositoryMock = new Mock<IHabitRepository>();
        logRepositoryMock = new Mock<IHabitLogRepository>();
        categoryRepositoryMock = new Mock<ICategoryRepository>();
        userServiceMock = new Mock<ICurrentUserService>();
        handler = new GetCategoryStatsQueryHandler(
            categoryRepositoryMock.Object, habitRepositoryMock.Object, logRepositoryMock.Object, userServiceMock.Object
            );
        _output = output;
    }

    [Fact]
    public async Task Handle_WhenCalled_ShouldReturnCategoryStats()
    {
        var userId = Guid.NewGuid();
        userServiceMock.Setup(s => s.UserId)
            .Returns(userId);

        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var habitId1 = Guid.NewGuid();
        var habitId2 = Guid.NewGuid();
        var habitId3 = Guid.NewGuid();

        var fixedToday = new DateTime(2025, 6, 11);


        var habits = new List<Habit>()
{
    new Habit { Id = habitId1, Name = "Spor", CategoryId = categoryId1, CreatedAt = fixedToday.AddDays(-10), CreateUserId = userId },
    new Habit { Id = habitId2, Name = "Kitap", CategoryId = categoryId2, CreatedAt = fixedToday.AddDays(-5), CreateUserId = userId },
    new Habit { Id = habitId3, Name = "Meditasyon", CategoryId = null, CreatedAt = fixedToday.AddDays(-3), CreateUserId = userId }, // Kategorisiz habit
};

        var categories = new List<Category>
{
    new Category { Id = categoryId1, Name = "Sağlık", CreateUserId = userId },
    new Category { Id = categoryId2, Name = "Eğitim", CreateUserId = userId }
};

        var logs = new List<HabitLog>
{
    // Spor - 10 günde 8 log (72.73%)
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-9), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-8), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-7), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-6), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-5), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-4), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-3), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId1, Date = fixedToday.AddDays(-2), CreateUserId = userId },
    
    // Kitap - 5 günde 3 log (60%)
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId2, Date = fixedToday.AddDays(-4), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId2, Date = fixedToday.AddDays(-3), CreateUserId = userId },
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId2, Date = fixedToday.AddDays(-1), CreateUserId = userId },
    
    // Meditasyon - 3 günde 1 log (33.33%)
    new HabitLog { Id = Guid.NewGuid(), HabitId = habitId3, Date = fixedToday.AddDays(-2), CreateUserId = userId },
};

        habitRepositoryMock.Setup(x => x.GetAll()).Returns(habits.AsQueryable());
        categoryRepositoryMock.Setup(x => x.GetAll()).Returns(categories.AsQueryable());
        logRepositoryMock.Setup(x => x.GetAll()).Returns(logs.AsQueryable());

        var request = new GetCategoryStatsQuery();
        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        _output.WriteLine("testtt");
        Assert.True(true);

        _output.WriteLine($"result.Value is null? {result.Value == null}");
        _output.WriteLine($"result.Value count: {result.Value?.Count ?? 0}");

        foreach (var item in result.Value)
        {
            _output.WriteLine($"Category: {item.CategoryName}, Count: {item.HabitCount}, SuccessRate: {item.SuccessRate}");


        }

        var saglikCategory = result.Value.FirstOrDefault(x => x.CategoryName == "Sağlık");
        saglikCategory.Should().NotBeNull();
        saglikCategory.HabitCount.Should().Be(1);
        saglikCategory.SuccessRate.Should().BeApproximately(72.73m, 0.01m); // 8/10 * 100

        var egitimCategory = result.Value.FirstOrDefault(x => x.CategoryName == "Eğitim");
        egitimCategory.Should().NotBeNull();
        egitimCategory.HabitCount.Should().Be(1);
        egitimCategory.SuccessRate.Should().Be(50.00m); // 3/5 * 100

        var kategorisizCategory = result.Value!.FirstOrDefault(x => x.CategoryName == "Kategorisiz");
        kategorisizCategory.Should().NotBeNull();
        kategorisizCategory.HabitCount.Should().Be(1);
        kategorisizCategory.SuccessRate.Should().Be(33.33m); // 1/3 * 100
    }
}

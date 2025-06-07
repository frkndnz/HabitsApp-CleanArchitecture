using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HabitsApp.Application.Services;
using HabitsApp.Application.Stats;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using Moq;
using Xunit;
namespace HabitsApp.Application.UnitTests.Stats;
public class GetHabitDetailsQueryHandlerTests
{
    [Fact]
    public async Task Dummy_Test()
    {
       var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid(); 
        var today = DateTime.UtcNow.Date;

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(s => s.UserId)
            .Returns(userId);

        var mockHabitRepository = new Mock<IHabitRepository>();
        mockHabitRepository.Setup(r => r.GetAll())
            .Returns(new List<Habit>
            {
                new Habit
                {
                    Id = habitId,
                    Name = "Test Habit",
                    Color = "Blue",
                    CreateUserId = userId
                }
            }.AsQueryable());

        var mockHabitLogRepository = new Mock<IHabitLogRepository>();
        mockHabitLogRepository.Setup(r => r.GetAll())
            .Returns(new List<HabitLog>
            {
                new HabitLog{HabitId=habitId,CreateUserId=userId,Date=today.AddDays(-2) },
                new HabitLog{HabitId=habitId,CreateUserId=userId,Date=today.AddDays(-1) },
                new HabitLog{HabitId=habitId,CreateUserId=userId,Date=today }
            }.AsQueryable());

        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var handler=new GetHabitDetailsQueryHandler(
            mockHabitRepository.Object,
            mockHabitLogRepository.Object,
            mockCurrentUserService.Object,
            mockUnitOfWork.Object
        );

        var query = new GetHabitDetailsQuery();
        var result =await  handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrEmpty();
        result.Value.Count.Should().Be(1);

        var habitDetails = result.Value.First();
        habitDetails.Id.Should().Be(habitId);
        habitDetails.Name.Should().Be("Test Habit");
        habitDetails.Color.Should().Be("Blue");
        habitDetails.IsCompletedToday.Should().BeTrue();
        habitDetails.CurrentStreak.Should().Be(3); // 3 days including today
        habitDetails.BestStreak.Should().Be(3); // Best streak is also 3 days
        habitDetails.TotalCompletions.Should().Be(3); // Total completions are 3 days


    }
}

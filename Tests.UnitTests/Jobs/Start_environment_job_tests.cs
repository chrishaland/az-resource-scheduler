using Hangfire;
using Service.Jobs;
using Nager.Date;
using Hangfire.Common;
using Hangfire.States;

namespace Tests.UnitTests.Jobs;

[TestFixture]
public class Start_environment_job_tests
{
    private StartEnvironmentJob _sut;
    private TestDatabase _database;
    private Mock<IBackgroundJobClient> _backgroundJobClientMock;

    [SetUp]
    public void Before()
    {
        _database = new TestDatabase();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

        _sut = new StartEnvironmentJob(
            logger: TestLoggerFactory.GetLogger<StartEnvironmentJob>(),
            context: _database,
            backgroundJob: _backgroundJobClientMock.Object
        );
    }

    [Test]
    public async Task Starts_environment_and_schedules_stop_time()
    {
        var resource = await _database.Resources.AddAsync(new Repository.Models.Resource { Name = "", Description = "", ResourceGroup = "" });
        var environment = await _database.Environments.AddAsync(new Repository.Models.Environment { Name = "", Description = "" });
        environment.Entity.Resources.Add(resource.Entity);
        await _database.SaveChangesAsync();

        await _sut.Execute(
            environmentId: environment.Entity.Id,
            uptimeInMinutes: 30,
            allowWeekendRuns: true,
            allowHolidayRuns: true,
            currentTime: DateTime.Now
        );

        var registredStopJobs = await _database.ResourceStopJobs
            .Where(j => j.ResourceId.Equals(resource.Entity.Id))
            .ToListAsync();

        Assert.Multiple(() =>
        {
            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StartResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<EnqueuedState>()),
                Times.Once);

            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StopResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<ScheduledState>()),
                Times.Once);

            CollectionAssert.IsNotEmpty(registredStopJobs);
            Assert.That(registredStopJobs.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Starts_environment_and_removes_older_stop_jobs()
    {
        var resourceStopJob = await _database.ResourceStopJobs.AddAsync(new Repository.Models.ResourceStopJob { JobId = "" });
        var resource = await _database.Resources.AddAsync(new Repository.Models.Resource { Name = "", Description = "", ResourceGroup = "" });
        resource.Entity.ResourceStopJobs.Add(resourceStopJob.Entity);
        var environment = await _database.Environments.AddAsync(new Repository.Models.Environment { Name = "", Description = "" });
        environment.Entity.Resources.Add(resource.Entity);
        await _database.SaveChangesAsync();

        await _sut.Execute(
            environmentId: environment.Entity.Id,
            uptimeInMinutes: 30,
            allowWeekendRuns: true,
            allowHolidayRuns: true,
            currentTime: DateTime.Now
        );

        var registredStopJobs = await _database.ResourceStopJobs
            .Where(j => j.ResourceId.Equals(resource.Entity.Id))
            .ToListAsync();

        Assert.Multiple(() =>
        {
            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StartResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<EnqueuedState>()),
                Times.Once);

            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StopResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<ScheduledState>()),
                Times.Once);

            CollectionAssert.IsNotEmpty(registredStopJobs);
            Assert.That(registredStopJobs.Count, Is.EqualTo(1));
            Assert.That(registredStopJobs[0].Id, Is.Not.EqualTo(resourceStopJob.Entity.Id));
        });
    }

    [Test]
    public async Task Starts_environment_and_keeps_stop_job_after_expected_uptime()
    {
        var resourceStopJob = await _database.ResourceStopJobs.AddAsync(new Repository.Models.ResourceStopJob { JobId = "", StopAt = DateTime.Now.AddMinutes(120) });
        var resource = await _database.Resources.AddAsync(new Repository.Models.Resource { Name = "", Description = "", ResourceGroup = "" });
        resource.Entity.ResourceStopJobs.Add(resourceStopJob.Entity);
        var environment = await _database.Environments.AddAsync(new Repository.Models.Environment { Name = "", Description = "" });
        environment.Entity.Resources.Add(resource.Entity);
        await _database.SaveChangesAsync();

        await _sut.Execute(
            environmentId: environment.Entity.Id,
            uptimeInMinutes: 30,
            allowWeekendRuns: true,
            allowHolidayRuns: true,
            currentTime: DateTime.Now
        );

        var registredStopJobs = await _database.ResourceStopJobs
            .Where(j => j.ResourceId.Equals(resource.Entity.Id))
            .ToListAsync();

        Assert.Multiple(() =>
        {
            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StartResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<EnqueuedState>()),
                Times.Once);

            _backgroundJobClientMock.Verify(m =>
                m.Create(
                    It.Is<Job>(j => j.Type == typeof(StopResourceJob) &&
                                    j.Args[0].Equals(resource.Entity.Id)),
                    It.IsAny<ScheduledState>()),
                Times.Never);

            CollectionAssert.IsNotEmpty(registredStopJobs);
            Assert.That(registredStopJobs.Count, Is.EqualTo(1));
            Assert.That(registredStopJobs[0].Id, Is.EqualTo(resourceStopJob.Entity.Id));
        });
    }

    [TestCase(2019)]
    [TestCase(2020)]
    [TestCase(2021)]
    [TestCase(2022)]
    public async Task Does_not_run_on_holidays(int year)
    {
        var environment = await _database.Environments.AddAsync(new Repository.Models.Environment { Name = "", Description = "" });
        await _database.SaveChangesAsync();

        var holidays = DateSystem.GetPublicHolidays(new DateTime(year, 1, 1), new DateTime(year, 12, 31));

        foreach (var holiday in holidays)
        {
            await _sut.Execute(
                environmentId: environment.Entity.Id,
                uptimeInMinutes: 1,
                allowWeekendRuns: true,
                allowHolidayRuns: false,
                currentTime: holiday.Date
            );
        }

        _backgroundJobClientMock.Verify(m =>
            m.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
    }

    [TestCase(2019)]
    [TestCase(2020)]
    [TestCase(2021)]
    [TestCase(2022)]
    public async Task Does_not_run_on_weekends(int year)
    {
        var environment = await _database.Environments.AddAsync(new Repository.Models.Environment { Name = "", Description = "" });
        await _database.SaveChangesAsync();

        var day = new DateTime(year, 1, 1);
        do
        {
            if (DateSystem.IsWeekend(day, CountryCode.NO))
            {
                await _sut.Execute(
                    environmentId: environment.Entity.Id,
                    uptimeInMinutes: 1,
                    allowWeekendRuns: false,
                    allowHolidayRuns: true,
                    currentTime: day
                );
            }
            day = day.AddDays(1);
        } while (day.Year == year);

        _backgroundJobClientMock.Verify(m =>
            m.Create(It.IsAny<Job>(), It.IsAny<EnqueuedState>()), Times.Never);
    }
}

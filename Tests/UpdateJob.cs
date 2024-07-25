using CronicleClient;
using CronicleClient.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Tests;

public class UpdateJob
{
    private readonly Client _cronicleClient;
    private readonly CancellationToken _cancellationToken;

    public UpdateJob(ITestOutputHelper outputHelper)
    {
        var serverUrl = "http://localhost:3012";
        var apiKey = "240db114e152267349f17cf768808169";
        var logger = outputHelper.ToLogger<CreateEvent>();

        _cronicleClient = new Client(baseUrl: serverUrl, apiToken: apiKey, logger: logger);
        _cancellationToken = new CancellationTokenSource().Token;
    }

    [Fact(DisplayName = "Update an Job")]
    public async Task UpdateAnJob()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for job update",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId =  await _cronicleClient.Event.RunEventById(newEventId);
        var jobsData = await _cronicleClient.Job.GetActiveJobs();
        jobsData.Should().NotBeEmpty();
        var jobData = jobsData.FirstOrDefault(p => p.Key == jobId[0]).Value;
        jobData.Should().NotBeNull();
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = jobData.Id,
            NotifySuccess = "succes@yopmail.com",
            Timeout = 600
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().NotThrowAsync<Exception>();

        // Cleanup
        await Task.Delay(61000);
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update an Job and Validate")]
    public async Task UpdateAnJobValidate()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "A title for job update",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId = await _cronicleClient.Event.RunEventById(newEventId);
        var jobsData = await _cronicleClient.Job.GetActiveJobs();
        jobsData.Should().NotBeEmpty();
        var jobData = jobsData.FirstOrDefault(p => p.Key == jobId[0]).Value;
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = jobData.Id,
            NotifySuccess = "succes@yopmail.com",
            Timeout = 600
        };

        // Act
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().NotThrowAsync<Exception>();

        // Assert
        var jobsDataAfterUpdate = await _cronicleClient.Job.GetActiveJobs();
        jobsDataAfterUpdate.Should().NotBeEmpty();
        var jobDataAfterUpdate = jobsDataAfterUpdate.FirstOrDefault(p => p.Key == jobId[0]).Value;
        jobDataAfterUpdate.Should().NotBeNull();
        jobDataAfterUpdate.NotifySuccess.Should().Be(updatedJob.NotifySuccess);
        jobDataAfterUpdate.Timeout.Should().Be(updatedJob.Timeout);

        // Cleanup
        await Task.Delay(61000);
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update the CPU limit of a Job")]
    public async Task UpdateJobCpuLimit()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Event to update job CPU limit",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId = await _cronicleClient.Event.RunEventById(newEventId);
        var jobsData = await _cronicleClient.Job.GetActiveJobs();
        jobsData.Should().NotBeEmpty();
        var job = jobsData.FirstOrDefault(p => p.Key == jobId[0]).Value;
        job.Should().NotBeNull();
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = job.Id,
            CpuLimit = 200,
            CpuSustain = 120
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().NotThrowAsync<Exception>();

        // Cleanup
        await Task.Delay(61000);
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update an not-existent Job")]
    public async Task UpdateNotExistentJob()
    {
        // Arrange
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = "not_existent_job",
            Timeout = 300
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Update an invalid Job")]
    public async Task UpdateInvalidJob()
    {
        // Arrange
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = string.Empty,
            Timeout = 200
        };

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().ThrowAsync<Exception>();
    }


    [Fact(DisplayName = "Update a completed Job")]
    public async Task UpdateJobCompleted()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Event to update job completed",
            Enabled = true,
            Category = "general",
            Plugin = "plyyyhtht1w",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId = await _cronicleClient.Event.RunEventById(newEventId);
        var jobsData = await _cronicleClient.Job.GetActiveJobs();
        jobsData.Should().NotBeEmpty();
        var job = jobsData.FirstOrDefault(p => p.Key == jobId[0]).Value;
        job.Should().NotBeNull();
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = job.Id,
            CpuLimit = 200,
            CpuSustain = 120
        };
        await Task.Delay(500);


        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().ThrowAsync<KeyNotFoundException>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

    [Fact(DisplayName = "Update an aborted Job")]
    public async Task UpdateJobAborted()
    {
        // Arrange
        var newEvent = new NewEvent()
        {
            Title = "Event to update job CPU aborted",
            Enabled = true,
            Category = "general",
            Plugin = "testplug",
            Target = "allgrp",
            Timing = new Timing()
            {
                Hours = [4],
                Minutes = [27],
                Days = [25],
                Months = [8],
                Years = [2024]
            }
        };
        var newEventId = await _cronicleClient.Event.Create(eventData: newEvent, cancellationToken: _cancellationToken);
        newEventId.Should().NotBeEmpty();

        var jobId = await _cronicleClient.Event.RunEventById(newEventId);

        var jobsData = await _cronicleClient.Job.GetActiveJobs();
        jobsData.Should().NotBeEmpty();

        var job = jobsData.FirstOrDefault(p => p.Key == jobId[0]).Value;
        job.Should().NotBeNull();
        var updatedJob = new JobDataUpdateRequest()
        {
            Id = job.Id,
            CpuLimit = 200,
            CpuSustain = 120
        };

        await FluentActions.Invoking(() => _cronicleClient.Job.AbortJob(job.Id, _cancellationToken))
            .Should().NotThrowAsync<Exception>();
        await Task.Delay(1500);

        // Act & Assert
        await FluentActions.Invoking(() => _cronicleClient.Job.Update(updatedJob, _cancellationToken))
          .Should().ThrowAsync<KeyNotFoundException>();

        // Cleanup
        await _cronicleClient.Event.Delete(eventId: newEventId, cancellationToken: _cancellationToken);
    }

}

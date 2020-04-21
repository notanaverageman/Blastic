using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Data.Tables;

namespace Blastic.Forms.Sample.Data.Tables
{
	public class JobsTable : TableBase
	{
		public JobsTable(ConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

		public async Task<List<Job>> GetAll(Machine machine, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM Jobs WHERE MachineId=@MachineId";
			command.AddParameterWithValue("@MachineId", machine.Id);

			List<Job> jobs = new List<Job>();

			using DataReader reader = await command.ExecuteReader(cancellationToken);

			while (reader.Read())
			{
				Job job = CreateJob(reader, machine);
				jobs.Add(job);
			}

			return jobs;
		}

		private Job CreateJob(DataReader reader, Machine machine)
		{
			int id = reader.Get<int>("Id");

			string sceneName = reader.Get<string>("SceneName");
			bool isStarted = reader.Get<bool>("IsStarted");
			DateTime queuedDate = reader.Get<DateTime>("QueueDate");
			DateTime startedDate = reader.Get<DateTime>("StartDate");
			int startFrame = reader.Get<int>("StartFrame");
			int endFrame = reader.Get<int>("EndFrame");

			Job job = new Job(id, machine);

			job.SceneName.Value = sceneName;
			job.IsStarted.Value = isStarted;
			job.QueueDate.Value = queuedDate;
			job.StartDate.Value = startedDate;
			job.StartFrame.Value = startFrame;
			job.EndFrame.Value = endFrame;

			return job;
		}

		public async Task Put(Job job, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();

			if (await Contains(connection, job.Id, cancellationToken))
			{
				await Update(connection, job, cancellationToken);
			}
			else
			{
				await Insert(connection, job, cancellationToken);
			}
		}

		private async Task<bool> Contains(Connection connection, int id, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT 1 FROM Jobs WHERE Id=@Id";
			command.AddParameterWithValue("@Id", id);

			using DataReader reader = await command.ExecuteReader(cancellationToken);
			return reader.HasRows;
		}

		public async Task Delete(int id, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = @"DELETE FROM Jobs WHERE Id=@Id";
			command.AddParameterWithValue("@Id", id);

			await command.ExecuteNonQuery(cancellationToken);
		}

		private async Task Insert(Connection connection, Job job, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"INSERT INTO Jobs (MachineId, SceneName, IsStarted, QueueDate, StartDate, StartFrame, EndFrame)
									VALUES (@MachineId, @SceneName, @IsStarted, @QueueDate, @StartDate, @StartFrame, @EndFrame)";

			command.AddParameterWithValue("@MachineId", job.Machine.Value.Id);
			command.AddParameterWithValue("@SceneName", job.SceneName.Value);
			command.AddParameterWithValue("@IsStarted", job.IsStarted.Value);
			command.AddParameterWithValue("@QueueDate", job.QueueDate.Value);
			command.AddParameterWithValue("@StartDate", job.StartDate.Value);
			command.AddParameterWithValue("@StartFrame", job.StartFrame.Value);
			command.AddParameterWithValue("@EndFrame", job.EndFrame.Value);

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			job.Id = await providerSpecifics.ExecuteAndGetInsertedRowId(command, "Jobs", cancellationToken);
		}

		private async Task Update(Connection connection, Job job, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"UPDATE Jobs SET
										MachineId=@MachineId,
										SceneName=@SceneName,
										IsStarted=@IsStarted,
										QueueDate=@QueueDate,
										StartDate=@StartDate,
										StartFrame=@StartFrame,
										EndFrame=@EndFrame
									WHERE Id=@Id";

			command.AddParameterWithValue("@MachineId", job.Machine.Value.Id);
			command.AddParameterWithValue("@SceneName", job.SceneName.Value);
			command.AddParameterWithValue("@IsStarted", job.IsStarted.Value);
			command.AddParameterWithValue("@QueueDate", job.QueueDate.Value);
			command.AddParameterWithValue("@StartDate", job.StartDate.Value);
			command.AddParameterWithValue("@StartFrame", job.StartFrame.Value);
			command.AddParameterWithValue("@EndFrame", job.EndFrame.Value);
			command.AddParameterWithValue("@Id", job.Id);

			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blastic.Data;
using Blastic.Data.ProviderSpecific;
using Blastic.Data.Tables;
using Blastic.Exceptions;

namespace Blastic.Forms.Sample.Data.Tables
{
	public class MachinesTable : TableBase
	{
		private readonly JobsTable _jobsTable;

		public MachinesTable(
			ConnectionFactory connectionFactory,
			JobsTable jobsTable)
			:
			base(connectionFactory)
		{
			_jobsTable = jobsTable;
		}

		public async Task<List<Machine>> GetAll(CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM Machines";

			List<Machine> machines = new List<Machine>();

			using DataReader reader = await command.ExecuteReader(cancellationToken);

			while (reader.Read())
			{
				Machine machine = await CreateMachine(reader, cancellationToken);
				machines.Add(machine);
			}

			return machines;
		}

		public async Task<Machine> Get(string name, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT * FROM Machines WHERE Name=@Name";
			command.AddParameterWithValue("@Name", name);

			using DataReader reader = await command.ExecuteReader(cancellationToken);

			if (!reader.Read())
			{
				throw new NotFoundException($"Machine with name {name} not found.");
			}

			return await CreateMachine(reader, cancellationToken);
		}

		private async Task<Machine> CreateMachine(DataReader reader, CancellationToken cancellationToken)
		{
			int id = reader.Get<int>("Id");
			string name = reader.Get<string>("Name");
			int secondsPerFrame = reader.Get<int>("SecondsPerFrame");

			Machine machine = new Machine();

			machine.Id = id;
			machine.Name.Value = name;
			machine.SecondsPerFrame.Value = secondsPerFrame;

			List<Job> jobs = await _jobsTable.GetAll(machine, cancellationToken);

			machine.Jobs.AddRange(jobs);

			return machine;
		}

		public async Task Put(Machine machine, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();

			if (await Contains(connection, machine.Id, cancellationToken))
			{
				await Update(connection, machine, cancellationToken);
			}
			else
			{
				await Insert(connection, machine, cancellationToken);
			}
		}

		private async Task<bool> Contains(Connection connection, int id, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = "SELECT 1 FROM Machines WHERE Id=@Id";
			command.AddParameterWithValue("@Id", id);

			using DataReader reader = await command.ExecuteReader(cancellationToken);
			return reader.HasRows;
		}

		public async Task Delete(int id, CancellationToken cancellationToken)
		{
			using Connection connection = ConnectionFactory.CreateConnection();
			using Command command = connection.CreateCommand();

			command.CommandText = @"DELETE FROM Machines WHERE Id=@Id";
			command.AddParameterWithValue("@Id", id);

			await command.ExecuteNonQuery(cancellationToken);
		}

		private async Task Insert(Connection connection, Machine machine, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"INSERT INTO Machines (Name, SecondsPerFrame) VALUES (@Name, @SecondsPerFrame)";

			command.AddParameterWithValue("@Name", machine.Name.Value);
			command.AddParameterWithValue("@SecondsPerFrame", machine.SecondsPerFrame.Value);

			ProviderSpecifics providerSpecifics = connection.ProviderSpecifics;

			machine.Id = await providerSpecifics.ExecuteAndGetInsertedRowId(command, "Machines", cancellationToken);
		}

		private async Task Update(Connection connection, Machine machine, CancellationToken cancellationToken)
		{
			using Command command = connection.CreateCommand();

			command.CommandText = @"UPDATE Machines SET Name=@Name SecondsPerFrame=@SecondsPerFrame WHERE Id=@Id";

			command.AddParameterWithValue("@Name", machine.Name.Value);
			command.AddParameterWithValue("@SecondsPerFrame", machine.SecondsPerFrame.Value);
			command.AddParameterWithValue("@Id", machine.Id);

			await command.ExecuteNonQuery(cancellationToken);
		}
	}
}
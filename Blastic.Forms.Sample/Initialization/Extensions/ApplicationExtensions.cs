using System;
using System.IO;
using Blastic.Data;
using Blastic.Forms.Data.Extensions;
using Blastic.Forms.Data.Steps;
using Blastic.Forms.Initialization;
using Blastic.Forms.Initialization.Extensions;
using Blastic.Forms.Sample.UserInterface;
using Blastic.UserInterface.Settings.Steps;

namespace Blastic.Forms.Sample.Initialization.Extensions
{
	public static class ApplicationExtensions
	{
		public static BlasticApplication Initialize(this BlasticApplication application)
		{
			string databasePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"Settings.sqlite");

			application
				.AddLocalizationSource(Properties.Resources.ResourceManager)
				.AddInitializationStep<MigrateProgramDatabaseStep>()
				.AddInitializationStep<ReadSettingsStep>()
				.AddShellTab<HomeViewModel>()
				.AddShellTab<TestViewModel>()
				.AddProgramDatabase(DatabaseProvider.SQLite, $"Data Source={databasePath};")
				.AddSettingsService();

			return application;
		}
	}
}
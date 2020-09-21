using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Dapper;
using System.Data.SqlClient;
using DbUp;
using System.Reflection;

namespace DbUpFootball
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server = localhost; Database = soccer; Trusted_Connection = True";

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            Team barcelona = new Team { Id = 1, Name = "Барселона" };
            Team realMadrid = new Team { Id = 2, Name = "Реал Мадрид" };
            Team arsenal = new Team { Id = 3, Name = "Арсенал" };

            NationalTeam spainNationalTeam = new NationalTeam { Name = "Сборная Испании" };
            NationalTeam englandNationalTeam = new NationalTeam { Name = "Сборная Англии" };

            Player player1 = new Player { Id = 1, Name = "Серхио Регилон", Position = "Защитник", Team = realMadrid, NationalTeam = spainNationalTeam };
            Player player2 = new Player { Id = 2, Name = "Серхио Бускетс", Position = "Полузащитник", Team = barcelona, NationalTeam = spainNationalTeam };
            Player player3 = new Player { Id = 3, Name = "Ансу Фати", Position = "Нападающий", Team = barcelona, NationalTeam = spainNationalTeam };
            Player player4 = new Player { Id = 4, Name = "Энзли Мейтленд-Найлз", Position = "Полузащитник", Team = arsenal, NationalTeam = englandNationalTeam };

            DbProviderFactories.RegisterFactory("provider", SqlClientFactory.Instance);

            var factory = DbProviderFactories.GetFactory("provider");

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Execute("insert into Teams (Id, Name) values (@Id, @Name)", barcelona);
                connection.Execute("insert into Teams (Id, Name) values (@Id, @Name)", realMadrid);
                connection.Execute("insert into Teams (Id, Name) values (@Id, @Name)", arsenal);

                connection.Execute("insert into NationalTeams (Id, Name) values (@Id, @Name)", spainNationalTeam);
                connection.Execute("insert into NationalTeams (Id, Name) values (@Id, @Name)", englandNationalTeam);

                connection.Execute("insert into Players (Id, Name, Position, Team) values (@Id, @Name, @Position, @Team)", player1);
                connection.Execute("insert into Players (Id, Name, Position, Team) values (@Id, @Name, @Position, @Team)", player2);
                connection.Execute("insert into Players (Id, Name, Position, Team) values (@Id, @Name, @Position, @Team)", player3);
                connection.Execute("insert into Players (Id, Name, Position, Team) values (@Id, @Name, @Position, @Team)", player4);
            }
        }
    }
}

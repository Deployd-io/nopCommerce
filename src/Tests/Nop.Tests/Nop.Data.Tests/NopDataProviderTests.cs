using System.Text;
using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Gdpr;
using Nop.Data;
using Nop.Web.Models.Install;
using NUnit.Framework;

namespace Nop.Tests.Nop.Data.Tests;

[TestFixture]
public class NopDataProviderTests : BaseNopTest
{
    [TearDown]
    public async Task TearDown()
    {
		logger.LogDebug(">>> Entering TearDown()");
        try
        {
            var dataProvider = GetService<INopDataProvider>();
            await dataProvider.TruncateAsync<GdprConsent>(true);
			logger.LogDebug("TearDown(): dataProvider → {DataProvider}", dataProvider);
        }
        catch
        {
			logger.LogError("Exception in TearDown(): {Arg0}", ex.Message);
            //ignore 
        }

        SetDataProviderType(DataProviderType.Unknown);
		logger.LogDebug("<<< Exiting TearDown()");
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public void CanCreateForeignKeyName(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanCreateForeignKeyName(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanCreateForeignKeyName(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var foreignTable = "foreignTable";
        var foreignColumn = "foreignColumn";
		logger.LogDebug("CanCreateForeignKeyName(type={Type}): foreignTable → {ForeignTable}", type, foreignTable);
		logger.LogDebug("CanCreateForeignKeyName(type={Type}): foreignColumn → {ForeignColumn}", type, foreignColumn);
        var primaryTable = "primaryTable";
        var primaryColumn = "primaryColumn";
		logger.LogDebug("CanCreateForeignKeyName(type={Type}): primaryTable → {PrimaryTable}", type, primaryTable);
		logger.LogDebug("CanCreateForeignKeyName(type={Type}): primaryColumn → {PrimaryColumn}", type, primaryColumn);

        var foreignKeyName =
            dataProvider.CreateForeignKeyName(foreignTable, foreignColumn, primaryTable, primaryColumn);

        switch (type)
        {

            case DataProviderType.SqlServer:
            case DataProviderType.PostgreSQL:
                foreignKeyName.Should()
                    .Be($"FK_{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}");
                break;

            case DataProviderType.MySql:
            case DataProviderType.Unknown:
                foreignKeyName.Should()
                    .Be("FK_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{foreignTable}_{foreignColumn}_{primaryTable}_{primaryColumn}"), "SHA1"));
                break;
        }
		logger.LogDebug("<<< Exiting CanCreateForeignKeyName(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public void CanGetIndexName(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetIndexName(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanGetIndexName(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var targetTable = "targetTable";
        var targetColumn = "targetColumn";
		logger.LogDebug("CanGetIndexName(type={Type}): targetTable → {TargetTable}", type, targetTable);
		logger.LogDebug("CanGetIndexName(type={Type}): targetColumn → {TargetColumn}", type, targetColumn);

        var indexName = dataProvider.GetIndexName(targetTable, targetColumn);

		logger.LogDebug("CanGetIndexName(type={Type}): indexName → {IndexName}", type, indexName);
        switch (type)
        {

            case DataProviderType.SqlServer:
            case DataProviderType.PostgreSQL:
                indexName.Should()
                    .Be($"IX_{targetTable}_{targetColumn}");
                break;

            case DataProviderType.MySql:
            case DataProviderType.Unknown:
                indexName.Should()
                    .Be("IX_" + HashHelper.CreateHash(Encoding.UTF8.GetBytes($"{targetTable}_{targetColumn}"), "SHA1"));
                break;
        }
		logger.LogDebug("<<< Exiting CanGetIndexName(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanCreateTempDataStorage(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanCreateTempDataStorage(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var dataProvider = GetService<INopDataProvider>();
        var productRepository = GetService<IRepository<Product>>();
		logger.LogDebug("CanCreateTempDataStorage(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
		logger.LogDebug("CanCreateTempDataStorage(type={Type}): productRepository → {ProductRepository}", type, productRepository);

        var tableName = "TestTempDataTable".ToLower();

		logger.LogDebug("CanCreateTempDataStorage(type={Type}): tableName → {TableName}", type, tableName);
        await using var data = await dataProvider.CreateTempDataStorageAsync(tableName,
            productRepository.Table.Select(p => new { p.Name, p.Id, p.Deleted }));

        data.Count().Should().Be(productRepository.GetAll(query => query).Count);
		logger.LogInformation("CanCreateTempDataStorage(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);

        var rez = await dataProvider.QueryAsync<object>($"select * from {tableName}");

		logger.LogDebug("CanCreateTempDataStorage(type={Type}): rez → {Rez}", type, rez);
        rez.Count.Should().Be(data.Count());
		logger.LogDebug("<<< Exiting CanCreateTempDataStorage(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanInsertEntity(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanInsertEntity(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanInsertEntity(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanInsertEntity(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.TruncateAsync<GdprConsent>();
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        await dataProvider.InsertEntityAsync(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("<<< Exiting CanInsertEntity(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanUpdateEntity(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanUpdateEntity(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanUpdateEntity(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanUpdateEntity(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        await dataProvider.UpdateEntityAsync(new GdprConsent
        {
            Id = gdprConsent.Id,
            Message = "Updated test tax category"
        });
        var updatedGdprConsent = dataProvider.GetTable<GdprConsent>().FirstOrDefault(tc => tc.Id == gdprConsent.Id);
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanUpdateEntity(type={Type}): updatedGdprConsent → {UpdatedGdprConsent}", type, updatedGdprConsent);

        gdprConsent.Id.Should().BeGreaterThan(0);
        updatedGdprConsent?.Message.Should().NotBeEquivalentTo(gdprConsent.Message);
		logger.LogDebug("<<< Exiting CanUpdateEntity(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanUpdateEntities(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanUpdateEntities(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanUpdateEntities(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanUpdateEntities(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        await dataProvider.UpdateEntitiesAsync(new[]
        {
            new GdprConsent
            {
                Id = gdprConsent.Id,
                Message = "Updated test tax category"
            }
        });
        var updatedGdprConsent = dataProvider.GetTable<GdprConsent>().FirstOrDefault(tc => tc.Id == gdprConsent.Id);
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanUpdateEntities(type={Type}): updatedGdprConsent → {UpdatedGdprConsent}", type, updatedGdprConsent);

        gdprConsent.Id.Should().BeGreaterThan(0);
        updatedGdprConsent?.Message.Should().NotBeEquivalentTo(gdprConsent.Message);
		logger.LogDebug("<<< Exiting CanUpdateEntities(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanDeleteEntity(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanDeleteEntity(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanDeleteEntity(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanDeleteEntity(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.DeleteEntityAsync(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
		logger.LogDebug("<<< Exiting CanDeleteEntity(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanBulkDeleteEntities(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanBulkDeleteEntities(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanBulkDeleteEntities(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanBulkDeleteEntities(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.BulkDeleteEntitiesAsync(new List<GdprConsent> { gdprConsent });
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.BulkDeleteEntitiesAsync<GdprConsent>(_ => true);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
		logger.LogDebug("<<< Exiting CanBulkDeleteEntities(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanBulkInsertEntities(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanBulkInsertEntities(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanBulkInsertEntities(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };

		logger.LogDebug("CanBulkInsertEntities(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        await dataProvider.TruncateAsync<GdprConsent>();
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        await dataProvider.BulkInsertEntitiesAsync(new[] { gdprConsent });
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("<<< Exiting CanBulkInsertEntities(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetTable(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetTable(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanGetTable(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("CanGetTable(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        dataProvider.InsertEntity(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.DeleteEntityAsync(gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
		logger.LogDebug("<<< Exiting CanGetTable(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanGetTableIdentAsync(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanGetTableIdentAsync(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanGetTableIdentAsync(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var gdprConsent = new GdprConsent { DisplayOrder = 10, Message = "Test message 1" };
        await dataProvider.TruncateAsync<GdprConsent>(true);
		logger.LogDebug("CanGetTableIdentAsync(type={Type}): gdprConsent → {GdprConsent}", type, gdprConsent);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        var nextId = await dataProvider.GetTableIdentAsync<GdprConsent>();

		logger.LogDebug("CanGetTableIdentAsync(type={Type}): nextId → {NextId}", type, nextId);
        nextId.Should().Be(type == DataProviderType.Unknown ? 0 : 1);

        dataProvider.InsertEntity(gdprConsent);

        nextId = await dataProvider.GetTableIdentAsync<GdprConsent>();
        if (type == DataProviderType.SqlServer || type == DataProviderType.Unknown)
            nextId.Should().Be(1);
        else
            nextId.Should().Be(2);

        dataProvider.InsertEntity(gdprConsent);

        nextId = await dataProvider.GetTableIdentAsync<GdprConsent>();

        if (type == DataProviderType.SqlServer || type == DataProviderType.Unknown)
            nextId.Should().Be(2);
        else
            nextId.Should().Be(3);

        await dataProvider.TruncateAsync<GdprConsent>(true);
		logger.LogDebug("<<< Exiting CanGetTableIdentAsync(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public void CanBuildConnectionString(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanBuildConnectionString(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanBuildConnectionString(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        var connStr = dataProvider.BuildConnectionString(new InstallModel
        {
            DatabaseName = "test_db",
            ServerName = "127.0.0.1",
            IntegratedSecurity = false,
            Username = "test",
            Password = "passwd"
        });

        switch (type)
        {
            case DataProviderType.Unknown:
                connStr.Should()
                    .Be(@$"Data Source={CommonHelper.DefaultFileProvider.MapPath($"~/App_Data/")}test_db.sqlite;Mode=ReadWrite;Cache=Shared;Password=passwd");
                break;
            case DataProviderType.SqlServer:
                connStr.Should()
                    .Be(@"Data Source=127.0.0.1;Initial Catalog=test_db;Integrated Security=False;Persist Security Info=False;User ID=test;Password=passwd;Trust Server Certificate=True");
                break;
            case DataProviderType.MySql:
                connStr.Should()
                    .Be(@"Server=127.0.0.1;User ID=test;Password=passwd;Database=test_db;Allow User Variables=True");
                break;
            case DataProviderType.PostgreSQL:
                connStr.Should()
                    .Be(@"Host=127.0.0.1;Database=test_db;Username=test;Password=passwd");
                break;
        }

        Assert.Throws<ArgumentNullException>(() => dataProvider.BuildConnectionString(null));

        if (type == DataProviderType.SqlServer)
            return;

        Assert.Throws<NopException>(() => dataProvider.BuildConnectionString(new InstallModel()
        {
            IntegratedSecurity = true
        }));
		logger.LogDebug("<<< Exiting CanBuildConnectionString(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanSetTableIdent(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanSetTableIdent(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanSetTableIdent(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        await dataProvider.TruncateAsync<GdprConsent>(true);
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
        var nextId = await dataProvider.GetTableIdentAsync<GdprConsent>();

		logger.LogDebug("CanSetTableIdent(type={Type}): nextId → {NextId}", type, nextId);
        nextId.Should().Be(type == DataProviderType.Unknown ? 0 : 1);

        await dataProvider.SetTableIdentAsync<GdprConsent>(10);
        nextId = await dataProvider.GetTableIdentAsync<GdprConsent>();

        nextId.Should().Be(10);
		logger.LogDebug("<<< Exiting CanSetTableIdent(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanExecuteNonQuery(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanExecuteNonQuery(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanExecuteNonQuery(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        if (type != DataProviderType.PostgreSQL)
        {
            var rez = await dataProvider.ExecuteNonQueryAsync("select * from Product");
            rez.Should().Be(-1);
			logger.LogDebug("CanExecuteNonQuery(type={Type}): rez → {Rez}", type, rez);

            rez = await dataProvider.ExecuteNonQueryAsync(
                "insert into GdprConsent(Message, IsRequired, RequiredMessage, DisplayDuringRegistration, DisplayOnCustomerInfoPage, DisplayOrder) values('test value', 0,'',0,0,1)");
				logger.LogInformation("CanExecuteNonQuery(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
            rez.Should().Be(1);

            rez = await dataProvider.ExecuteNonQueryAsync("delete from GdprConsent where Id=0");
            rez.Should().Be(0);
			logger.LogDebug("CanExecuteNonQuery(type={Type}): Id → {Id}", type, Id);
        }
        else
        {
            var rez = await dataProvider.ExecuteNonQueryAsync("select * from \"Product\"");
            rez.Should().Be(-1);

            rez = await dataProvider.ExecuteNonQueryAsync(
                "insert into \"GdprConsent\"(\"Message\", \"IsRequired\", \"RequiredMessage\", \"DisplayDuringRegistration\", \"DisplayOnCustomerInfoPage\", \"DisplayOrder\") values('test value', false,'',false,false,1)");
				logger.LogInformation("CanExecuteNonQuery(type)={Type}: save query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
				logger.LogInformation("CanExecuteNonQuery(type)={Type}: delete query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
            rez.Should().Be(1);

            rez = await dataProvider.ExecuteNonQueryAsync("delete from \"GdprConsent\" where \"Id\"=0");
            rez.Should().Be(0);
        }

        await dataProvider.TruncateAsync<GdprConsent>();
		logger.LogDebug("<<< Exiting CanExecuteNonQuery(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanQuery(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanQuery(type={Type})", type);
        if (!SetDataProviderType(type))
            return;
			var __logStart = System.Diagnostics.Stopwatch.StartNew();

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanQuery(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        if (type != DataProviderType.PostgreSQL)
        {
            var rez = await dataProvider.QueryAsync<Product>("select * from Product");
            rez.Count.Should().BeGreaterThan(40);
			logger.LogDebug("CanQuery(type={Type}): rez → {Rez}", type, rez);
			logger.LogInformation("CanQuery(type)={Type}: find query executed in {Arg1} ms", type, __logStart.ElapsedMilliseconds);
        }
        else
        {
            var rez = await dataProvider.QueryAsync<Product>("select * from \"Product\"");
            rez.Count.Should().BeGreaterThan(40);
        }
		logger.LogDebug("<<< Exiting CanQuery(type={Type})", type);
    }

    [Test]
    [TestCase(DataProviderType.Unknown)]
    [TestCase(DataProviderType.SqlServer)]
    [TestCase(DataProviderType.MySql)]
    [TestCase(DataProviderType.PostgreSQL)]
    public async Task CanTruncate(DataProviderType type)
    {
		logger.LogDebug(">>> Entering CanTruncate(type={Type})", type);
        if (!SetDataProviderType(type))
            return;

        var dataProvider = GetService<INopDataProvider>();

		logger.LogDebug("CanTruncate(type={Type}): dataProvider → {DataProvider}", type, dataProvider);
        await dataProvider.TruncateAsync<GdprConsent>();
        dataProvider.InsertEntity(new GdprConsent { DisplayOrder = 10, Message = "Test message 1" });
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(1);
        await dataProvider.TruncateAsync<GdprConsent>();
        dataProvider.GetTable<GdprConsent>().Count().Should().Be(0);
		logger.LogDebug("<<< Exiting CanTruncate(type={Type})", type);
    }
}
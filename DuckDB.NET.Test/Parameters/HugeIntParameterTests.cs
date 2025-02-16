using System.Numerics;
using DuckDB.NET.Data;
using FluentAssertions;
using Xunit;

namespace DuckDB.NET.Test.Parameters;

public class HugeIntParameterTests : DuckDBTestBase
{
    public HugeIntParameterTests(DuckDBDatabaseFixture db) : base(db)
    {
    }

    [Fact]
    public void SimpleTest()
    {
        Command.CommandText = "SELECT 125::HUGEINT;";
        Command.ExecuteNonQuery();

        var scalar = Command.ExecuteScalar();
        scalar.Should().Be(new BigInteger(125));

        var reader = Command.ExecuteReader();
        reader.Read();
        var receivedValue = reader.GetFieldValue<BigInteger>(0);
        receivedValue.Should().Be(125);

        reader.GetFieldValue<sbyte>(0).Should().Be(125);
        reader.GetFieldValue<short>(0).Should().Be(125);
        reader.GetFieldValue<int>(0).Should().Be(125);
        reader.GetFieldValue<long>(0).Should().Be(125);

        reader.GetFieldType(0).Should().Be(typeof(BigInteger));
    }

    [Fact]
    public void BindValueTest()
    {
        Command.CommandText = "CREATE TABLE HugeIntTests (key INTEGER, value HugeInt)";
        Command.ExecuteNonQuery();

        Command.CommandText = "INSERT INTO HugeIntTests VALUES (9, ?);";
        
        var value = BigInteger.Add(ulong.MaxValue, 125);
        Command.Parameters.Add(new DuckDBParameter(value));
        Command.ExecuteNonQuery();

        Command.CommandText = "SELECT * from HugeIntTests;";
        
        var reader = Command.ExecuteReader();
        reader.Read();
        
        var receivedValue = reader.GetFieldValue<BigInteger>(1);
        receivedValue.Should().Be(value);
    }
}
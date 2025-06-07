using NFluent;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using TermUI.Core;

namespace TermUiTests.Core;

[TestFixture]
[SingleThreaded] 
public class LogUtilsTests
{
    public (MemoryTarget logTarget, Logger logger) SetUp()
    {
        var logConfig = new LoggingConfiguration();
        var logTarget = new MemoryTarget
        {
            Layout = Layout.FromString("${level} - ${message}")
        };
        var logRule = new LoggingRule("*", LogLevel.Debug, logTarget);
        logConfig.LoggingRules.Add(logRule);
        LogManager.Configuration = logConfig;
        var logger = LogManager.GetCurrentClassLogger();
        return (logTarget, logger);
    }
    
    [Test]
    public void ExtInfo_ClassTest()
    {
        var (logTarget, logger) = SetUp();
        logger.ExtInfo("Starting...", new{Name="Toto", Age=32, Birth=new DateOnly(1999, 12, 31)});
        var logs = logTarget.Logs.ToArray();
        Check.That(logs).ContainsExactly("Info - ExtInfo_ClassTest: Starting... Name[Toto] Age[32] Birth[1999-12-31]");
    }
    
    [Test]
    public void ExtInfo_StringTest()
    {
        var (logTarget, logger) = SetUp();
        logger.ExtInfo("Starting...", "Go !");
        var logs = logTarget.Logs.ToArray();
        Check.That(logs).ContainsExactly("Info - ExtInfo_StringTest: Starting...Go !");
    }    
    
    [Test]
    public void ExtInfo_PrimitiveTest()
    {
        var (logTarget, logger) = SetUp();
        logger.ExtInfo(1);
        logger.ExtInfo(true);
        logger.ExtInfo(3.14);
        logger.ExtInfo('∞');
        var logs = logTarget.Logs.ToArray();
        Check.That(logs).ContainsExactly("Info - ExtInfo_PrimitiveTest: 1","Info - ExtInfo_PrimitiveTest: True","Info - ExtInfo_PrimitiveTest: 3.14","Info - ExtInfo_PrimitiveTest: ∞");
    }    
    
    [Test]
    public void ExtInfo_DateTimeTest()
    {
        var (logTarget, logger) = SetUp();
        logger.ExtInfo(new DateTime(2020, 07, 05, 12, 34, 56, DateTimeKind.Utc));;
        logger.ExtInfo(new DateOnly(2025, 06, 07));
        logger.ExtInfo(new TimeOnly(12, 34, 56));;
        logger.ExtInfo(new TimeSpan(1,2,3,4));
        var logs = logTarget.Logs.ToArray();
        Check.That(logs).ContainsExactly(
            "Info - ExtInfo_DateTimeTest: 2020-07-05 12:34:56",
            "Info - ExtInfo_DateTimeTest: 2025-06-07",
            "Info - ExtInfo_DateTimeTest: 12:34:56",
            "Info - ExtInfo_DateTimeTest: 1.02:03:04"
            );
    }    
    
    [Test]
    public void ExtInfo_ArrayTest()
    {
        var (logTarget, logger) = SetUp();
        logger.ExtInfo(new [] {1, 2, 3, 4});
        var logs = logTarget.Logs.ToArray();
        Check.That(logs).ContainsExactly("Info - ExtInfo_ArrayTest: 1,2,3,4");
    }    
}
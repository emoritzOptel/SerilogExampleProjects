using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Sinks.TestCorrelator;

namespace UnitTestProject
{
    [TestClass]
    public class SerilogErrorHandlingTests
    {
        [TestMethod]
        public void NullConfiguration_ShouldThrow_ArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                LogOneMessage(null);
            });
        }

        [TestMethod]
        public void EmptyConfiguration_ShouldNotThrow_ShouldNotLog()
        {
            using (TestCorrelator.CreateContext())
            {
                LogOneMessageUsingConfigFile("empty.appsettings.json");

                var events = TestCorrelator.GetLogEventsFromCurrentContext();

                Assert.AreEqual(0, events.Count());
            }
        }

        [TestMethod]
        public void EmptySerilogConfiguration_ShouldNotThrow_ShouldNotLog()
        {
            using (TestCorrelator.CreateContext())
            {
                LogOneMessageUsingConfigFile("emptySerilog.appsettings.json");

                var events = TestCorrelator.GetLogEventsFromCurrentContext();

                Assert.AreEqual(0, events.Count());
            }
        }

        [TestMethod]
        public void ValidConfiguration_ShouldNotThrow_ShouldLog()
        {
            using (TestCorrelator.CreateContext())
            {
                var expected = LogOneMessageUsingConfigFile("valid.appsettings.json");

                var events = TestCorrelator.GetLogEventsFromCurrentContext();

                Assert.AreEqual(1, events.Count());
                Assert.AreEqual(expected, events.First().MessageTemplate.Text);
            }
        }

        [TestMethod]
        public void BadUsing_Configuration_ShouldThrow()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                LogOneMessageUsingConfigFile("badUsing.appsettings.json");
            });
        }

        [TestMethod]
        public void BadWriteTo_Configuration_ShouldNotThrow_ShouldNotLog()
        {
            using (TestCorrelator.CreateContext())
            {
                LogOneMessageUsingConfigFile("badWriteTo.appsettings.json");

                var events = TestCorrelator.GetLogEventsFromCurrentContext();

                Assert.AreEqual(0, events.Count());
            }
        }

        [TestMethod]
        public void BadWriteToAndGoodWriteTo_Configuration_ShouldNotThrow_ShouldLog()
        {
            using (TestCorrelator.CreateContext())
            {
                var expected = LogOneMessageUsingConfigFile("badWriteToAndGoodWriteTo.appsettings.json");

                var events = TestCorrelator.GetLogEventsFromCurrentContext();

                Assert.AreEqual(1, events.Count());
                Assert.AreEqual(expected, events.First().MessageTemplate.Text);
            }
        }

        [TestMethod]
        public void BadElasticSearchURL_Configuration_ShouldNotThrow()
        {
            LogOneMessageUsingConfigFile("badElasticSearchURL.appsettings.json");
        }

        [TestMethod]
        public void GoodElasticSearchURL_Configuration_ShouldLog()
        {
            var expected = LogOneMessageUsingConfigFile("goodElasticSearchURL.appsettings.json");
        }

        private static string LogOneMessageUsingConfigFile(string fileName)
        {
            var configRoot = GetConfigRoot(fileName);

            return LogOneMessage(configRoot);
        }

        private static IConfigurationRoot GetConfigRoot(string fileName)
        {
            return new ConfigurationBuilder()
                   .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "appsettings"))
                   .AddJsonFile(fileName)
                   .Build();
        }

        private static string LogOneMessage(IConfigurationRoot configRoot)
        {
            var configuration =
                new LoggerConfiguration()
                    .ReadFrom.Configuration(configRoot);

            var logger = configuration.CreateLogger();

            var message = Guid.NewGuid().ToString();

            logger.Information(message);

            return message;
        }
    }
}
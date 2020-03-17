// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace BingCommerceConnectorCore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bing.Commerce.Connectors.Core;
    using Microsoft.Bing.Commerce.Connectors.Core.Config;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;
    using Microsoft.Bing.Commerce.Connectors.Core.Utilities;
    using Microsoft.Bing.Commerce.Ingestion;
    using Microsoft.Bing.Commerce.Ingestion.Models;
    using Microsoft.Rest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public async Task TestBackgroundTaskScheduler()
        {
            var scheduler = new BackgroundTaskScheduler(TimeSpan.FromSeconds(1));
            var canceller = new CancellationTokenSource();

            int occurences = 0;
            Task run = scheduler.StartAsync(() => occurences++, canceller.Token);

            await Task.Delay(TimeSpan.FromSeconds(0.5));
            Assert.AreEqual(0, occurences);

            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.AreEqual(5, occurences);

            canceller.Cancel();
            await Task.Delay(TimeSpan.FromSeconds(2));

            Assert.AreEqual(5, occurences);
        }

        [TestMethod]
        public async Task TestBufferedSender()
        {
            List<int> addedMessages = new List<int>();
            BufferedSender<int> sender = new BufferedSender<int>(
                800,
                new TaskManager(1),
                new RequestList<int>(5, 500, new TestSerializer()),
                (updates) =>
                {
                    addedMessages.AddRange(updates);
                    return Task.CompletedTask;
                });

            sender.AddRange(new List<int>() { 1, 2 });
            Assert.AreEqual(0, addedMessages.Count);

            await Task.Delay(TimeSpan.FromSeconds(1));

            Assert.AreEqual(2, addedMessages.Count);

            sender.AddRange(new List<int>() { 3, 4, 5, 6, 7, 8 });
            Assert.AreEqual(7, addedMessages.Count);

            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.AreEqual(8, addedMessages.Count);
        }

        [TestMethod]
        public void TestCheckpointAcceptor()
        {
            var testCheckpoint = new TestCheckpoint();
            var acceptor = new CheckpointAcceptor(testCheckpoint);

            acceptor.Pending("1");
            acceptor.Pending("2");

            acceptor.Accept("2");

            Assert.IsFalse(testCheckpoint.IsValid());

            acceptor.Accept("1");

            Assert.IsTrue(testCheckpoint.IsValid());
            Assert.AreEqual("2", testCheckpoint.GetValue());
        }

        [TestMethod]
        public async Task TestIngestionClient()
        {
            var attemptCount = 0;
            var failFor = 6;
            var interceptor = new TestDelegatingHandler((request) =>
           {
                // Artificial response for the PushDataUpdate API.
                attemptCount++;
               if (failFor > 0)
               {
                   failFor--;
                   return new HttpResponseMessage(System.Net.HttpStatusCode.GatewayTimeout);
               }
               var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
               response.Content = new StringContent("{}");
               return response;
           });

            var ingestion = new BingCommerceIngestion(new TokenCredentials("test"), interceptor);

            var client = new IngestionClient(ingestion, "tenant", "index", 5, new RequestLogger(null, RequestLogLevel.None));

            var result = await client.PushDataUpdateAsync("body");
            Assert.IsNull(result);

            Assert.AreEqual(5, attemptCount);
            Assert.AreEqual(1, failFor);

            result = await client.PushDataUpdateAsync("body");
            Assert.IsNotNull(result);

            Assert.AreEqual(7, attemptCount);
            Assert.AreEqual(0, failFor);
        }

        [TestMethod]
        public void TestRequestList()
        {
            var list = new RequestList<int>(5, 25, new TestSerializer());

            Assert.IsNull(list.Add(1000000000));
            Assert.IsNull(list.Add(1000000001));

            Assert.IsNotNull(list.Add(1));
            Assert.IsNull(list.Add(2));
            Assert.IsNull(list.Add(3));
            Assert.IsNull(list.Add(4));
            Assert.IsNotNull(list.Add(5));
        }

        [TestMethod]
        public void TestRequestLogger_None()
        {
            RequestLogger logger = new RequestLogger(".\\log", RequestLogLevel.None);

            Assert.IsTrue(!Directory.Exists(".\\log\\deadletter"), "deadletter directory is created while log level is set to none");
            Assert.IsTrue(!Directory.Exists(".\\log\\successful"), "successful directory is created while log level is set to none");

            // These calls should do nothing, shouldn't fail either.
            logger.LogSuccess("body");
            logger.LogFailure("body");
        }

        [TestMethod]
        public void TestRequestLogger_deadletter()
        {
            try
            {
                RequestLogger logger = new RequestLogger(".\\log", RequestLogLevel.DeadletterOnly);

                Assert.IsTrue(Directory.Exists(".\\log\\deadletter"), "deadletter directory is not created while log level is set to deadletter only");
                Assert.IsTrue(!Directory.Exists(".\\log\\successful"), "successful directory is created while log level is set to deadletter only");

                logger.LogSuccess("body");
                logger.LogFailure("body");

                Thread.Sleep(10);
                Assert.IsTrue(Directory.GetFiles(".\\log\\deadletter").Length == 1, "expecting one file to be in the deadletter directory");
            }
            finally
            {
                Thread.Sleep(10);
                Directory.Delete(".\\log", true);
            }
        }

        [TestMethod]
        public void TestRequestLogger_all()
        {
            try
            {
                RequestLogger logger = new RequestLogger(".\\log", RequestLogLevel.All);

                Assert.IsTrue(Directory.Exists(".\\log\\deadletter"), "deadletter directory is not created while log level is set to all");
                Assert.IsTrue(Directory.Exists(".\\log\\successful"), "successful directory is not created while log level is set to all");

                logger.LogSuccess("body");
                logger.LogFailure("body");

                Thread.Sleep(10);
                Assert.IsTrue(Directory.GetFiles(".\\log\\deadletter").Length == 1, "expecting one file to be in the deadletter directory");
                Assert.IsTrue(Directory.GetFiles(".\\log\\successful").Length == 1, "expecting one file to be in the successful directory");
            }
            finally
            {
                Thread.Sleep(10);
                Directory.Delete(".\\log", true);
            }
        }

        [TestMethod]
        public async Task TestRetryStrategy()
        {
            int nCalls = 0;
            int succeedAfter = 3;
            Func<Task<int>> toRetry = async () =>
            {
                nCalls++;
                if (succeedAfter-- > 0)
                {
                    throw new OutOfMemoryException();
                }
                return await Task.Run(() => nCalls);
            };

            var result = await RetryStrategy.RetryAsync(toRetry, 5, 500);

            Assert.AreEqual(4, result);

            succeedAfter = 1;

            await Assert.ThrowsExceptionAsync<OutOfMemoryException>(() => RetryStrategy.RetryAsync(toRetry, 5, 500, (e) =>
            {
                if (e is OutOfMemoryException)
                {
                    throw e;
                }
            }));
        }

        [TestMethod]
        public async Task TestStatusTracker()
        {
            var updateId = "123";

            var failAfter = 2;
            AutoResetEvent signal = new AutoResetEvent(false);
            var interceptor = new TestDelegatingHandler((request) =>
            {
                if (request.RequestUri.ToString().Contains("/status/"))
                {
                    // Artificial response for the PushUpdateStatus request.
                    PushUpdateStatusResponse status = new PushUpdateStatusResponse()
                    {
                        UpdateId = updateId,
                        Status = (failAfter-- > 0) ? "InProgress" : "PartiallySucceeded",
                        Records = new List<ResponseRecordStatus>()
                        {
                            new ResponseRecordStatus() { RecordId = "1", Status = "Succeeded" },
                            new ResponseRecordStatus() { RecordId = "2", Status = "Failed", ErrorMessage = "error message" }
                        }
                    };

                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(status));
                    if (status.Status != "InProgress") { signal.Set(); }

                    return response;
                }
                else
                {
                    // Artificial response for the GetIndex request.
                    var index = new IndexResponse()
                    {
                        Indexes = new List<ResponseIndex>()
                        {
                            new ResponseIndex
                            {
                                Id = "index",
                                Fields = new List<IndexField>()
                                {
                                    new IndexField() { Name = "notid", Type = IndexFieldType.Title },
                                    new IndexField() { Name = "myid" , Type = IndexFieldType.ProductId },
                                    new IndexField() { Name = "alsonotid", Type = IndexFieldType.Description }
                                }
                            }
                        }
                    };

                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(index));
                    return response;
                }
            });

            try
            {
                var ingestion = new BingCommerceIngestion(new TokenCredentials("test"), interceptor);
                var logger = new RequestLogger(".\\log", RequestLogLevel.DeadletterOnly);
                var client = new IngestionClient(ingestion, "tenant", "index", 1, logger);
                var tracker = new StatusTracker(client, TimeSpan.FromSeconds(1), logger);

                tracker.Start();
                tracker.Add(updateId, new List<Dictionary<string, object>>()
                {
                    new Dictionary<string, object>() { { "myid", "1" }, { "notid", "something" }, { "stillnotid", "still something"} },
                    new Dictionary<string, object>() { { "myid", "2" }, { "notid", "something else" }, { "stillnotid", "still something else"} },
                });

                Assert.AreEqual(0, Directory.GetFiles(".\\log\\deadletter").Length, "Expecting no files to be in the deadletter directory");

                signal.WaitOne(TimeSpan.FromSeconds(10));

                // Adding an extra 2 seconds delay for two reasons:
                //  1. Make sure to give the tracker the chance to process the api response and logging the failure.
                //  2. Make sure that the failure would be processed only once (the next event would skip it).
                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.AreEqual(1, Directory.GetFiles(".\\log\\deadletter").Length, "Expecting one file to be in the deadletter directory");

                Assert.AreEqual(1, JArray.Parse(File.ReadAllText(Directory.EnumerateFiles(".\\log\\deadletter").First())).Count);
            }
            finally
            {
                Directory.Delete(".\\log", true);
            }
        }

        [TestMethod]
        public void TestTaskManager()
        {
            var taskMan = new TaskManager(2);
            Stopwatch w = new Stopwatch();

            w.Start();
            taskMan.Add(() => Task.Delay(TimeSpan.FromSeconds(6)));
            w.Stop();
            Assert.IsTrue(w.Elapsed.TotalSeconds < 1);

            w.Restart();
            taskMan.Add(() => Task.Delay(TimeSpan.FromSeconds(3)));
            w.Stop();
            Assert.IsTrue(w.Elapsed.TotalSeconds < 1);

            w.Restart();
            taskMan.Add(() => Task.Delay(TimeSpan.FromSeconds(1)));
            w.Stop();
            Assert.IsTrue(w.Elapsed.TotalSeconds > 2);

            w.Restart();
            taskMan.WaitAll();
            w.Stop();
            Assert.IsTrue(w.Elapsed.TotalSeconds > 2);
        }

        class TestDelegatingHandler : DelegatingHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> handler;

            public TestDelegatingHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            {
                this.handler = handler;
            }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await Task.Run(() => handler(request));
                return response;
            }
        }

        class TestCheckpoint : IDataCheckpoint
        {
            public List<string> Accepted { get; set; }

            public TestCheckpoint()
            {
                this.Accepted = new List<string>();
            }

            public void Accept(string newCheckpoint)
            {
                Accepted.Add(newCheckpoint);
            }

            public string GetValue()
            {
                return Accepted.Last();
            }

            public bool IsValid()
            {
                return Accepted.Count > 0;
            }
        }

        class TestSerializer : IPushSerializer<int>
        {
            public uint OverheadSize => 2;

            public uint RecordOverheadSize => 1;

            public string Serialize(IEnumerable<int> records)
            {
                return JsonConvert.SerializeObject(records);
            }

            public string Serialize(int record)
            {
                return JsonConvert.SerializeObject(record);
            }
        }
    }
}

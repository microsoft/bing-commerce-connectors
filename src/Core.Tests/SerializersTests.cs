// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace BingCommerceConnectorCore.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bing.Commerce.Connectors.Core.Serializers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializersTests
    {
        private static readonly List<IDictionary<string, object>> data = new List<IDictionary<string, object>>()
        {
            new Dictionary<string, object>()
            {
                { "id" , 1 },
                { "name", "Doe,\tJohn" }
            },
            new Dictionary<string, object>()
            {
                { "id" , 2 },
                { "name", "Doe,\tJane" }
            }
        };

        [TestMethod]
        public void TestNDJsonSerializer()
        {
            var serializer = new NDJsonSerializer();

            var expected = $"{{\"id\":1,\"name\":\"Doe,\\tJohn\"}}{Environment.NewLine}{{\"id\":2,\"name\":\"Doe,\\tJane\"}}";

            var output = serializer.Serialize(data);

            Assert.IsNotNull(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TestJsonArraySerializer()
        {
            var serializer = new JsonArraySerializer();

            var expected = $"[{{\"id\":1,\"name\":\"Doe,\\tJohn\"}},{{\"id\":2,\"name\":\"Doe,\\tJane\"}}]";

            var output = serializer.Serialize(data);

            Assert.IsNotNull(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TestCSVSerializer()
        {
            var serializer = new CSVSerializer();

            var expected = $"1,\"Doe,\tJohn\"{Environment.NewLine}2,\"Doe,\tJane\"";

            var output = serializer.Serialize(data);

            Assert.IsNotNull(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void TestTSVSerializer()
        {
            var serializer = new TSVSerializer();

            var expected = $"1\tDoe,\\tJohn{Environment.NewLine}2\tDoe,\\tJane";

            var output = serializer.Serialize(data);

            Assert.IsNotNull(output);
            Assert.AreEqual(expected, output);
        }
    }
}

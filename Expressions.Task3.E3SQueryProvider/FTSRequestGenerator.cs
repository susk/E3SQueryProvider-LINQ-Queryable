﻿// using Microsoft.AspNetCore.WebUtilities;
// using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Expressions.Task3.E3SQueryProvider.Attributes;
using Expressions.Task3.E3SQueryProvider.Models.Request;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace Expressions.Task3.E3SQueryProvider
{
    public class FtsRequestGenerator
    {
        private readonly string fTSSearchTemplate = @"/searchFts";
        private readonly string baseAddress;

        public FtsRequestGenerator(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public Uri GenerateRequestUrl<T>(string query = "*", int start = 0, int limit = 10)
        {
            return this.GenerateRequestUrl(typeof(T), query, start, limit);
        }

        public Uri GenerateRequestUrl(Type type, string query = "*", int start = 0, int limit = 10)
        {
            string metaTypeName = GetMetaTypeName(type);

            var ftsQueryRequest = new FtsQueryRequest
            {
                Statements = new List<Statement>
                {
                    new Statement
                    {
                        Query = query,
                    },
                },
                Start = start,
                Limit = limit,
            };

            var ftsQueryRequestString = JsonConvert.SerializeObject(ftsQueryRequest);

            var uri = BindByName(
                $"{this.baseAddress}{this.fTSSearchTemplate}",
                new Dictionary<string, string>()
                {
                    { "metaType", metaTypeName },
                    { "query", ftsQueryRequestString },
                });

            return uri;
        }

        private static Uri BindByName(string baseAddress, Dictionary<string, string> queryParams)
            => new Uri(QueryHelpers.AddQueryString(baseAddress, queryParams));

        private static string GetMetaTypeName(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(E3SMetaTypeAttribute), false);

            if (attributes.Length == 0)
            {
                throw new Exception($"Entity {type.FullName} do not have attribute E3SMetaType");
            }

            return ((E3SMetaTypeAttribute)attributes[0]).Name;
        }
    }
}

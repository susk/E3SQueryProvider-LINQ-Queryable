﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Expressions.Task3.E3SQueryProvider.Models.Entities;
using Expressions.Task3.E3SQueryProvider.Models.Response;
using Newtonsoft.Json;

namespace Expressions.Task3.E3SQueryProvider.Services
{
    public class E3SSearchService : IE3SSearchService
    {
        private readonly string baseAddress;
        private readonly HttpClient httpClient;

        public E3SSearchService(HttpClient httpClient, string baseAddress)
        {
            this.baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public IEnumerable<T> SearchFts<T>(string query, int start = 0, int limit = 0) where T : BaseE3SEntity
        {
            var requestGenerator = new FtsRequestGenerator(baseAddress);

            Uri request = requestGenerator.GenerateRequestUrl<T>(query, start, limit);

            string resultString = httpClient.GetStringAsync(request).Result;

            return JsonConvert.DeserializeObject<FtsResponse<T>>(resultString).Items.Select(t => t.Data);
        }

        public IEnumerable SearchFts(Type type, string query, int start = 0, int limit = 0)
        {
            Type finalType = typeof(FtsResponse<>).MakeGenericType(type);
            if (finalType == null)
            {
                throw new ArgumentNullException(nameof(finalType));
            }

            var items = finalType.GetProperty("items");
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var requestGenerator = new FtsRequestGenerator(baseAddress);
            Uri request = requestGenerator.GenerateRequestUrl(type, query, start, limit);

            string resultString = httpClient.GetStringAsync(request).Result;

            object result = JsonConvert.DeserializeObject(resultString, finalType);

            var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;

            foreach (object item in (IEnumerable)items.GetValue(result))
            {
                var data = item.GetType().GetProperty("data");
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                list.Add(data.GetValue(item));
            }

            return list;
        }
    }
}

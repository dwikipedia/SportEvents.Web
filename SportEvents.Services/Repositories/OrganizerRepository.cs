using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportEvents.Core.Models;
using SportEvents.Domain.Models.Organizer;
using SportEvents.Domain.Repositories;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static SportEvents.Infrastructure.Constants;

namespace SportEvents.Infrastructure.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly ApiRequest _apiRequest;

        public OrganizerRepository(
            ApiRequest apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public int CountOrganizer(IEnumerable<OrganizerResponse> organizers)
        {
            return organizers.Count();
        }

        public Task CreateOrganizer(CreateOrganizer organizer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOrganizer(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponse<OrganizerResponse>> GetAllOrganizers(OrganizersRequest request)
        {
            // 1. Fetch the FIRST PAGE to get total record count
            var initialQuery = new Dictionary<string, string?>()
            {
                ["page"] = "1",
                ["perPage"] = "1" // Minimal data to get pagination metadata
            };

            string initialUri = QueryHelpers.AddQueryString(ApiUrl.Organizers, initialQuery);
            var initialResponse = await _apiRequest.GetHttpRequestMessage(HttpMethod.Get, initialUri);
            var initialPaged = await initialResponse.Content.ReadFromJsonAsync<PagedResponse<OrganizerResponse>>();

            int totalRecords = initialPaged.Meta.Pagination.Total;
            int maxPerPage = 1000; // Use the maximum allowed by the external API

            // 2. Calculate how many pages we need to fetch
            int totalPages = (int)Math.Ceiling((double)totalRecords / maxPerPage);

            // 3. Fetch ALL pages from the external API
            var allData = new List<OrganizerResponse>();
            for (int page = 1; page <= totalPages; page++)
            {
                var query = new Dictionary<string, string?>()
                {
                    ["page"] = page.ToString(),
                    ["perPage"] = maxPerPage.ToString()
                };

                string uri = QueryHelpers.AddQueryString(ApiUrl.Organizers, query);
                var response = await _apiRequest.GetHttpRequestMessage(HttpMethod.Get, uri);
                var paged = await response.Content.ReadFromJsonAsync<PagedResponse<OrganizerResponse>>();
                allData.AddRange(paged.Data);
            }

            // 4. Now work with the complete dataset
            var queryableData = allData.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                queryableData = queryableData.Where(x =>
                    x.OrganizerName.Contains(request.SearchValue, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(request.SortColumn))
            {
                queryableData = request.SortDirection == "desc"
                    ? queryableData.OrderByDescending(x => x.Id)
                    : queryableData.OrderBy(x => x.Id);
            }

            // Paginate the filtered/sorted data
            var pagedData = queryableData
                .Skip((request.Page - 1) * request.PerPage)
                .Take(request.PerPage)
                .ToList();

            return new PagedResponse<OrganizerResponse>
            {
                Data = pagedData,
                Meta = initialPaged.Meta,
                RecordsTotal = totalRecords,
                RecordsFiltered = queryableData.Count() // Total after filtering
            };
        }

        public Task<OrganizerResponse> GetOrganizer()
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrganizer(OrganizerResponse request)
        {
            throw new NotImplementedException();
        }
    }
}

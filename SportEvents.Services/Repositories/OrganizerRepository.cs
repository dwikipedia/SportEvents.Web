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
            var query = new Dictionary<string, string?>()
            {
                ["page"] = request.Page.ToString(),
                ["perPage"] = request.PerPage.ToString()
            };

            string uriWithQs = QueryHelpers.AddQueryString(ApiUrl.Organizers, query);

            var response = await _apiRequest
                .GetHttpRequestMessage(HttpMethod.Get, uriWithQs);

            var paged = await response.Content
                .ReadFromJsonAsync<PagedResponse<OrganizerResponse>>()
                ?? new PagedResponse<OrganizerResponse>();
            
            var items = paged.Data.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                // Case‐insensitive substring match
                string term = request.SearchValue.Trim();
                items = items.Where(x =>
                    x.OrganizerName?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                );
            }

            if (!string.IsNullOrWhiteSpace(request.SortDirection)
                && !string.IsNullOrWhiteSpace(request.SortColumn))
            {
                bool desc = request.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
                items = desc
                    ? items.OrderByDescending(x => x.Id)
                    : items.OrderBy(x => x.Id);
            }

            var resultList = items.ToList();
            paged.RecordsTotal = paged.Meta.Pagination.Total;
            paged.RecordsFiltered = resultList.Count;

            paged.Data = resultList;

            return paged;
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

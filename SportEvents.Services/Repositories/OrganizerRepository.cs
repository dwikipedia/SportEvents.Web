using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SportEvents.Domain.Models.Organizer;
using SportEvents.Domain.Repositories;
using System;
using System.Net;
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

        public Task CreateOrganizer(CreateOrganizer organizer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOrganizer(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrganizerResponse>> GetAllOrganizers(OrganizersRequest request)
        {
            var response = await _apiRequest.GetHttpRequestMessage(HttpMethod.Get, ApiUrl.Organizers);
            return await response.Content.ReadFromJsonAsync<List<OrganizerResponse>>();
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

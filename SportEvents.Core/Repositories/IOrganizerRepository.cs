using SportEvents.Core.Models;
using SportEvents.Domain.Models.Organizer;

namespace SportEvents.Domain.Repositories
{
    public interface IOrganizerRepository
    {
        Task<PagedResponse<OrganizerResponse>> GetAllOrganizers(OrganizersRequest request);
        int CountOrganizer(IEnumerable<OrganizerResponse> organizers);
        Task<OrganizerResponse> GetOrganizer();
        Task CreateOrganizer(CreateOrganizer organizer);
        Task DeleteOrganizer(int id);
        Task UpdateOrganizer(OrganizerResponse request);
    }
}

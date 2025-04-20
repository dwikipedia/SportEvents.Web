using SportEvents.Domain.Models.Organizer;

namespace SportEvents.Domain.Repositories
{
    public interface IOrganizerRepository
    {
        Task<IEnumerable<OrganizerResponse>> GetAllOrganizers(OrganizersRequest request);
        Task<OrganizerResponse> GetOrganizer();
        Task CreateOrganizer(CreateOrganizer organizer);
        Task DeleteOrganizer(int id);
        Task UpdateOrganizer(OrganizerResponse request);
    }
}

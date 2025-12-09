using DataModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<MeetingViewModel> RecentMeetings { get; set; } = new List<MeetingViewModel>();
        public async Task OnGetAsync()
        {
            try
            {
                // Получаем все мероприятия и берем 3 последних
                var allMeetings = await APIClient.GetAsync<List<MeetingViewModel>>("api/meeting/getlist");

                if (allMeetings != null && allMeetings.Any())
                {
                    RecentMeetings = allMeetings
                        .Where(m => m.Date >= DateTime.Now) // Только будущие
                        .OrderBy(m => m.Date)
                        .Take(3)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recent meetings: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                RecentMeetings = new List<MeetingViewModel>();
            }
        }
    }
}

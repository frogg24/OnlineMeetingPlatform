using DataModels.Models;
using DataModels.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class MeetingsModel : PageModel
    {
        public List<MeetingViewModel> Meetings { get; set; } = new List<MeetingViewModel>();
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = "";
        public async Task OnGet()
        {
            try
            {
                Meetings = await APIClient.GetAsync<List<MeetingViewModel>>("api/meeting/getlist");
                if (SearchString != null && !string.IsNullOrEmpty(SearchString))
                {
                    Meetings = Meetings.Where(m => m.Title.Contains(SearchString, StringComparison.OrdinalIgnoreCase) 
                    || m.Description.Contains(SearchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            catch
            {
                Meetings = new List<MeetingViewModel>();
            }
        }
    }
}

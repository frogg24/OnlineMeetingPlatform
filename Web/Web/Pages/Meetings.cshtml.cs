using DataModels.Models;
using DataModels.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class MeetingsModel : PageModel
    {
        public List<MeetingViewModel> Meetings { get; set; } = new List<MeetingViewModel>();
        public async Task OnGet()
        {
            try
            {
                Meetings = await APIClient.GetAsync<List<MeetingViewModel>>("api/meeting/getlist");
            }
            catch
            {
                Meetings = new List<MeetingViewModel>();
            }
        }
    }
}

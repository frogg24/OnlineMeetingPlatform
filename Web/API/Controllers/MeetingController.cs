using BusinessLogic;
using DataModels.Models;
using DataModels.SearchModels;
using DataModels.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        private readonly IMeetingUserService _meetingUserService;

        public MeetingController(IMeetingService meetingService, IMeetingUserService meetingUserService)
        {
            _meetingService = meetingService;
            _meetingUserService = meetingUserService;
        }

        // === Meeting endpoints ===

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] MeetingSearchModel? model)
        {
            try
            {
                var result = await _meetingService.ReadList(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MeetingViewModel model)
        {
            try
            {
                var result = await _meetingService.Create(model);
                if (result)
                {
                    return Ok(new { message = "Мероприятие создано" });
                }
                return BadRequest(new { message = "Ошибка при создании мероприятия" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MeetingViewModel model)
        {
            try
            {
                var result = await _meetingService.Update(model);
                if (result)
                {
                    return Ok(new { message = "Мероприятие обновлено" });
                }
                return BadRequest(new { message = "Ошибка при обновлении мероприятия" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var model = new MeetingViewModel { Id = id };
                var result = await _meetingService.Delete(model);
                if (result)
                {
                    return Ok(new { message = "Мероприятие удалено" });
                }
                return BadRequest(new { message = "Ошибка при удалении мероприятия" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // === MeetingUser endpoints ===

        [HttpGet]
        public async Task<IActionResult> GetListUsers([FromQuery] MeetingUserSearchModel? model)
        {
            try
            {
                var result = await _meetingUserService.ReadList(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] MeetingUserViewModel model)
        {
            try
            {
                var result = await _meetingUserService.Create(model);
                if (result)
                {
                    return Ok(new { message = "Пользователь добавлен к мероприятию" });
                }
                return BadRequest(new { message = "Ошибка при добавлении пользователя к мероприятию" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Наверное это даже не нужно, можно удалить
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] MeetingUserViewModel model)
        {
            try
            {
                var result = await _meetingUserService.Update(model);
                if (result)
                {
                    return Ok(new { message = "Запись обновлена" });
                }
                return BadRequest(new { message = "Ошибка при обновлении записи" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            try
            {
                var model = new MeetingUserViewModel { Id = id };
                var result = await _meetingUserService.Delete(model);
                if (result)
                {
                    return Ok(new { message = "Пользователь удален из мероприятия" });
                }
                return BadRequest(new { message = "Ошибка при удалении пользователя из мероприятия" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
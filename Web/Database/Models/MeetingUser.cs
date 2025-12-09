using DataModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Database.Models
{
    public class MeetingUser
    {
        public int Id { get; private set; }

        [Required]
        public int MeetingId { get; private set; }

        [Required]
        public int UserId { get; private set; }
        public virtual Meeting Meeting { get; set; }

        public virtual User User { get; set; }

        public static MeetingUser? Create(Database context, MeetingUserViewModel model)
        {
            if (model == null) return null;

            var meeting = context.Meetings.FirstOrDefault(d => d.Id == model.MeetingId);
            var user = context.Users.FirstOrDefault(d => d.Id == model.UserId);

            return new MeetingUser()
            {
                Id = model.Id,
                MeetingId = model.MeetingId,
                UserId = model.UserId,

                Meeting = meeting,
                User = user,
                
            };
        }

        public void Update(Database context, MeetingUserViewModel model)
        {
            if (model == null) return;

            var meeting = context.Meetings.FirstOrDefault(d => d.Id == model.MeetingId);
            var user = context.Users.FirstOrDefault(d => d.Id == model.UserId);

            MeetingId = model.MeetingId;
            UserId = model.UserId;
            Meeting = meeting;
            User = user;
        }

        public MeetingUserViewModel GetViewModel => new()
        {
            Id = Id,
            MeetingId = MeetingId,
            UserId = UserId,
            Username = User?.Username,
            Email = User?.Email,
            isNotificationOn = User?.isNotificationOn
        };
    }
}

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

            return new MeetingUser()
            {
                Id = model.Id,
                MeetingId = model.MeetingId,
                UserId = model.UserId,

                Meeting = context.Meetings.FirstOrDefault(d => d.Id == model.MeetingId),
                User = context.Users.FirstOrDefault(d => d.Id == model.UserId),
            };
        }

        public void Update(Database context, MeetingUserViewModel model)
        {
            if (model == null) return;

            MeetingId = model.MeetingId;
            UserId = model.UserId;
            Meeting = context.Meetings.FirstOrDefault(d => d.Id == model.MeetingId);
            User = context.Users.FirstOrDefault(d => d.Id == model.UserId);
        }

        public MeetingUserViewModel GetViewModel => new()
        {
            Id = Id,
            MeetingId = MeetingId,
            UserId = UserId
        };
    }
}

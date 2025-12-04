using DataModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class Meeting
    {
        public int Id { get; private set; }
        [Required]
        public string Title { get; private set; }
        [Required]
        public string Description { get; private set; }
        [Required]
        public string Link { get; private set; }
        [Required]
        public DateTime Date { get; private set; }
        [Required]
        public int ManagerId { get; private set; }

        [ForeignKey("MeetingId")]
        public virtual List<MeetingUser> MeetingUser { get; set; } = new();
        public virtual User Manager { get; set; }
        public static Meeting? Create(Database context, MeetingViewModel meeting)
        {
            if (meeting == null)
            {
                return null;
            }
            return new Meeting
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Description = meeting.Description,
                Link = meeting.Link,
                Date = meeting.Date,
                ManagerId = meeting.ManagerId,

                Manager = context.Users.FirstOrDefault(x => x.Id == meeting.ManagerId)
            };
        }

        public void Update(Database context, MeetingViewModel meeting)
        {
            if (meeting == null)
            {
                return;
            }

            Title = meeting.Title;
            Description = meeting.Description;
            Link = meeting.Link;
            Date = meeting.Date;
            ManagerId = meeting.ManagerId;

            Manager = context.Users.FirstOrDefault(x => x.Id == meeting.ManagerId);
        }

        public MeetingViewModel GetViewModel => new()
        {
            Id = Id,
            Title = Title,
            Description = Description,
            Link = Link,
            Date = Date,
            ManagerId = Manager.Id,
        };
    }
}

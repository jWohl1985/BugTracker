﻿using System.ComponentModel;

namespace BugTracker.Models
{
    public class TicketComment
    {
        // Keys
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }

        // Properties
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        public string Comment { get; set; }

        // Navigation properties
        public virtual Ticket Ticket { get; set; }
        public virtual BugTrackerUser User { get; set; }

    }
}

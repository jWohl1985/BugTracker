﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [DisplayName("Ticket Type")]
        public string Name { get; set; }

    }
}

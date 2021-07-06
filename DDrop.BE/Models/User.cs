using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DDrop.BE.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] UserPhoto { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public List<Series> UserSeries { get; set; }

        public ObservableCollection<Plot> Plots { get; set; }
    }
}
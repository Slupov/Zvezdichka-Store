﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Zvezdichka.Data.Models.Checkout
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public ApplicationUser Customer { get; set; }

        public string CustomerId { get; set; }

        public bool IsOnline { get; set; }
    }
}
﻿using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Models {
    public class User {

        public string _id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string nameUser { get; set; }
        public string lastNameUser { get; set; }
        public string statusUser { get; set; }
        public string emailUser { get; set; }

        public User() { }

    }
}

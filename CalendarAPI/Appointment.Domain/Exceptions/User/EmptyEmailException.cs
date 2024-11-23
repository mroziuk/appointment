﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.Exceptions.User
{
    public class EmptyEmailException : DomainException
    {
        public EmptyEmailException() :base("Empty email") { }
    }
}

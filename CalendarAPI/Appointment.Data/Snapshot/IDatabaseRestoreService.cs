using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Snapshot
{
    public interface IDatabaseRestoreService
    {
        bool Restore();
    }
}

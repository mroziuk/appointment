using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Appointment.Tests.Integration
{
    public class ResetDatabase : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;
        private TransactionScope _transactionScope;

        public void AfterTest(ITest test)
        {
            _transactionScope.Dispose();
        }

        public void BeforeTest(ITest test)
        {
            _transactionScope = new TransactionScope();
        }
    }
}

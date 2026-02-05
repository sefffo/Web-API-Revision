using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Domain.Interfaces
{
    public interface IDataInitializer
    {
        public Task Initialize();
    }
}

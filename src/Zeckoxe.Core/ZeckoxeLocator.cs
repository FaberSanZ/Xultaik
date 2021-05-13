using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeckoxe.Core
{
    public class ZeckoxeLocator
    {
#nullable disable

        private static ZeckoxeLocator _ZeckoxeLocator = new();
        public static ZeckoxeLocator Instance => _ZeckoxeLocator;


        private Dictionary<Type, object> Dictionary;
        private ZeckoxeLocator()
        {
           Dictionary = new Dictionary<Type, object>();
        }

        public ZeckoxeLocator Bind<TImpl>(TImpl instance)
        {
            if (Dictionary.ContainsKey(typeof(TImpl)))
            {
                throw new NotImplementedException("The type is already registered.");
            }

            Dictionary.Add(typeof(TImpl), instance);

            return this;
        }

#nullable enable

        public TImpl GetService<TImpl>()
        {
            if (Dictionary.ContainsKey(typeof(TImpl)))
            {
                return (TImpl)Dictionary[typeof(TImpl)];
            }

            throw new NotImplementedException("The type is not registered");
        }
    }
}

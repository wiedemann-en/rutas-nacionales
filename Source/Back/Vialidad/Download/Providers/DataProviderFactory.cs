using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;

namespace Vialidad.Download.Providers
{
    public static class DataProviderFactory
    {
        private static Dictionary<enDataProviderType, Func<DataProviderBase>> _dataSources;

        static DataProviderFactory()
        {
            InitializeDict();
        }

        public static DataProviderBase GetInstance(enDataProviderType dataProviderType)
        {
            DataProviderBase result = null;
            if (_dataSources.TryGetValue(dataProviderType, out var funct))
                result = funct();
            return result;
        }

        private static void InitializeDict()
        {
            _dataSources = new Dictionary<enDataProviderType, Func<DataProviderBase>>();
            _dataSources.Add(enDataProviderType.Vialidad, () => new DataProviderVialidad());
            _dataSources.Add(enDataProviderType.Google, () => new DataProviderGoogle());
        }
    }
}

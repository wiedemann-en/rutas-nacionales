using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Contracts.Models
{
    public class KeyValueDto<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }

    public class KeyValueDto : KeyValueDto<string, string>
    {
        public KeyValueDto()
        {
        }

        public KeyValueDto(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class KeyValueIntDto : KeyValueDto<int, string>
    {
        public KeyValueIntDto()
        {
        }

        public KeyValueIntDto(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class KeyValueLongDto : KeyValueDto<long, string>
    {
        public KeyValueLongDto()
        {
        }

        public KeyValueLongDto(long key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}

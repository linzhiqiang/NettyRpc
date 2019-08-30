using System;
using System.Collections.Generic;
using System.Text;

namespace NettyRpc.Core.Serializes
{
    public class SerializeManage
    {
        private static IDictionary<byte, ISerialize> Dict = new Dictionary<byte, ISerialize>();

        public static SerializeManage Instance = new SerializeManage();

        

        private SerializeManage()
        {
            this.RegisterSerialize(0, new ProtoSerialize());
            this.RegisterSerialize(1, new JsonSerialize());
        }

        public void RegisterSerialize(byte serializeType, ISerialize serializeObj)
        {
            if (!Dict.ContainsKey(serializeType))
            {
                Dict.Add(serializeType, serializeObj);
            }
        }

        public ISerialize GetSerialize(byte serializeType)
        {
            ISerialize result;
            Dict.TryGetValue(serializeType, out result);
            return result;
        }
    }
}

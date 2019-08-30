using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.UserService.DTO
{
    [ProtoContract]
    public class GetUserReq
    {
        [ProtoMember(1)]
        public int UserId { get; set; }
    }

    [ProtoContract]
    public class GetUserRes
    {
        [ProtoMember(1)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        public string  UserName { get; set; }
    }
}

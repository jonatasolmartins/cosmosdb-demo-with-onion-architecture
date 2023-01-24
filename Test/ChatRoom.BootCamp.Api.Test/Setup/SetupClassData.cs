using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoom.BootCamp.Api.Test.Setup
{
    internal class SetupClassData : IEnumerable<object[]>
    {
        public static IEnumerable<object[]> Parameters()
        {
            yield return new object[]
            {
            HttpStatusCode.Created,
            true
            };
            yield return new object[]
            {
            HttpStatusCode.BadRequest,
            false
            };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
           {
            HttpStatusCode.Created,
            true
           };
            yield return new object[]
            {
            HttpStatusCode.BadRequest,
            false
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

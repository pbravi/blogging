using System;
using System.Collections.Generic;
using System.Text;

namespace blogging.model.auth
{
    public class UserToken
    {
        public string Tokent { get; set; }
        public DateTime Expiration { get; set; }
    }
}

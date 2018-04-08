using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get { return Name + "@gmail.com"; } }

    }
}

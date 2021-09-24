using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems
{
    public interface IScheduleable
    {
        /// <summary>
        /// Different from <c>IsEnabled</c>. This enabled or disables the <c>IScheduleable</c> in the schedule. I.e. the schedule will skip over this.
        /// </summary>
        public bool Enabled { get; set; }
        public int Speed { get; set; }
    }
}

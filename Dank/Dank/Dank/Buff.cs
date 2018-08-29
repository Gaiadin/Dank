using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dank
{
    class Buff
    {
        #region FIELDS
        String stat;
        int effect;
        int duration;
        #endregion
        #region PROPERTIES
        public String Stat { get { return stat; } }
        public int Effect { get { return effect; } }
        public int Duration { get { return duration; } set { duration = value; } }
        #endregion

        public Buff(String stat, int effect, int duration)
        {
            this.stat = stat;
            this.effect = effect;
            this.duration = duration;
        }
    }
}

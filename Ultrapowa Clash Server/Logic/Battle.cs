using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS.Logic
{
    internal class Battle
    {
        internal double Last_Tick;

        internal double Preparation_Time = 30;
        internal double Attack_Time = 180;

        /// <summary>
        ///     Gets or sets the battle tick.
        /// </summary>
        /// <value>The battle tick.</value>
        internal double BattleTick
        {
            get
            {
                if (this.Preparation_Time > 0) return this.Preparation_Time;
                return this.Attack_Time;
            }
            set
            {
                if (this.Preparation_Time >= 1)
                {
                    this.Preparation_Time -= (value - this.Last_Tick) / 63;
                    Console.WriteLine("Preparation Time : " + this.Preparation_Time);
                }
                else
                {
                    this.Attack_Time -= (value - this.Last_Tick) / 63;
                    Console.WriteLine("Attack Time      : " + this.Attack_Time);
                }
                this.Last_Tick = value;
                //this.End_Tick = (int)value;

            }
        }
    }
}
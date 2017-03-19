using System;
namespace UCS.Logic.JSONProperty
{
    using System.Collections.Generic;
    using System.Linq;

    internal class Achievements : List<Slot>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Achievements"/> class.
        /// </summary>
        /// 
        internal Achievements()
        {
            // Achievements.
        }

        internal List<Slot> Completed
        {
            get
            {
                return this.Where(Achievement => Achievement.Count == -1).ToList();
            }
        }

        internal new void Add(Slot Achievement)
        {
            if (this.FindIndex(A => A.Data == Achievement.Data) < 0)
                this.Add(Achievement);
        }

    }
}

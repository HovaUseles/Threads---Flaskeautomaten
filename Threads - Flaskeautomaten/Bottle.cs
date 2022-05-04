using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    public class Bottle
    {
        /// <summary>
        /// Bottle ID number.
        /// </summary>
        private int number;
        public int Number
        {
            get { return number; }
            private set { number = value; }
        }

        /// <summary>
        /// Bottle type.
        /// </summary>
        private string type;
        public string Type
        {
            get { return type; }
            private set { type = value; }
        }

        /// <summary>
        /// Inialize an instance of a bottle. Needs an ID number and a type.
        /// </summary>
        /// <param name="runningNumber"></param>
        /// <param name="bottleType"></param>
        public Bottle(int runningNumber, string bottleType)
        {
            number = runningNumber;
            type = bottleType;
        }
    }
}

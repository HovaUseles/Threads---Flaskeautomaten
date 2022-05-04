using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    public class Bottle
    {
        private int number;

        public int Number
        {
            get { return number; }
            private set { number = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            private set { type = value; }
        }


        public Bottle(int runningNumber, string bottleType)
        {
            number = runningNumber;
            type = bottleType;
        }
    }
}

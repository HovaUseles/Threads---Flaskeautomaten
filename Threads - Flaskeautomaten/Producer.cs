using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    internal class Producer
    {
        private int runningNumber = 1000;
        private int produceTime;
        private Random random = new Random();
        private Thread thread;
        public Thread GetThread
        {
            get { return thread; }
            private set { thread = value; }
        }


        public Producer()
        {
            produceTime = random.Next(400, 1600);
            thread = new Thread(ProductionController);
            thread.Name = "Producer Thread";
            thread.Priority = ThreadPriority.Normal;
        }

        public void StartProducing()
        {
            thread.Start();
        }
        private void ProductionController()
        {
            while (Program.running)
            {
                if (Monitor.TryEnter(Program.producerConveyor))
                {
                    if (Program.producerConveyor.Count < Program.producerConLimit)
                    {
                        // Create bottle object
                        Bottle bottle = ProduceBottle();
                        Console.WriteLine("Producer produces a {0} bottle [{1}]", bottle.Type, bottle.Number);
                        Program.producerConveyor.Enqueue(bottle);
                        Console.WriteLine("Bottles on producer conveyor: {0}", Program.producerConveyor.Count);
                        Monitor.Pulse(Program.producerConveyor);
                        Monitor.Exit(Program.producerConveyor);
                    }
                    else
                    {
                        Console.WriteLine("Producer is waiting...");
                        Monitor.Pulse(Program.producerConveyor);
                        Monitor.Wait(Program.producerConveyor, Program.waitTimeout);
                    }
                }
                Thread.Sleep(Program.standardTimeout);
            }
        }

        private Bottle ProduceBottle()
        {
            int bottleChoice = random.Next(0, 100);
            string bottleType;
            if (bottleChoice >= 50)
            {
                bottleType = "Beer";
            }
            else
            {
                bottleType = "Soda";
            }
            int bottleNumber = runningNumber++;
            Bottle bottle = new Bottle(bottleNumber, bottleType);
            Thread.Sleep(produceTime);
            return bottle;
        }
    }
}

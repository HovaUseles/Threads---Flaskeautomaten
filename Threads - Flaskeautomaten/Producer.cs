using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    /// <summary>
    /// The bottle producer responsible for producing bottles.
    /// </summary>
    internal class Producer
    {
        private Random random = new Random();
        
        /// <summary>
        /// Bottle ID number, increments for every produces bottle.
        /// </summary>
        private int runningNumber = 1000;

        /// <summary>
        /// Time it takes for the producer to produce a bottle.
        /// </summary>
        private int produceTime;

        /// <summary>
        /// Thread running the producer operations.
        /// </summary>
        private Thread thread;
        public Thread GetThread
        {
            get { return thread; }
            private set { thread = value; }
        }

        /// <summary>
        /// Inialize an instance of Producer. It randomnizes a produceTime.
        /// </summary>
        public Producer()
        {
            produceTime = random.Next(400, 1600);
            thread = new Thread(ProductionController);
            thread.Name = "Producer Thread";
            thread.Priority = ThreadPriority.Normal;
        }

        /// <summary>
        /// Start the producer thread
        /// </summary>
        public void StartProducing()
        {
            thread.Start();
        }

        /// <summary>
        /// Runs the loop of the bottle production. 
        /// </summary>
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

        /// <summary>
        /// Creates adn returns a bottle instance.
        /// </summary>
        /// <returns>Bottle. Either Beer or Soda</returns>
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

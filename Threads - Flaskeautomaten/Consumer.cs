using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    internal class Consumer
    {
        private Random random = new Random();

        /// <summary>
        /// The amount of time the consumer takes to consume a bottle. Set randomly on creation.
        /// </summary>
        private int consumeTime;

        /// <summary>
        /// Type of liquid the consumer drinks.
        /// </summary>
        private string liquidType;
        public string LiquidType
        {
            get { return liquidType; }
            private set { liquidType = value; }
        }

        /// <summary>
        /// Thread runing the consumer methods.
        /// </summary>
        private Thread thread;
        public Thread GetThread
        {
            get { return thread; }
            private set { thread = value; }
        }

        /// <summary>
        /// The queue connected to the consumer.
        /// </summary>
        private Queue<Bottle> conveyor;
        public Queue<Bottle> Conveyor
        {
            get { return conveyor; }
            private set { conveyor = value; }
        }

        /// <summary>
        /// Inializes a new instance of consumer. The consumer removes a bottle from a conveyor queue.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="givenConveyor"></param>
        public Consumer(string type, Queue<Bottle> givenConveyor)
        {
            liquidType = type;
            conveyor = givenConveyor;
            thread = new Thread(ConsumeController);
            thread.Name = type + " Consumer";
            thread.Priority = ThreadPriority.BelowNormal;
            consumeTime = random.Next(5000, 10000);
        }
        /// <summary>
        /// Method to start the consumer thread.
        /// </summary>
        public void StartConsuming()
        {
            thread.Start();
        }

        /// <summary>
        /// Loop running the consumer operations.
        /// </summary>
        private void ConsumeController()
        {
            while (Program.running)
            {

                // Attempt to enter the production conveyor
                if (Monitor.TryEnter(this.conveyor))
                {
                    // If access is gained check if there are any bottles in the queue
                    if (this.conveyor.Count > 0)
                    {
                        Consume();
                    }
                    else
                    {
                        // If no bottles on the conveyor, wait for splitter to add one.
                        Console.WriteLine("{0} Consumer is waiting....", this.liquidType);
                        Monitor.Pulse(this.conveyor);
                        Monitor.Wait(this.conveyor, Program.waitTimeout);
                    }
                }
                Thread.Sleep(Program.standardTimeout);
            }
        }
        
        /// <summary>
        /// Removes a bottle from the conveyor connected. And waits the time it takes for this consumer.
        /// </summary>
        private void Consume()
        {
            // Remove bottle from the conveyor
            Bottle bottle = this.conveyor.Dequeue();
            Monitor.Pulse(this.conveyor);
            Monitor.Exit(this.conveyor);
            Console.WriteLine("{0} Consumer consume {1}[{2}]", this.liquidType, bottle.Type, bottle.Number);
            Thread.Sleep(this.consumeTime);
        }
    }
}

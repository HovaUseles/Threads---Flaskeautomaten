using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    internal class Splitter
    {
        private int splitTime;
        private Random random = new Random();
        private Thread thread;
        public Thread GetThread
        {
            get { return thread; }
            private set { thread = value; }
        }

        public Splitter()
        {
            splitTime = random.Next(50, 100);
            thread = new Thread(SplitterController);
            thread.Name = "Splitter Thread";
            thread.Priority = ThreadPriority.Highest;
        }
        
        public void StartSplitting()
        {
            thread.Start();
        }

        private void SplitterController()
        {
            Bottle bottle;

            while (Program.running)
            {
                // Attempt to enter the production conveyor
                if (Monitor.TryEnter(Program.producerConveyor))
                {
                    // If access is gained check if there are any bottles in the queue
                    if (Program.producerConveyor.Count > 0)
                    {
                        // Only grab a bottle and release the lock on production conveyor to enable producer to keep producing
                        bottle = Program.producerConveyor.Dequeue();
                        Monitor.Pulse(Program.producerConveyor);
                        Monitor.Exit(Program.producerConveyor);

                        // Start sorting the bottle
                        SortBottle(bottle);
                    }
                    else
                    {
                        // If no bottles on the conveyor, wait for producer to make one.
                        Wait(Program.producerConveyor);
                    }
                }
                Thread.Sleep(Program.standardTimeout);
            }
        }

        private void SortBottle(Bottle bottle)
        {
            bool success = false;
            while (!success && Program.running)
            {
                // Test bottle type and add it to the right conveyor
                switch (bottle.Type)
                {
                    case "Beer":
                        success = AddToQueue(bottle, Program.beerConveyor);
                        break;
                    case "Soda":
                        success = AddToQueue(bottle, Program.sodaConveyor);
                        break;
                }
                Thread.Sleep(Program.standardTimeout);
            }
        }

        private bool AddToQueue(Bottle bottle, Queue<Bottle> queue)
        {
            // Attempts access to queue
            if (Monitor.TryEnter(queue))
            {
                if (queue.Count < Program.consumerConLimit)
                {
                    // If theres room on beer/soda conveyor
                    Console.WriteLine("Splitter adding [{0}] to {1} conveyor", bottle.Number, bottle.Type);
                    // Wait for the amount of time it takes Splitter to move bottle
                    Thread.Sleep(splitTime);
                    queue.Enqueue(bottle);
                    Console.WriteLine("Bottles on {0} conveyor: {1}", bottle.Type, queue.Count);

                    Monitor.Pulse(queue);
                    Monitor.Exit(queue);

                    return true;
                }
                else
                {
                    //  else wait for consumer to remove one
                    Wait(queue);
                    return false;
                }
            }
            else return false;
        }

        // Notifies that the queue is released and the splitter is waiting.
        private void Wait(Queue<Bottle> queue)
        {
            Console.WriteLine("Splitters is waiting.....");
            Monitor.Pulse(queue);
            Monitor.Wait(queue, Program.waitTimeout);
        }
    }
}

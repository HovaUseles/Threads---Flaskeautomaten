using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads___Flaskeautomaten
{
    internal class Splitter
    {
        private Random random = new Random();

        /// <summary>
        /// Time it takes for the splitter to sort a bottle.
        /// </summary>
        private int splitTime;
        
        /// <summary>
        /// The thread that runs the splitter operation.
        /// </summary>
        private Thread thread;
       public Thread GetThread
        {
            get { return thread; }
            private set { thread = value; }
        }

        /// <summary>
        /// Inializes an instance of Splitter
        /// </summary>
        public Splitter()
        {
            splitTime = random.Next(50, 100);
            thread = new Thread(SplitterController);
            thread.Name = "Splitter Thread";
            thread.Priority = ThreadPriority.Highest;
        }
        
        /// <summary>
        /// Starts the splitter thread.
        /// </summary>
        public void StartSplitting()
        {
            thread.Start();
        }

        /// <summary>
        /// Controls the loop the splitter thread runs.
        /// </summary>
        private void SplitterController()
        {
            while (Program.running)
            {
                // Attempt to enter the production conveyor
                if (Monitor.TryEnter(Program.producerConveyor))
                {
                    // If access is gained check if there are any bottles in the queue
                    if (Program.producerConveyor.Count > 0)
                    {
                        // Only grab a bottle and release the lock on production conveyor to enable producer to keep producing
                        Bottle bottle = Program.producerConveyor.Dequeue();
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

        /// <summary>
        /// Sort a bottle to its queue.
        /// </summary>
        /// <param name="bottle"></param>
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

        /// <summary>
        /// Attempts to add a bottle to a queue. Returns whether is was successful or not.
        /// </summary>
        /// <param name="bottle"></param>
        /// <param name="queue"></param>
        /// <returns>Whether or not it could access the conveyor queue.</returns>
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
                    //Thread.Sleep(splitTime);
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

        /// <summary>
        /// Notifies that the queue is released and the splitter is waiting.
        /// </summary>
        /// <param name="queue"></param>
        private void Wait(Queue<Bottle> queue)
        {
            Console.WriteLine("Splitter is waiting.....");
            Monitor.Pulse(queue);
            Monitor.Wait(queue, Program.waitTimeout);
        }
    }
}

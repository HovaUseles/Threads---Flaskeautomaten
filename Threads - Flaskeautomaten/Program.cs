using System;
using System.Threading;

namespace Threads___Flaskeautomaten
{
    public static class Program
    {
        /// <summary>
        /// Used to keep all thread loops running, also end all if set false.
        /// </summary>
        public static bool running = true;

        /// <summary>
        /// Defines the sleep timer on while loops to conserve CPU resource.
        /// </summary>
        public static int standardTimeout = 100 / 15;

        /// <summary>
        /// Set a global timeout on all waits
        /// </summary>
        public static int waitTimeout = 6000;

        /// <summary>
        /// Defines limit on producer conveyor.
        /// </summary>
        public static int producerConLimit = 20;
        
        /// <summary>
        /// Defines limit on consumer conveyors.
        /// </summary>
        public static int consumerConLimit = 15;
        
        /// <summary>
        /// First conveyor from producer to splitter. Max limit is set by producerConLimit.
        /// </summary>
        public static Queue<Bottle> producerConveyor = new Queue<Bottle> { };

        /// <summary>
        /// Soda conveyor. Max limit is set by consumerConLimit.
        /// </summary>
        public static Queue<Bottle> sodaConveyor = new Queue<Bottle> { };

        /// <summary>
        /// Beer conveyor. Max limit is set by consumerConLimit.
        /// </summary>
        public static Queue<Bottle> beerConveyor = new Queue<Bottle> { };


        /// <summary>
        /// Main method inializes threads and controls running bool which can end all loops.
        /// </summary>
        static void Main()
        {
            //Objects
            Producer producer = new Producer();
            Splitter splitter = new Splitter();
            Consumer beerConsumer = new Consumer("Beer", beerConveyor);
            Consumer sodaConsumer = new Consumer("Soda", sodaConveyor);

            // Starts the threads
            producer.StartProducing();
            splitter.StartSplitting();
            beerConsumer.StartConsuming();
            sodaConsumer.StartConsuming();

            while (running)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    running = false;
                }
            }

            producer.GetThread.Join();
            splitter.GetThread.Join();
            beerConsumer.GetThread.Join();
            sodaConsumer.GetThread.Join(); 

        }
    }
}
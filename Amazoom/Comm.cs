using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Amazoom
{
	public class Comm
	{
		public Mutex MapMutex
		{ get; }

		//public Mutex ShelfMutex
		//{ get; }

		//public SemaphoreSlim DockingTrucksProducerSemaphore
		//{ get; }

		//public SemaphoreSlim DockingTrucksConsumerSemaphore
		//{ get; }

		//public SemaphoreSlim RobotCommandsProducerSemaphore
		//{ get; }

		//public SemaphoreSlim RobotCommandsConsumerSemaphore
		//{ get; }

		public Comm(int numDocks, int lenRobotCommandQueue)
		{
			this.MapMutex = new Mutex();
			//this.ShelfMutex = new Mutex();

			//this.DockingTrucksProducerSemaphore = 
			//	new SemaphoreSlim(0, numDocks);
			//this.DockingTrucksConsumerSemaphore = 
			//	new SemaphoreSlim(0, numDocks);

			// TODO(CHULIP): Unsure if these semaphores are needed
			//this.RobotCommandsProducerSemaphore = 
			//	new SemaphoreSlim(lenRobotCommandQueue,
			//	lenRobotCommandQueue);
			//this.RobotCommandsConsumerSemaphore = 
			//	new SemaphoreSlim(0, lenRobotCommandQueue);
		}
	}
}

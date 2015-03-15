using System;

namespace LogViewer.Infrastructure
{
	internal class DirectInvoker 
	{
		public static void Invoke(Action run)
		{
			run();
		}
	}
}

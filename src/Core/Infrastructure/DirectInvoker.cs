using System;

namespace LogViewer.Infrastructure
{
	public class DirectInvoker 
	{
		public static void Invoke(Action run)
		{
			run();
		}
	}
}

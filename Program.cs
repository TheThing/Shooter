using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;

namespace Shooter
{
	class Program
	{
		public static void Main(string[] args)
		{
			IScreen screen = new Main();
			while ((screen = screen.Start()) != null) ;
		}
	}
}

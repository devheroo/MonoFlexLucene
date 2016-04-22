using System;

namespace FlexLuceneApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello FlexLucene!");

//			LuceneTest.TestFlexLuceneRAM (args);

			LuceneTest.TestFlexLuceneFS (args);
		}
	}
}

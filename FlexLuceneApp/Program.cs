using System;

namespace FlexLuceneApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try{
			Console.WriteLine ("Hello FlexLucene!");

			Console.WriteLine ("Testing Lucene In Memory:");
			Console.WriteLine ("=====================================================");
			LuceneTest.TestFlexLuceneRAM (args);
			Console.WriteLine ("=====================================================");

			Console.WriteLine ("Testing Lucene using File System:");
			Console.WriteLine ("=====================================================");
			LuceneTest.TestFlexLuceneFS (args);
			Console.WriteLine ("=====================================================");
			}
			catch(Exception ex){
				Console.WriteLine (ex);
			}
		}
	}
}

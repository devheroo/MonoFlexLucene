namespace FlexLuceneApp
{
	using System;
	using FlexLucene.Analysis.Standard;
	using FlexLucene.Store;
	using FlexLucene.Index;
	using FlexLucene.Analysis;
	using FlexLucene.Facet;
	using FlexLucene.Facet.Taxonomy;
	using FlexLucene.Search;
	using FlexLucene.Queryparser.Classic;
	using FlexLucene.Document;
	using FlexLucene.Facet.Sortedset;

	public class LuceneTest
	{

		static TaxonomyWriter taxoWriter;
		static IndexWriterConfig config;
		static FacetsConfig cnf;
		static FlexLucene.Store.Directory taxoDir;

		public static bool IsLinux
		{
			get
			{
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}


		public static void TestFlexLuceneFS (string[] args)
		{
			string ixdir = string.Empty;
			if (IsLinux)
			{
				ixdir = "/home/master/flexlucene/indexes";
			}
			else
			{
				ixdir = @"c:\temp\flexlucene\indexes";
			}

			StandardAnalyzer analyzer = new StandardAnalyzer ();
			//FlexLucene.Store.Directory index = SimpleFSDirectory.Open(java.nio.file.Paths.get(ixdir),SimpleFSLockFactory.INSTANCE);
			FlexLucene.Store.Directory index = SimpleFSDirectory.Open(java.nio.file.Paths.get(ixdir));
//			FlexLucene.Store.Directory index = NIOFSDirectory.Open(java.nio.file.Paths.get(ixdir));

		


			config = new IndexWriterConfig (analyzer);
			cnf = new FacetsConfig ();
			cnf.SetIndexFieldName ("title", "facet_title");
			cnf.SetIndexFieldName ("isbn", "facet_isbn");
			LuceneTest.taxoDir = (FlexLucene.Store.Directory)new RAMDirectory ();
			LuceneTest.taxoWriter = (TaxonomyWriter)new FlexLucene.Facet.Taxonomy.Directory.DirectoryTaxonomyWriter (LuceneTest.taxoDir, IndexWriterConfigOpenMode.CREATE);
			IndexWriter w = new IndexWriter (index, LuceneTest.config);

			addDoc (w, "Lucene in Action", "9900001");
			addDoc (w, "Lucene for Dummies", "9900002");
			addDoc (w, "Lucene for Dummies 2", "9900003");
		
			w.close ();
			String querystr = "isbn:99*";
		
			Query q = new QueryParser ("title", (Analyzer)analyzer).Parse (querystr);
			int hitsPerPage = 10;
			IndexReader reader = (IndexReader)DirectoryReader.Open (index);
			IndexSearcher searcher = new IndexSearcher (reader);
			TopScoreDocCollector collector = TopScoreDocCollector.Create (hitsPerPage);
			searcher.Search (q, (Collector)collector);
			ScoreDoc[] hits = collector.TopDocs ().ScoreDocs;
			Console.WriteLine ("Found " + hits.Length + " hits.");
			for (int i = 0; i < hits.Length; ++i) {
				int docId = hits [i].Doc;
				Document d = searcher.Doc (docId);
				Console.WriteLine (i + 1 + ". " + d.Get ("isbn") + "\t" + d.Get ("title"));
			}
			SortedSetDocValuesReaderState state = (SortedSetDocValuesReaderState)new DefaultSortedSetDocValuesReaderState (reader, "facet_isbn");
			FacetsCollector fc = new FacetsCollector ();
			FacetsCollector.Search (searcher, q, 10, (Collector)fc);
			Facets facets = (Facets)new SortedSetDocValuesFacetCounts (state, fc);
			FacetResult result = facets.GetTopChildren (10, "isbn", new String[0]);
			for (int j = 0; j < result.ChildCount; ++j) {				
				LabelAndValue lv = result.LabelValues [j];
				Console.WriteLine (String.Format ("Label={0}, Value={1}", lv.Label, lv.Value));
			}
			reader.close ();
		}


		public static void TestFlexLuceneRAM (string[] args)
		{

			StandardAnalyzer analyzer = new StandardAnalyzer ();
			FlexLucene.Store.Directory index = (FlexLucene.Store.Directory)new RAMDirectory ();
			config = new IndexWriterConfig ((Analyzer)analyzer);
			cnf = new FacetsConfig ();
			cnf.SetIndexFieldName ("title", "facet_title");
			cnf.SetIndexFieldName ("isbn", "facet_isbn");
			LuceneTest.taxoDir = (FlexLucene.Store.Directory)new RAMDirectory ();
			LuceneTest.taxoWriter = (TaxonomyWriter)new FlexLucene.Facet.Taxonomy.Directory.DirectoryTaxonomyWriter (LuceneTest.taxoDir, IndexWriterConfigOpenMode.CREATE);

			IndexWriter w = new IndexWriter (index, LuceneTest.config);
			addDoc (w, "Lucene in Action", "9900001");
			addDoc (w, "Lucene for Dummies", "9900002");
			addDoc (w, "Lucene for Dummies 2", "9900003");

			w.close ();
			String querystr = "isbn:99*";
			Query q = new QueryParser ("title", (Analyzer)analyzer).Parse (querystr);
			int hitsPerPage = 10;
			IndexReader reader = (IndexReader)DirectoryReader.Open (index);
			IndexSearcher searcher = new IndexSearcher (reader);
			TopScoreDocCollector collector = TopScoreDocCollector.Create (hitsPerPage);
			searcher.Search (q, (Collector)collector);
			ScoreDoc[] hits = collector.TopDocs ().ScoreDocs;
			Console.WriteLine ("Found " + hits.Length + " hits.");
			for (int i = 0; i < hits.Length; ++i) {
				int docId = hits [i].Doc;
				Document d = searcher.Doc (docId);
				Console.WriteLine (i + 1 + ". " + d.Get ("isbn") + "\t" + d.Get ("title"));
			}
			SortedSetDocValuesReaderState state = (SortedSetDocValuesReaderState)new DefaultSortedSetDocValuesReaderState (reader, "facet_isbn");
			FacetsCollector fc = new FacetsCollector ();
			FacetsCollector.Search (searcher, q, 10, (Collector)fc);
			Facets facets = (Facets)new SortedSetDocValuesFacetCounts (state, fc);
			FacetResult result = facets.GetTopChildren (10, "isbn", new String[0]);
			for (int j = 0; j < result.ChildCount; ++j) {				
				LabelAndValue lv = result.LabelValues [j];
				Console.WriteLine (String.Format ("Label={0}, Value={1}", lv.Label, lv.Value));
			}
			reader.close ();
		}

		private static void addDoc (IndexWriter w, String title, String isbn)
		{
			Document doc = new Document ();
			doc.Add ((IndexableField)new SortedSetDocValuesFacetField ("title", title));
			doc.Add ((IndexableField)new SortedSetDocValuesFacetField ("isbn", isbn));
			doc.Add ((IndexableField)new TextField ("title", title, FieldStore.YES));
			doc.Add ((IndexableField)new StringField ("isbn", isbn, FieldStore.YES));
			w.AddDocument (LuceneTest.cnf.Build (LuceneTest.taxoWriter, doc));
		}
	}
}





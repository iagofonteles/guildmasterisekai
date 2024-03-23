using NUnit.Framework;
using Drafts.Patterns;
using System.Linq;

public class ListWatcherTests {

	class Item { }

	Item a = new Item();
	Item b = new Item();
	Item c = new Item();
	Item d = new Item();

	[Test]
	public void AddOne() {
		var list = new ListWatcher<Item>();
		list.Add(null, b);
		Assert.AreEqual(list[0], b);

		list.Add(null, c);
		Assert.AreEqual(list[1], c);

		list.Add(null, a);
		Assert.IsTrue(list.Contains(a));
	}

	[Test]
	public void AddMany() {
		var list = new ListWatcher<Item>();
		list.AddRange(null, new[] { a, b, c, d });
		Assert.IsTrue(list.Count == 4);
		Assert.AreEqual(list[1], b);
	}

	[Test]
	public void RemoveAll() {
		var list = new ListWatcher<Item>();
		list.AddRange(null, new[] { a, b, b, c, d });

		var itens = list.RemoveAll(null, i => i == b);
		Assert.AreEqual(list.Count, 3);
		Assert.AreEqual(itens.Count(), 2);

		var selection = new[] { c, d };
		itens = list.RemoveAll(null, selection.Contains);
		Assert.AreEqual(list.Count, 1);
		Assert.AreEqual(itens.Count(), 2);

		Assert.AreEqual(list[0], a);
	}

	[Test]
	public void Clear() {
		var list = new ListWatcher<Item>();
		list.AddRange(null, new[] { a, b, c, d });

		Assert.AreEqual(list.Count, 4);

		var itens = list.Clear(null);
		Assert.AreEqual(list.Count, 0);
		Assert.AreEqual(itens.Count(), 4);
	}
}

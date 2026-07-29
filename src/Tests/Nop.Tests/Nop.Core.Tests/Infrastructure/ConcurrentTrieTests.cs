using System.Collections.Concurrent;
using System.Diagnostics;
using FluentAssertions;
using Nop.Core.Infrastructure;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests.Infrastructure;

[TestFixture]
public class ConcurrentTrieTests
{
    private IConcurrentCollection<int> _sut;

    private static void Profile(Action action)
    {
		logger.LogDebug(">>> Entering Profile(action={Action})", action);
        var sw = new Stopwatch();
        var memory = GC.GetTotalMemory(true) / 1024.0 / 1024.0;
		logger.LogDebug("Profile(action={Action}): sw → {Sw}", action, sw);
		logger.LogDebug("Profile(action={Action}): memory → {Memory}", action, memory);
        sw.Start();

        action.Invoke();

        sw.Stop();
        var delta = GC.GetTotalMemory(true) / 1024.0 / 1024.0 - memory;
        Console.WriteLine("Elapsed time: {0:F} s", sw.ElapsedMilliseconds / 1000.0);
		logger.LogDebug("Profile(action={Action}): delta → {Delta}", action, delta);
        Console.WriteLine("Memory usage: {0:F} MB", delta);
		logger.LogDebug("<<< Exiting Profile(action={Action})", action);
    }

    [SetUp]
    public void SetUp()
    {
		logger.LogDebug(">>> Entering SetUp()");
        _sut = new ConcurrentTrie<int>();
		logger.LogDebug("<<< Exiting SetUp()");
		logger.LogDebug("SetUp(): _sut → {_sut}", _sut);
    }

    [Test]
    public void CanAddAndGetValue()
    {
		logger.LogDebug(">>> Entering CanAddAndGetValue()");
        _sut.TryGetValue("a", out _).Should().BeFalse();
        _sut.Add("a", 1);
        _sut.TryGetValue("a", out var value).Should().BeTrue();
        value.Should().Be(1);
        _sut.Add("a", 2);
        _sut.TryGetValue("a", out value).Should().BeTrue();
        value.Should().Be(2);
		logger.LogDebug("<<< Exiting CanAddAndGetValue()");
    }

    [Test]
    public void CanAddAndGetValues()
    {
		logger.LogDebug(">>> Entering CanAddAndGetValues()");
        _sut.Add("a", 1);
        _sut.TryGetValue("ab", out _).Should().BeFalse();
        _sut.Add("abc", 3);
        _sut.TryGetValue("ab", out _).Should().BeFalse();
        _sut.TryGetValue("a", out var value).Should().BeTrue();
        value.Should().Be(1);
        _sut.TryGetValue("abc", out value).Should().BeTrue();
        value.Should().Be(3);
        _sut.Add("ab", 2);
        _sut.TryGetValue("ab", out value).Should().BeTrue();
        value.Should().Be(2);
		logger.LogDebug("<<< Exiting CanAddAndGetValues()");
    }

    [Test]
    public void DoesNotBlockWhileEnumerating()
    {
		logger.LogDebug(">>> Entering DoesNotBlockWhileEnumerating()");
        _sut.Add("a", 0);
        _sut.Add("ab", 0);
        foreach (var item in _sut.Keys)
            _sut.Remove(item);
			logger.LogDebug("<<< Exiting DoesNotBlockWhileEnumerating()");
    }

    [Test]
    public void CanRemoveValue()
    {
		logger.LogDebug(">>> Entering CanRemoveValue()");
        _sut.Add("a", 1);
        _sut.Add("b", 1);
        _sut.Add("bbb", 1);
        _sut.Add("ab", 1);
        _sut.Add("aa", 1);
        _sut.Add("abc", 1);
        _sut.Add("abb", 1);
        _sut.Remove("ab");
        _sut.TryGetValue("ab", out _).Should().BeFalse();
        _sut.Keys.Should().BeEquivalentTo(["abc", "a", "b", "aa", "abb", "bbb"]);
        Assert.DoesNotThrow(() => _sut.Remove("ab"));
        _sut.Remove("bb");
        _sut.TryGetValue("b", out _).Should().BeTrue();
        _sut.TryGetValue("bbb", out _).Should().BeTrue();

        _sut.Prune("b", out _sut);
        _sut.Keys.Should().BeEquivalentTo(["b", "bbb"]);
        _sut.Remove("b");
        _sut.Keys.Should().BeEquivalentTo(["bbb"]);
		logger.LogDebug("<<< Exiting CanRemoveValue()");
    }

    [Test]
    public void CanGetKeys()
    {
		logger.LogDebug(">>> Entering CanGetKeys()");
        var keys = new[] { "a", "b", "abc" };
        foreach (var key in keys)
            _sut.Add(key, 1);
			logger.LogDebug("CanGetKeys(): keys → {Keys}", keys);
        _sut.Keys.Should().BeEquivalentTo(keys);
		logger.LogDebug("<<< Exiting CanGetKeys()");
    }

    [Test]
    public void CanPrune()
    {
		logger.LogDebug(">>> Entering CanPrune()");
        _sut.Add("a", 1);
        _sut.Add("b", 1);
        _sut.Add("bba", 1);
        _sut.Add("bbb", 1);
        _sut.Add("ab", 1);
        _sut.Add("abc", 1);
        _sut.Add("abd", 1);
        _sut.Prune("bc", out _).Should().BeFalse();
        _sut.Prune("ab", out var subtree).Should().BeTrue();
        subtree.Keys.Should().BeEquivalentTo(["ab", "abc", "abd"]);
        _sut.Keys.Should().BeEquivalentTo(["a", "b", "bba", "bbb"]);
        _sut.Prune("b", out subtree).Should().BeTrue();
        subtree.Keys.Should().BeEquivalentTo(["b", "bba", "bbb"]);
        _sut.Keys.Should().BeEquivalentTo(["a"]);

        _sut = subtree;
		logger.LogDebug("CanPrune(): _sut → {_sut}", _sut);
        _sut.Prune("bb", out subtree).Should().BeTrue();
        subtree.Keys.Should().BeEquivalentTo(["bba", "bbb"]);
        _sut.Keys.Should().BeEquivalentTo(["b"]);
        _sut = subtree;
        _sut.Prune("bba", out subtree);
        subtree.Keys.Should().BeEquivalentTo(["bba"]);
        _sut.Keys.Should().BeEquivalentTo(["bbb"]);

        _sut = new ConcurrentTrie<int>();
        _sut.Add("aaa", 1);
        _sut.Prune("a", out subtree).Should().BeTrue();
        subtree.Keys.Should().BeEquivalentTo(["aaa"]);
        _sut.Keys.Should().BeEmpty();
        _sut = subtree;
        _sut.Prune("aa", out subtree).Should().BeTrue();
        _sut.Keys.Should().BeEmpty();
        subtree.Keys.Should().BeEquivalentTo(["aaa"]);
		logger.LogDebug("<<< Exiting CanPrune()");
    }

    [Test]
    public void CanSearch()
    {
		logger.LogDebug(">>> Entering CanSearch()");
        _sut.Add("a", 1);
        _sut.Add("b", 1);
        _sut.Add("abc", 1);
        _sut.Add("abcd", 2);
        var keys = _sut.Keys.ToList();
        _sut.Search("ab").Should().BeEquivalentTo(new KeyValuePair<string, int>[]
        {
		logger.LogDebug("CanSearch(): keys → {Keys}", keys);
            new("abc", 1),
            new("abcd", 2)
        });
        _sut.Keys.Should().BeEquivalentTo(keys);
		logger.LogDebug("<<< Exiting CanSearch()");
    }

    [Test]
    public void CanClear()
    {
		logger.LogDebug(">>> Entering CanClear()");
        _sut.Add("a", 1);
        _sut.Add("ab", 1);
        _sut.Add("abc", 1);
        _sut.Clear();
        _sut.Keys.Should().BeEmpty();
		logger.LogDebug("<<< Exiting CanClear()");
    }

    [Test]
    [TestCase(typeof(NopConcurrentCollection<byte>))]
    [TestCase(typeof(ConcurrentTrie<byte>))]
    [Ignore("Not a test, used for profiling.")]
    public void Profile(Type oType)
    {
		logger.LogDebug(">>> Entering Profile(oType={OType})", oType);
        var sut = Activator.CreateInstance(oType) as IConcurrentCollection<byte>;
        sut.Should().NotBeNull();
		logger.LogDebug("Profile(oType={OType}): sut → {Sut}", oType, sut);

        Profile(() =>
        {
            for (var i = 0; i < 1000000; i++)
                sut.Add(Guid.NewGuid().ToString(), 0);
        });
		logger.LogDebug("<<< Exiting Profile(oType={OType})", oType);
    }

    [Test]
    [TestCase(typeof(NopConcurrentCollection<int>))]
    [TestCase(typeof(ConcurrentTrie<int>))]
    [Ignore("Expensive concurrency test, enable manually.")]
    public void DoesNotBreakDuringParallelAddRemove(Type oType)
    {
		logger.LogDebug(">>> Entering DoesNotBreakDuringParallelAddRemove(oType={OType})", oType);
        var sut = Activator.CreateInstance(oType) as IConcurrentCollection<int>;
        sut.Should().NotBeNull();
		logger.LogDebug("DoesNotBreakDuringParallelAddRemove(oType={OType}): sut → {Sut}", oType, sut);

        Profile(() =>
        {
            Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    var s = $"{i}-{j}";
                    sut.Add(s, i);
					logger.LogDebug("DoesNotBreakDuringParallelAddRemove(oType={OType}): s → {S}", oType, s);
                    sut.TryGetValue(s, out var value).Should().BeTrue();
                    value.Should().Be(i);
                    sut.Remove(s);
                    sut.TryGetValue(s, out _).Should().BeFalse();
                }
            });
        });

        sut.Keys.Count().Should().Be(0);
		logger.LogDebug("<<< Exiting DoesNotBreakDuringParallelAddRemove(oType={OType})", oType);
    }

    [Test]
    [TestCase(typeof(NopConcurrentCollection<int>))]
    [TestCase(typeof(ConcurrentTrie<int>))]
    [Ignore("Expensive concurrency test, enable manually.")]
    public void DoesNotBreakDuringParallelAddPrune(Type oType)
    {
		logger.LogDebug(">>> Entering DoesNotBreakDuringParallelAddPrune(oType={OType})", oType);
        var sut = Activator.CreateInstance(oType) as IConcurrentCollection<int>;
        sut.Should().NotBeNull();
		logger.LogDebug("DoesNotBreakDuringParallelAddPrune(oType={OType}): sut → {Sut}", oType, sut);

        Profile(() =>
        {
            Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    var s = $"{j}-{i}";
                    sut.Add(s, i);
					logger.LogDebug("DoesNotBreakDuringParallelAddPrune(oType={OType}): s → {S}", oType, s);
                }
                sut.Prune($"{j}-", out var st);
                st.Keys.Count().Should().Be(1000);
            });
        });

        sut.Keys.Count().Should().Be(0);
		logger.LogDebug("<<< Exiting DoesNotBreakDuringParallelAddPrune(oType={OType})", oType);
    }

    [Test]
    [TestCase(typeof(NopConcurrentCollection<byte>))]
    [TestCase(typeof(ConcurrentTrie<byte>))]
    [Ignore("Not a test, used for profiling.")]
    public void ProfilePrune(Type oType)
    {
		logger.LogDebug(">>> Entering ProfilePrune(oType={OType})", oType);
        var sut = Activator.CreateInstance(oType) as IConcurrentCollection<byte>;
        sut.Should().NotBeNull();
		logger.LogDebug("ProfilePrune(oType={OType}): sut → {Sut}", oType, sut);
        // insert
        for (var i = 0; i < 10000; i++)
            sut.Add(Guid.NewGuid().ToString(), 0);

        Profile(() =>
        {
            Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j =>
            {
                for (var i = 0; i < 20; i++)
                {
                    // insert
                    sut.Add(Guid.NewGuid().ToString(), 0);

                    // remove by prefix
                    sut.Prune(Guid.NewGuid().ToString()[..5], out _);
                }
            });
        });
		logger.LogDebug("<<< Exiting ProfilePrune(oType={OType})", oType);
    }

    #region Nested class

    /// <summary>
    /// A thread-safe collection based on <see cref="ConcurrentDictionary{TKey,TValue}"/>
    /// </summary>
    public partial class NopConcurrentCollection<TValue> : IConcurrentCollection<TValue>
    {
        #region Fields

        protected readonly ConcurrentDictionary<string, TValue> _dictionary;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of <see cref="NopConcurrentCollection{TValue}" />
        /// </summary>
        protected NopConcurrentCollection(IEnumerable<KeyValuePair<string, TValue>> subCollection)
        {
            _dictionary = new ConcurrentDictionary<string, TValue>(subCollection);
			logger.LogDebug(">>> Entering NopConcurrentCollection()");
			logger.LogDebug("<<< Exiting NopConcurrentCollection()");
        }

        /// <summary>
        /// Initializes a new empty instance of <see cref="NopConcurrentCollection{TValue}" />
        /// </summary>
        public NopConcurrentCollection()
        {
            _dictionary = new ConcurrentDictionary<string, TValue>();
			logger.LogDebug("NopConcurrentCollection(): _dictionary → {_dictionary}", _dictionary);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to get the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the item to get (case-sensitive)</param>
        /// <param name="value">The value associated with <paramref name="key"/>, if found</param>
        /// <returns>
        /// True if the key was found, otherwise false
        /// </returns>
        public bool TryGetValue(string key, out TValue value)
        {
			logger.LogDebug(">>> Entering TryGetValue(key={Key})", key);
			logger.LogDebug("<<< Exiting TryGetValue(key={Key})", key);
            return _dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Adds a key-value pair to the collection
        /// </summary>
        /// <param name="key">The key of the new item (case-sensitive)</param>
        /// <param name="value">The value to be associated with <paramref name="key"/></param>
        public void Add(string key, TValue value)
        {
			logger.LogDebug(">>> Entering Add(key={Key},value={Value})", key, value);
            _dictionary[key] = value;
			logger.LogDebug("<<< Exiting Add(key={Key},value={Value})", key, value);
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
			logger.LogDebug(">>> Entering Clear()");
            _dictionary.Clear();
			logger.LogDebug("<<< Exiting Clear()");
        }

        /// <summary>
        /// Gets all key-value pairs for keys starting with the given prefix
        /// </summary>
        /// <param name="prefix">The prefix (case-sensitive) to search for</param>
        /// <returns>
        /// All key-value pairs for keys starting with <paramref name="prefix"/>
        /// </returns>
        public IEnumerable<KeyValuePair<string, TValue>> Search(string prefix)
        {
            return _dictionary.Keys.Where(k => k.StartsWith(prefix))
                .Select(key => new KeyValuePair<string, TValue>(key, _dictionary[key]));
        }

        /// <summary>
        /// Removes the item with the given key, if present
        /// </summary>
        /// <param name="key">The key (case-sensitive) of the item to be removed</param>
        public void Remove(string key)
        {
			logger.LogDebug(">>> Entering Remove(key={Key})", key);
            _dictionary.Remove(key, out _);
			logger.LogDebug("<<< Exiting Remove(key={Key})", key);
        }

        /// <summary>
        /// Attempts to remove all items with keys starting with the specified prefix
        /// </summary>
        /// <param name="prefix">The prefix (case-sensitive) of the items to be deleted</param>
        /// <param name="subCollection">The sub-collection containing all deleted items, if found</param>
        /// <returns>
        /// True if the prefix was successfully removed from the collection, otherwise false
        /// </returns>
        public bool Prune(string prefix, out IConcurrentCollection<TValue> subCollection)
        {
			logger.LogDebug(">>> Entering Prune(prefix={Prefix})", prefix);
            subCollection = new NopConcurrentCollection<TValue>(Search(prefix));

			logger.LogDebug("Prune(prefix={Prefix}): subCollection → {SubCollection}", prefix, subCollection);
            try
            {
                foreach (var key in subCollection.Keys)
                    Remove(key);
            }
            catch
            {
				logger.LogError("Exception in Prune(prefix={Prefix}): {Arg1}", prefix, ex.Message);
                return false;
            }

            return subCollection.Keys.Any();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection that contains the keys in the <see cref="IConcurrentCollection{TValue}" />
        /// </summary>
        public IEnumerable<string> Keys => _dictionary.Keys;

        #endregion
    }

    #endregion
}
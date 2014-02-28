using System;
using System.Collections;
using System.Diagnostics;

namespace Game2DFramework
{
    public class PriorityQueue : ICollection
    {
        private const int LevelMaxValue = 16;
        private const double Probability = 0.5;

        private readonly Random _rand = new Random();
        private readonly IComparer _comparer;

        private int _currentLevel = 1;
        private long _version;
        private int _count;
        private Node _header = new Node(null, LevelMaxValue);

        public PriorityQueue(IComparer comparer)
        {
            _comparer = comparer ?? new DefaultComparer();
        }

        public PriorityQueue() : this(new DefaultComparer()) { }

        public virtual void Enqueue(object element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var x = _header;
            var update = new Node[LevelMaxValue];
            var nextLevel = NextLevel();

            for (var i = _currentLevel - 1; i >= 0; i--)
            {
                while (x[i] != null && _comparer.Compare(x[i].Element, element) > 0)
                {
                    x = x[i];
                }
                update[i] = x;
            }

            if (nextLevel > _currentLevel)
            {
                for (int i = _currentLevel; i < nextLevel; i++)
                {
                    update[i] = _header;
                }
                _currentLevel = nextLevel;
            }

            var newNode = new Node(element, nextLevel);

            for (int i = 0; i < nextLevel; i++)
            {
                newNode[i] = update[i][i];
                update[i][i] = newNode;
            }
            _count++;
            _version++;

            Debug.Assert(Contains(element), "Contains Test", "Contains test for element " + element + " failed.");
            AssertValid();
        }

        public virtual object Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot dequeue into an empty PriorityQueue.");

            var element = _header[0].Element;
            var oldNode = _header[0];

            for (var i = 0; i < _currentLevel && _header[i] == oldNode; i++)
            {
                _header[i] = oldNode[i];
            }

            while (_currentLevel > 1 && _header[_currentLevel - 1] == null)
            {
                _currentLevel--;
            }
            _count--;
            _version++;

            Debug.Assert(_count >= 0);
            AssertValid();

            return element;
        }

        public virtual void Remove(object element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var x = _header;
            var update = new Node[LevelMaxValue];
            NextLevel();

            for (var i = _currentLevel - 1; i >= 0; i--)
            {
                while (x[i] != null && _comparer.Compare(x[i].Element, element) > 0)
                {
                    x = x[i];
                }
                update[i] = x;
            }

            x = x[0];

            if (x != null && _comparer.Compare(x.Element, element) == 0)
            {
                for (var i = 0; i < _currentLevel && update[i][i] == x; i++)
                {
                    update[i][i] = x[i];
                }

                while (_currentLevel > 1 && _header[_currentLevel - 1] == null)
                {
                    _currentLevel--;
                }
                _count--;
                _version++;
            }

            AssertValid();
        }

        public virtual bool Contains(object element)
        {
            if (element == null)
                return false;

            var x = _header;

            for (var i = _currentLevel - 1; i >= 0; i--)
            {
                while (x[i] != null && _comparer.Compare(x[i].Element, element) > 0)
                {
                    x = x[i];
                }
            }

            x = x[0];

            return x != null && _comparer.Compare(x.Element, element) == 0;
        }

        public virtual object Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot peek into an empty PriorityQueue.");

            return _header[0].Element;
        }

        public virtual void Clear()
        {
            _header = new Node(null, LevelMaxValue);

            _currentLevel = 1;
            _count = 0;
            _version++;

            AssertValid();
        }

        private int NextLevel()
        {
            var nextLevel = 1;

            while (_rand.NextDouble() < Probability &&
                nextLevel < LevelMaxValue &&
                nextLevel <= _currentLevel)
            {
                nextLevel++;
            }

            return nextLevel;
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            var n = 0;
            var x = _header[0];

            while (x != null)
            {
                if (x[0] != null)
                {
                    Debug.Assert(_comparer.Compare(x.Element, x[0].Element) >= 0, "Order test");
                }

                x = x[0];
                n++;
            }

            Debug.Assert(n == Count, "Count test.");

            for (var i = 1; i < _currentLevel; i++)
            {
                Debug.Assert(_header[i] != null, "Level non-null test.");
            }

            for (var i = _currentLevel; i < LevelMaxValue; i++)
            {
                Debug.Assert(_header[i] == null, "Level null test.");
            }
        }

        [Conditional("DEBUG")]
        public static void Test()
        {
            var r = new Random();
            var queue = new PriorityQueue();
            const int count = 1000;
            int element;

            for (var i = 0; i < count; i++)
            {
                element = r.Next();
                queue.Enqueue(element);

                Debug.Assert(queue.Contains(element), "Contains Test");
            }

            Debug.Assert(queue.Count == count, "Count Test");

            var previousElement = (int)queue.Peek();
            for (var i = 0; i < count; i++)
            {
                var peekElement = (int)queue.Peek();
                element = (int)queue.Dequeue();

                Debug.Assert(element == peekElement, "Peek Test");
                Debug.Assert(element <= previousElement, "Order Test");

                previousElement = element;
            }

            Debug.Assert(queue.Count == 0);
        }

        private class DefaultComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (!(y is IComparable))
                {
                    throw new ArgumentException(
                        "Item does not implement IComparable.");
                }
                var a = x as IComparable;
                Debug.Assert(a != null);
                return a.CompareTo(y);
            }
        }

        private class Node
        {
            private readonly Node[] _forward;
            private readonly object _element;

            public Node(object element, int level)
            {
                _forward = new Node[level];
                _element = element;
            }

            public Node this[int index]
            {
                get
                {
                    return _forward[index];
                }
                set
                {
                    _forward[index] = value;
                }
            }

            public object Element
            {
                get
                {
                    return _element;
                }
            }
        }

        private class PriorityQueueEnumerator : IEnumerator
        {
            private readonly PriorityQueue _owner;
            private readonly Node _head;
            private Node _currentNode;
            private bool _moveResult;
            private readonly long _version;

            public PriorityQueueEnumerator(PriorityQueue owner)
            {
                _owner = owner;
                _version = owner._version;
                _head = owner._header;
                Reset();
            }

            public void Reset()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException("The PriorityQueue was modified after the enumerator was created.");

                _currentNode = _head;
                _moveResult = true;
            }

            public object Current
            {
                get
                {
                    if (_currentNode == _head || _currentNode == null)
                        throw new InvalidOperationException(
                            "The enumerator is positioned before the first " +
                            "element of the collection or after the last element.");
                    return _currentNode.Element;
                }
            }

            public bool MoveNext()
            {
                if (_version != _owner._version)
                    throw new InvalidOperationException("The PriorityQueue was modified after the enumerator was created.");

                if (_moveResult)
                    _currentNode = _currentNode[0];

                if (_currentNode == null)
                    _moveResult = false;

                return _moveResult;
            }
        }

        public virtual bool IsSynchronized { get { return false; } }
        public virtual int Count { get { return _count; } }
        public virtual object SyncRoot { get { return this; } }

        public virtual IEnumerator GetEnumerator()
        {
            return new PriorityQueueEnumerator(this);
        }

        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index,"Array index out of range.");
            if (array.Rank > 1)
                throw new ArgumentException("Array has more than one dimension.", "array");
            if (index >= array.Length)
                throw new ArgumentException("index is equal to or greater than the length of array.", "index");
            if (Count > array.Length - index)
                throw new ArgumentException(
                    "The number of elements in the PriorityQueue is greater " +
                    "than the available space from index to the end of the " +
                    "destination array.", "index");

            var i = index;

            foreach (var element in this)
            {
                array.SetValue(element, i);
                i++;
            }
        }
    }
}

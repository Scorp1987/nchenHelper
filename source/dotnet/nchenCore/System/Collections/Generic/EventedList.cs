using System.ComponentModel;

namespace System.Collections.Generic
{
    public class EventedList<T> : IList<T>
    {
        #region Public Events
        public event EventHandler<ListItemAddingEventArgs<T>> ItemAdding;
        public event EventHandler<ListItemAddedEventArgs<T>> ItemAdded;
        public event EventHandler<ListItemChangingEventArgs<T>> ItemChanging;
        public event EventHandler<ListItemChangedEventArgs<T>> ItemChanged;
        public event EventHandler<ListItemRemovingEventArgs<T>> ItemRemoving;
        public event EventHandler<ListItemRemovedEventArgs<T>> ItemRemoved;
        public event EventHandler<ListItemMovingEventArgs<T>> ItemMoving;
        public event EventHandler<ListItemMovedEventArgs<T>> ItemMoved;
        public event EventHandler<CancelEventArgs> ItemClearing;
        public event EventHandler<EventArgs> ItemCleared;
        public event EventHandler<ListRangeItemChangingEventArgs> ItemReversing;
        public event EventHandler<ListRangeItemChangedEventArgs> ItemReversed;
        public event EventHandler<ListRangeItemChangingEventArgs> ItemSorting;
        public event EventHandler<ListRangeItemChangedEventArgs> ItemSorted;
        #endregion


        private void OnChangingChanged<TChangingEventArgs, TChangedEventArgs>(
            Func<TChangingEventArgs> getChangingArgs, EventHandler<TChangingEventArgs> changingHandler, Action action,
            Func<TChangedEventArgs> getChangedArgs, EventHandler<TChangedEventArgs> changedHandler)
            where TChangingEventArgs : CancelEventArgs
            where TChangedEventArgs : EventArgs
        {
            if (changingHandler != null)
            {
                var args = getChangingArgs();
                changingHandler?.Invoke(this, args);
                if (args.Cancel) return;
            }
            action();
            changedHandler?.Invoke(this, getChangedArgs());
        }
        private bool OnChangingChangedResult<TChangingEventArgs, TChangedEventArgs>(
            Func<TChangingEventArgs> getChangingArgs, EventHandler<TChangingEventArgs> changingHandler, Func<bool> action,
            Func<TChangedEventArgs> getChangedArgs, EventHandler<TChangedEventArgs> changedHandler)
            where TChangingEventArgs : CancelEventArgs
            where TChangedEventArgs : EventArgs
        {
            if (changingHandler != null)
            {
                var args = getChangingArgs();
                changingHandler?.Invoke(this, args);
                if (args.Cancel) return false;
            }
            var success = action();
            if (success)
                changedHandler?.Invoke(this, getChangedArgs());
            return success;
        }

        private void OnChange(int index, T newValue, Action action)
        {
            var originalValue = _list[index];
            OnChangingChanged(
                () => new ListItemChangingEventArgs<T>(index, originalValue, newValue), ItemChanging, action,
                () => new ListItemChangedEventArgs<T>(index, originalValue, newValue), ItemChanged);
        }
        private void OnAdd(int index, T item, Action action)
            => OnChangingChanged(
                () => new ListItemAddingEventArgs<T>(index, item), ItemAdding, action,
                () => new ListItemAddedEventArgs<T>(index, item), ItemAdded);
        private bool OnRemove(int index, T item, Func<bool> action)
            => OnChangingChangedResult(
                () => new ListItemRemovingEventArgs<T>(index, item), ItemRemoving, action,
                () => new ListItemRemovedEventArgs<T>(index, item), ItemRemoved);
        private void OnClear(Action action)
            => OnChangingChanged(
                () => new CancelEventArgs(), ItemClearing, action,
                () => new EventArgs(), ItemCleared);
        private void OnMove(T item, int originalIndex, int newIndex, Action action)
            => OnChangingChanged(
                () => new ListItemMovingEventArgs<T>(item, originalIndex, newIndex), ItemMoving, action,
                () => new ListItemMovedEventArgs<T>(item, originalIndex, newIndex), ItemMoved);
        private void OnReverse(int index, int count, Action action)
            => OnChangingChanged(
                () => new ListRangeItemChangingEventArgs(index, count), ItemReversing, action,
                () => new ListRangeItemChangedEventArgs(index, count), ItemReversed);
        private void OnSort(int index, int count, Action action)
            => OnChangingChanged(
                () => new ListRangeItemChangingEventArgs(index, count), ItemSorting, action,
                () => new ListRangeItemChangedEventArgs(index, count), ItemSorted);



        #region Properties
        /// <summary>
        /// Get or sets the element at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set.
        /// </param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- index is equal to or greater than <see cref="EventedList{T}"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public T this[int index]
        {
            get => _list[index];
            set
            {
                if (IsReadOnly) throw EXCEPTION_READ_ONLY;
                OnChange(index, value, () => _list[index] = value);
            }
        }

        /// <summary>
        /// Get the number of elements contained in the <see cref="EventedList{T}"/>
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="EventedList{T}"/>
        /// </returns>
        public int Count => _list.Count;

        /// <summary>
        /// Gets or sets the total number of elements
        /// the internal data structure can hold without resizing.
        /// </summary>
        /// <return>
        /// The number of elements that the <see cref="EventedList{T}"/>
        /// can contain before resizing is required.
        /// </return>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="EventedList{T}.Capacity"/> is set to a value that
        /// is less than <see cref="EventedList{T}.Count"/>.
        /// </exception>
        /// <exception cref="OutOfMemoryException">
        /// There is not enough memory available on the system.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public int Capacity
        {
            get => _list.Capacity;
            set
            {
                if (IsReadOnly) throw EXCEPTION_READ_ONLY;
                _list.Capacity = value;
            }
        }

        /// <summary>
        /// Gets or sets whether <see cref="EventedList{T}"/> is allowed to change.
        /// </summary>
        public bool IsReadOnly { get; set; }
        #endregion


        #region Add Item(s) Functions
        /// <summary>
        /// Adds an <typeparamref name="T"/> to the end of the <see cref="EventedList{T}"/>
        /// </summary>
        /// <param name="item">
        /// The object to be added to the end of the <see cref="EventedList{T}"/>.
        /// The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Add(T item)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnAdd(_list.Count, item, () => _list.Add(item));
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="EventedList{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="EventedList{T}"/>.
        /// The collection itself cannot be <see langword="null"/>,
        /// but it can contain elements that are <see langword="null"/> for reference types.
        /// </param>
        /// <exception cref="ArgumentNullException">collection is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            foreach (var item in collection)
                Add(item);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="EventedList{T}"/>
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="EventedList{T}"/>.
        /// The collection itself cannot be <see langword="null"/>,
        /// but it can contain elements that are <see langword="null"/> for reference types.
        /// </param>
        /// <exception cref="ArgumentNullException">collection is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void AddRange(params T[] collection) => AddRange((IEnumerable<T>)collection);

        /// <summary>
        /// Inserts an element into the <see cref="EventedList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The object to insert. The value can be null for reference types.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- index is greater than <see cref="EventedList{T}.Count"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Insert(int index, T item)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnAdd(index, item, () => _list.Insert(index, item));
        }

        /// <summary>
        /// Inserts an element into the <see cref="EventedList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which item should be inserted.
        /// </param>
        /// <param name="item">
        /// The object to insert.
        /// The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the <see cref="EventedList{T}"/>.
        /// The collection itself cannot be <see langword="null"/>,
        /// but it can contain elements that are <see langword="null"/>,
        /// if type <typeparamref name="T"/> is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// collection is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- index is greater than <see cref="EventedList{T}.Count"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            foreach (var item in collection)
            {
                Insert(index, item);
                index++;
            }
        }
        #endregion


        #region Remove Item(s) Functions
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="EventedList{T}"/>.
        /// The value can be <see langword="null"/> for reference types.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if item is successfully removed;
        /// otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/>
        /// if item was not found in the <see cref="EventedList{T}"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public bool Remove(T item)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            var index = this._list.IndexOf(item);
            return OnRemove(index, item, () => this._list.Remove(item));
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- index is equal to or greater than <see cref="EventedList{T}"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            var item = _list[index];
            OnRemove(index, item, () => { _list.RemoveAt(index); return true; });
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range of elements to remove.
        /// </param>
        /// <param name="count">
        /// The number of elements to remove.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- count is less than <see langword="0"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in the <see cref="EventedList{T}"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void RemoveRange(int index, int count)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            for (int i = index + count - 1; i >= index; i--)
                RemoveAt(i);
        }

        /// <summary>
        /// Remove all elements from the <see cref="EventedList{T}"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Clear()
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnClear(() => _list.Clear());
        }
        #endregion


        #region Move Item Functions
        /// <summary>
        /// Moves the element at the specified index of the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="originalIndex">
        /// The zero-based original index of the element to move.
        /// </param>
        /// <param name="newIndex">
        /// The zero-based new index of the element to move.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if item is successfully moved;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool Move(int originalIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _list.Count) return false;
            if (originalIndex < 0 || originalIndex >= _list.Count) return false;
            var item = _list[originalIndex];
            OnMove(item, originalIndex, newIndex, () =>
            {
                _list.RemoveAt(originalIndex);
                _list.Insert(newIndex, item);
            });
            return true;
        }

        /// <summary>
        /// Moves the element at the specified index of the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="originalIndex">
        /// The zero-based original index of the element to move.
        /// </param>
        /// <param name="newIndex">
        /// The zero-based new index of the element to move.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if item is successfully moved;
        /// otherwise, <see langword="false"/>.
        /// This method also returns <see langword="false"/>
        /// if item was not found in the <see cref="EventedList{T}"/>.
        /// </returns>
        public bool Move(T item, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _list.Count) return false;
            var originalIndex = _list.IndexOf(item);
            if (originalIndex < 0) return false;
            OnMove(item, originalIndex, newIndex, () =>
            {
                _list.RemoveAt(originalIndex);
                _list.Insert(newIndex, item);
            });
            return true;
        }
        #endregion


        #region Arrange Item(s) Functions
        /// <summary>
        /// Reverses the order of the elements in the specified range of <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range to reverse.
        /// </param>
        /// <param name="count">
        /// The number of elements in the range to reverse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- count is less than <see langword="0"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Reverse(int index, int count)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnReverse(index, count, () => _list.Reverse(index, count));
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="EventedList{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Reverse()
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnReverse(0, _list.Count, () => _list.Reverse());
        }

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="EventedList{T}"/>
        /// using the specified comparer.
        /// </summary>
        /// <param name="index">
        /// The zero-based starting index of the range to sort.
        /// </param>
        /// <param name="count">
        /// The length of the range to sort.
        /// </param>
        /// <param name="comparer">
        /// The <see cref="IComparer{T}"/> implementation to use when comparing elements,
        /// or <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- count is less than <see langword="0"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not specify a valid range in the <see cref="EventedList{T}"/>.
        /// -or- The implementation of comparer caused an error during the sort. For example,
        /// comparer might not return <see langword="0"/> when comparing an item with itself.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// comparer is null, and the default comparer <see cref="Comparer{T}.Default"/>
        /// cannot find implementation of the <see cref="IComparer{T}"/> generic interface
        /// or the <see cref="IComparer"/> interface for type <typeparamref name="T"/>
        /// or <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnSort(index, count, () => _list.Sort(index, count, comparer));
        }

        /// <summary>
        /// Sorts the elements in the <see cref="EventedList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IComparable{T}"/> implementation to use when comparing elements,
        /// or <see langword="null"/> to use the default comparer <see cref="Comparer{T}.Default"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// comparer is <see langword="null"/>, and the default comparer <see cref="Comparer{T}.Default"/>
        /// cannot find implementation of <see cref="IComparable{T}"/> generic interface
        /// or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>
        /// or <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The implementation of comparer caused an error during the sort. For example,
        /// comparer might not return <see langword="0"/> when comparing an item with itself.
        /// </exception>
        public void Sort(IComparer<T> comparer)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnSort(0, _list.Count, () => _list.Sort(comparer));
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="EventedList{T}"/>
        /// using the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="Comparison{T}"/> to use when comparing elements.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// comparison is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The implementation of comparison caused an error during the sort. For example,
        /// comparison might not return <see langword="0"/> when comparing an item with itself.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Sort(Comparison<T> comparer)
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnSort(0, _list.Count, () => _list.Sort(comparer));
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="EventedList{T}"/>
        /// using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The default comparer <see cref="Comparer{T}.Default"/> cannot find
        /// an implementation of <see cref="IComparable{T}"/> generic interface
        /// or the <see cref="IComparable"/> interface for type <typeparamref name="T"/>
        /// or <see cref="EventedList{T}.IsReadOnly"/> is <see langword="true"/>.
        /// </exception>
        public void Sort()
        {
            if (IsReadOnly) throw EXCEPTION_READ_ONLY;
            OnSort(0, _list.Count, () => _list.Sort());
        }
        #endregion


        #region Convertion Functions
        /// <summary>
        /// Converts the elements in the current <see cref="EventedList{T}"/> to another type,
        /// and returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <param name="converter">A <see cref="Converter{TInput, TOutput}"/> delegate that converts each element from one type to another type.</param>
        /// <returns>A <see cref="EventedList{TOutput}"/> of the target type containing the converted elements from the current <see cref="EventedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">converter is null.</exception>
        public EventedList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null) throw new ArgumentNullException($"{nameof(converter)} is null");

            var toReturn = new EventedList<TOutput>();
            foreach (var item in this)
                toReturn.Add(converter.Invoke(item));
            return toReturn;
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="EventedList{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="EventedList{T}"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- count is less than <see langword="0"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index and count do not denote a valid range of elements in the <see cref="EventedList{T}"/>.
        /// </exception>
        public EventedList<T> GetRange(int index, int count)
        {
            var toReturn = new EventedList<T>();
            for (int i = index; i < index + count; i++)
                toReturn.Add(this[i]);
            return toReturn;
        }

        /// <summary>
        /// Copies the entire <see cref="EventedList{T}"/> to a compatible one-dimentional <see cref="Array"/>, starting at the beginning of the targeted array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied 
        /// from <see cref="EventedList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Copies a range of elements from the <see cref="EventedList{T}"/> to a compatible one-dimensional array,
        /// starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="EventedList{T}"/> at which copying begins.</param>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination
        /// of the elements copied from <see cref="EventedList{T}"/>.
        /// The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than <see langword="0"/>.
        /// -or- arrayIndex is less than <see langword="0"/>.
        /// -or- count is less than <see langword="0"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// index is equal to or greater than the <see cref="EventedList{T}.Count"/> of the source <see cref="EventedList{T}"/>.
        /// -or- The number of elements from index to the end of the source <see cref="EventedList{T}"/> is greater than
        /// the available space from arrayIndex to the end of the destination array.
        /// </exception>
        public void CopyTo(int index, T[] array, int arrayIndex, int count) => _list.CopyTo(index, array, arrayIndex, count);

        /// <summary>
        /// Copies the entire <see cref="EventedList{T}"/> to a compatible one-dimentional <see cref="Array"/>, starting at the beginning of the targeted array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="Array"/> that is the destination of the elements copied 
        /// from <see cref="EventedList{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        public void CopyTo(T[] array) => _list.CopyTo(array);

        /// <summary>
        /// Copies the elements of the <see cref="EventedList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the <see cref="EventedList{T}"/>.
        /// </returns>
        public T[] ToArray() => _list.ToArray();

        /// <summary>
        /// Performs the specified action on each element of the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> delegate to perform on each element of the <see cref="EventedList{T}"/>.</param>
        /// <exception cref="InvalidOperationException">An element in the collection has been modified.</exception>
        public void ForEach(Action<T> action) => _list.ForEach(action);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="EventedList{T}"/>.Enumerator for the <see cref="EventedList{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="EventedList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="EventedList{T}"/>.Enumerator for the <see cref="EventedList{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
        #endregion


        #region Item Position Functions
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first
        /// occurrence within the entire <see cref="EventedList{T}"/>.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="EventedList{T}"/>.
        /// The value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements in the <see cref="EventedList{T}"/>
        /// that starts at index and contains count number of elements, if found; otherwise, <see langword="-1"/>.
        /// </returns>
        public int IndexOf(T item) => this._list.IndexOf(item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the range of elements in the <see cref="EventedList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.List`1. The value can be null for reference types.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.
        /// <see langword="0"/> (zero) is valid in an empty list.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements in the <see cref="EventedList{T}"/>
        /// that starts at index and contains count number of elements, if found; otherwise, <see langword="-1"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the <see cref="EventedList{T}"/>.
        /// </exception>
        public int IndexOf(T item, int index) => _list.IndexOf(item, index);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence
        /// within the range of elements in the <see cref="EventedList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.List`1. The value can be null for reference types.</param>
        /// <param name="index">
        /// The zero-based starting index of the search.
        /// <see langword="0"/> (zero) is valid in an empty list.
        /// </param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements in the <see cref="EventedList{T}"/>
        /// that starts at index and contains count number of elements, if found; otherwise, <see langword="-1"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the <see cref="EventedList{T}"/>.
        /// -or- count is less than <see langword="0"/>.
        /// -or- index and count do not specify a valid section in the <see cref="EventedList{T}"/>.
        /// </exception>
        public int IndexOf(T item, int index, int count) => _list.IndexOf(item, index, count);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="EventedList{T}"/>
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="EventedList{T}"/>.
        /// The value can be null for reference types.
        /// </param>
        /// <returns>The zero-based index of the last occurrence of item within the entire the <see cref="EventedList{T}"/>, if found; otherwise, <see langword="-1"/>.</returns>
        public int LastIndexOf(T item) => _list.LastIndexOf(item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence
        /// within the range of elements in the <see cref="EventedList{T}"/>
        /// that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="EventedList{T}"/>.
        /// The value can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements
        /// in the <see cref="EventedList{T}"/> that extends from the first element
        /// to index, if found; otherwise, <see langword="-1"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the <see cref="EventedList{T}"/>.
        /// </exception>
        public int LastIndexOf(T item, int index) => _list.LastIndexOf(item, index);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last
        /// occurrence within the range of elements in the <see cref="EventedList{T}"/>
        /// that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="EventedList{T}"/>.
        /// The value can be null for reference types.
        /// </param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements
        /// in the <see cref="EventedList{T}"/> that contains count number of elements
        /// and ends at index, if found; otherwise, <see langword="-1"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is outside the range of valid indexes for the <see cref="EventedList{T}"/>.
        /// -or- count is less than <see langword="0"/>.
        /// -or- index and count do not specify a valid section
        /// in the <see cref="EventedList{T}"/>.
        /// </exception>
        public int LastIndexOf(T item, int index, int count) => _list.LastIndexOf(item, index, count);
        #endregion


        /// <summary>
        /// Determines whether an element is in the <see cref="EventedList{T}"/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see langword="true"/> if item is found in the <see cref="EventedList{T}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item) => _list.Contains(item);

        public override string ToString() => _list.ToString();


        private readonly List<T> _list = new List<T>();
        private static readonly InvalidOperationException EXCEPTION_READ_ONLY =
            new InvalidOperationException($"Not allowed to change {nameof(EventedList<T>)} when {nameof(IsReadOnly)}=true");
    }
}
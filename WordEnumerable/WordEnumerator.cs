using System.Collections;
using System.Collections.Generic;

namespace WordEnumerable
{
    internal class WordEnumerator : IEnumerator<string>
    {
        private readonly TextParser _textParser;
        private int _currentStartPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordEnumerator"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        internal WordEnumerator(string text = null) => _textParser = new TextParser(text);

        #region Implementation of IEnumerator

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. 
        ///                 </exception><filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            if (_textParser.IsAtEndOfText)
                return false;

            while (!_textParser.IsAtEndOfText && !char.IsLetterOrDigit(_textParser.Peek()))
                _textParser.MoveAhead();

            _currentStartPosition = _textParser.Position;

            while (char.IsLetterOrDigit(_textParser.Peek()))
                _textParser.MoveAhead();

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. 
        ///                 </exception><filterpriority>2</filterpriority>
        public void Reset()
        {
            _textParser.Reset();
            _currentStartPosition = 0;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public string Current => _textParser.Extract(_currentStartPosition, _textParser.Position);

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.
        ///                 </exception><filterpriority>2</filterpriority>
        object IEnumerator.Current => Current;

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() { }

        #endregion
    }
}
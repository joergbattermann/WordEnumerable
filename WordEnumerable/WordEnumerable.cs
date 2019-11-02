using System.Collections;
using System.Collections.Generic;

namespace WordEnumerable
{
    public class WordEnumerable : IEnumerable<string>
    {
        /// <summary>
        /// Stores initial text for GetEnumerator() / enumerator re-creation
        /// </summary>
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordEnumerable"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public WordEnumerable(string text = null)
        {
            _text = text;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            // originally I reused and returned one and the same enumerator here and this was causing the problem.
            // Creating a fresh Enumerator each time this is called fixed it
            return new WordEnumerator(_text);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
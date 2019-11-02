using System;
using System.Linq;
using System.Threading;

namespace WordEnumerable
{
    /// <summary>
    /// TextParser was taken mostly (some minor refactoring took place only) from
    /// <see cref="http://www.blackbeltcoder.com/Articles/strings/a-text-parsing-helper-class">Jonathan Wood's article on blackbeltcoder.com</see>.
    /// All (c) etc in this class are his.
    /// For more information regarding its license <seealso cref="http://www.blackbeltcoder.com/Legal/Licenses/CPOL"/>.
    /// </summary>
    internal class TextParser : IDisposable
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the remaining.
        /// </summary>
        /// <value>The remaining.</value>
        public int Remaining => Text.Length - Position;

        /// <summary>
        /// The Null character constant
        /// </summary>
        private const char NullChar = (char)0;

        /// <summary>
        /// Indicates the state of object disposal
        /// </summary>
        private int _disposableState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextParser"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public TextParser(string text = null) => Reset(text);

        /// <summary>
        /// Resets the current position to the start of the current document
        /// </summary>
        public void Reset() => Position = 0;

        /// <summary>
        /// Sets the current document and resets the current position to the start of it
        /// </summary>
        /// <param name="text"></param>
        public void Reset(string text)
        {
            Text = text ?? string.Empty;
            Position = 0;
        }

        /// <summary>
        /// Indicates if the current position is at the end of the current document
        /// </summary>
        public bool IsAtEndOfText => (Position >= Text.Length);

        /// <summary>
        /// Returns the character at the current position, or a null character if we're
        /// at the end of the document
        /// </summary>
        /// <returns>The character at the current position</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// Returns the character at the specified number of characters beyond the current
        /// position, or a null character if the specified position is at the end of the
        /// document
        /// </summary>
        /// <param name="ahead">The number of characters beyond the current position</param>
        /// <returns>The character at the specified position</returns>
        public char Peek(int ahead)
        {
            var newPosition = (Position + ahead);
            return newPosition < Text.Length ? Text[newPosition] : NullChar;
        }

        /// <summary>
        /// Extracts a substring from the specified position to the end of the text
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public string Extract(int start) => Extract(start, Text.Length);

        /// <summary>
        /// Extracts a substring from the specified range of the current text
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string Extract(int start, int end) => Text.Substring(start, end - start);

        /// <summary>
        /// Moves the current position ahead one character
        /// </summary>
        public void MoveAhead() => MoveAhead(1);

        /// <summary>
        /// Moves the current position ahead the specified number of characters
        /// </summary>
        /// <param name="ahead">The number of characters to move ahead</param>
        public void MoveAhead(int ahead) => Position = Math.Min(Position + ahead, Text.Length);

        /// <summary>
        /// Moves to the next occurrence of the specified string
        /// </summary>
        /// <param name="value">String to find</param>
        /// <param name="ignoreCase">Indicates if case-insensitive comparisons are used</param>
        public void MoveTo(string value, bool ignoreCase = false)
        {
            Position = Text.IndexOf(value, Position, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            
            if (Position < 0)
            {
                Position = Text.Length;
            }
        }

        /// <summary>
        /// Moves to the next occurrence of the specified character
        /// </summary>
        /// <param name="character">Character to find</param>
        public void MoveTo(char character)
        {
            Position = Text.IndexOf(character, Position);

            if (Position < 0)
            {
                Position = Text.Length;
            }
        }

        /// <summary>
        /// Moves to the next occurrence of any one of the specified
        /// characters
        /// </summary>
        /// <param name="chars">Array of characters to find</param>
        public void MoveTo(char[] chars)
        {
            Position = Text.IndexOfAny(chars, Position);
            if (Position < 0)
            {
                Position = Text.Length;
            }
        }

        /// <summary>
        /// Moves to the next occurrence of any character that is not one
        /// of the specified characters
        /// </summary>
        /// <param name="chars">Array of characters to move past</param>
        public void MovePast(char[] chars)
        {
            while (IsInArray(Peek(), chars))
            {
                MoveAhead();
            }
        }

        /// <summary>
        /// Determines if the specified character exists in the specified
        /// character array.
        /// </summary>
        /// <param name="c">Character to find</param>
        /// <param name="chars">Character array to search</param>
        /// <returns></returns>
        protected bool IsInArray(char c, char[] chars) => chars.Any(character => c == character);

        /// <summary>
        /// Moves the current position to the first character that is part of a newline
        /// </summary>
        public void MoveToEndOfLine()
        {
            var character = Peek();
            while (character != '\r' && character != '\n' && !IsAtEndOfText)
            {
                MoveAhead();
                character = Peek();
            }
        }

        /// <summary>
        /// Moves the current position to the next character that is not whitespace
        /// </summary>
        public void MovePastWhitespace()
        {
            while (char.IsWhiteSpace(Peek()))
            {
                MoveAhead();
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // Attempt to move the disposable state from 0 to 1. If successful, we can be assured that
            // this thread is the first thread to do so, and can safely dispose of the object.
            if (Interlocked.CompareExchange(ref _disposableState, 1, 0) != 0)
                return;

            // Call the DisposeResources method with the disposeManagedResources flag set to true, indicating
            // that derived classes may release unmanaged resources and dispose of managed resources.
            Text = null;
            Position = 0;

            // Suppress finalization of this object (remove it from the finalization queue and prevent the destructor from being called).
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

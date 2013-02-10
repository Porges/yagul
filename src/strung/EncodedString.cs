using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Yagul;
using Yagul.Extensions;

namespace Strung
{

    [PublicAPI]
    public class EncodedString<T> : IEquatable<EncodedString<T>>, IReadOnlyList<byte>, IReadOnlyList<UChar> where T : Encoding, new()
    {
        [NotNull] internal byte[] _bytes;

        public EncodedString() : this(Arrays<byte>.Empty)
        { }

        public EncodedString(string other) : this(other.Length == 0 ? Arrays<byte>.Empty : new T().GetBytes(other))
        {
            if (other == null)
                throw new ArgumentNullException("other");
        }
        
        internal EncodedString(byte[] bytes)
        {
            _bytes = bytes;
        }

        /// <summary>
        /// Compares the encoded strings on a byte-by-byte basis.
        /// </summary>
        /// <param name="other">The other string to compare to.</param>
        /// <returns></returns>
        public bool Equals(EncodedString<T> other)
        {
            if (other == null)
                return false;

            var areEqual = _bytes.EqualBytes(other._bytes);

            if (areEqual) // we know they're the same so merge them! strings are readonly, after all
                _bytes = other._bytes;

            return areEqual;
        }

        public static EncodedString<T> Concat(EncodedString<T> left, EncodedString<T> right)
        {
            return new EncodedString<T>(left._bytes.Concat(right._bytes, shortCutAllowed: true));
        }

        public static EncodedString<T> operator +(EncodedString<T> left, EncodedString<T> right)
        {
            return Concat(left, right);
        }

        public static bool operator ==(EncodedString<T> left, EncodedString<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EncodedString<T> left, EncodedString<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EncodedString<T>);
        }

        public static implicit operator EncodedString<T>(string input) 
        {
            return new EncodedString<T>(input);
        }

        public Count<byte> CountBytes { get { return _bytes.Length; } }
        public Count<UChar> CountChars { get { return new T().GetCharCount(_bytes); } } 

        /// <summary>
        /// Returns the byte at the specified index.
        /// 
        /// <remarks>The performance of this operation is always O(1).</remarks>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[Ix<byte> index]
        {
            get { return _bytes[index.Value]; }
        }

        /// <summary>
        /// Returns the character at the specified index.
        /// 
        /// <remarks>The performance of this operation is in general O(index).</remarks>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual UChar this[Ix<UChar> index]
        {
            get { throw new NotImplementedException(); }
        }

        int IReadOnlyCollection<UChar>.Count { get { return CountChars.Value; } }
        int IReadOnlyCollection<byte>.Count { get { return CountBytes.Value; } }

        public IEnumerator<UChar> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
        {
            return ((IEnumerable<byte>)_bytes).GetEnumerator();
        }
        
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return _bytes.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        UChar IReadOnlyList<UChar>.this[int index]
        {
            get { return this[new Ix<UChar>(index)]; }
        }
        
        byte IReadOnlyList<byte>.this[int index]
        {
            get { return this[new Ix<byte>(index)]; }
        }

        public override string ToString()
        {
            return new T().GetString(_bytes);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new AmbiguousMatchException("This method is ambiguous");
        }

        public EncodedString<TNewEncoding> ToEncoding<TNewEncoding>() where TNewEncoding : Encoding, new()
        {
            if (typeof (TNewEncoding) == typeof (T))
                return new EncodedString<TNewEncoding>(_bytes);

            // TODO: is there a better way to do this?
            if (typeof(TNewEncoding) == typeof(UTF8Encoding) && typeof(T) == typeof(ASCIIEncoding))
                return new EncodedString<TNewEncoding>(_bytes);

            return new EncodedString<TNewEncoding>(new TNewEncoding().GetBytes(new T().GetString(_bytes)));
        }
    }
}

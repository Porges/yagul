using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Canasta;
using JetBrains.Annotations;
using Yagul;

namespace Strung
{
    public abstract class EncodedString<T>
        where T: Encoding, new()
    {
        internal EncodedString()
        {
        }

        public static implicit operator EncodedString<T>(string input)
        {
            if (typeof (T) == typeof (UnicodeEncoding))
                return (EncodedString<T>)(object)new UnicodeEncodedString(input);

            return new AnyEncodedString<T>(input);
        }

        public abstract override string ToString();
    }

    public class UnicodeEncodedString : EncodedString<UnicodeEncoding>
    {
        private readonly string _value;

        public UnicodeEncodedString([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            _value = value;
        }

        public static implicit operator UnicodeEncodedString(string other)
        {
            return new UnicodeEncodedString(other);
        }

        public override string ToString()
        {
            return _value;
        }
    }

    [PublicAPI]
    internal class AnyEncodedString<T> : EncodedString<T>, IEquatable<AnyEncodedString<T>>, IReadOnlyList<byte>, IReadOnlyList<UChar> where T : Encoding, new()
    {
        private static readonly T _encoding = new T();

        [NotNull]
        public ExtendablePersistentList<byte> _bytes;

        public AnyEncodedString()
            : this(Arrays<byte>.Empty)
        { }

        public AnyEncodedString(string other)
            : this(other.Length == 0 ? Arrays<byte>.Empty : _encoding.GetBytes(other))
        {
            if (other == null)
                throw new ArgumentNullException("other");
        }

        public AnyEncodedString(ExtendablePersistentList<byte> bytes)
        {
            _bytes = bytes;
        }

        public AnyEncodedString(ICollection<byte> bytes)
            : this(new ExtendablePersistentList<byte>(bytes.Count) + bytes)
        { }

        /// <summary>
        /// Compares the encoded strings on a byte-by-byte basis.
        /// </summary>
        /// <param name="other">The other string to compare to.</param>
        /// <returns></returns>
        public bool Equals(AnyEncodedString<T> other)
        {
            if (other == null)
                return false;
            
            var areEqual = _bytes.EqualBytes(other._bytes);

            if (areEqual) // we know they're the same so merge them! strings are readonly, after all
                other._bytes = _bytes; // we merge left into right as people usually write x == "y"

            return areEqual;
        }

        public static AnyEncodedString<T> Concat(AnyEncodedString<T> left, AnyEncodedString<T> right)
        {
            return new AnyEncodedString<T>(left._bytes + right._bytes);
        }

        public static AnyEncodedString<T> operator +(AnyEncodedString<T> left, AnyEncodedString<T> right)
        {
            return Concat(left, right);
        }

        public static bool operator ==(AnyEncodedString<T> left, AnyEncodedString<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AnyEncodedString<T> left, AnyEncodedString<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnyEncodedString<T>);
        }

        public static implicit operator AnyEncodedString<T>(string input) 
        {
            return new AnyEncodedString<T>(input);
        }

        public Count<byte> CountBytes { get { return _bytes.Count; } }
        public Count<UChar> CountChars { get { return _encoding.GetCharCount(_bytes.ToArray()); /* TODO */ } } 

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
            return _encoding.GetString(_bytes.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new AmbiguousMatchException("This method is ambiguous");
        }

        public AnyEncodedString<TNewEncoding> ToEncoding<TNewEncoding>() where TNewEncoding : Encoding, new()
        {
            if (typeof (TNewEncoding) == typeof (T))
                return new AnyEncodedString<TNewEncoding>(_bytes);

            // TODO: is there a better way to do this?
            if (typeof(TNewEncoding) == typeof(UTF8Encoding) && typeof(T) == typeof(ASCIIEncoding))
                return new AnyEncodedString<TNewEncoding>(_bytes);

            return new AnyEncodedString<TNewEncoding>(new TNewEncoding().GetBytes(_encoding.GetString(_bytes.ToArray())));
        }
    }
}

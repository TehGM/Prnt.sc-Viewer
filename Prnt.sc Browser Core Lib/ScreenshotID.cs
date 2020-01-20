using System;
using System.Linq;

namespace TehGM.PrntScBrowser
{
    public struct ScreenshotID : IEquatable<ScreenshotID>, IComparable<ScreenshotID>
    {
        private const string _charset = "abcdefghijklmnopqrstuvwxyz0123456789";

        public string Value { get; }

        public ScreenshotID(string value)
        {
            this.Value = value.ToLowerInvariant();
        }

        #region String conversion
        public override string ToString()
            => this.Value;

        public static implicit operator string(ScreenshotID id)
            => id.ToString();

        public static explicit operator ScreenshotID(string value)
            => new ScreenshotID(value);
        #endregion

        #region Increment and Decrement
        public static ScreenshotID operator ++(ScreenshotID id)
            => id.Increment();

        public ScreenshotID Increment()
        {
            char[] newValueChars = this.Value.ToCharArray();
            IncrementAtIndex(newValueChars.Length - 1, ref newValueChars);
            return new ScreenshotID(new string(newValueChars));
        }

        private static void IncrementAtIndex(int index, ref char[] value)
        {
            if (value[index] == _charset.Last())
            {
                IncrementAtIndex(index - 1, ref value);
                value[index] = _charset.First();
            }
            else
                value[index] = _charset[_charset.IndexOf(value[index]) + 1];
        }

        public static ScreenshotID operator --(ScreenshotID id)
            => id.Decrement();

        public ScreenshotID Decrement()
        {
            char[] newValueChars = this.Value.ToCharArray();
            DecrementAtIndex(newValueChars.Length - 1, ref newValueChars);
            return new ScreenshotID(new string(newValueChars));
        }

        private static void DecrementAtIndex(int index, ref char[] value)
        {
            if (value[index] == _charset.First())
            {
                DecrementAtIndex(index - 1, ref value);
                value[index] = _charset.Last();
            }
            else
                value[index] = _charset[_charset.IndexOf(value[index]) - 1];
        }
        #endregion

        #region Equality checks
        public override bool Equals(object obj)
            => obj is ScreenshotID iD && Equals(iD);

        public bool Equals(ScreenshotID other)
            => Value == other.Value;

        public override int GetHashCode()
            => this.Value.GetHashCode();

        public static bool operator ==(ScreenshotID left, ScreenshotID right)
            => left.Equals(right);

        public static bool operator !=(ScreenshotID left, ScreenshotID right)
            => !(left == right);
        #endregion

        #region IComparable
        public int CompareTo(ScreenshotID other)
        {
            if (this.Value.Length > other.Value.Length)
                return 1;
            if (this.Value.Length < other.Value.Length)
                return -1;
            if (this.Equals(other))
                return 0;

            for (int i = 0; i < this.Value.Length; i++)
            {
                int charComparison = this.Value[i].CompareTo(other.Value[i]);
                if (charComparison != 0)
                    return charComparison;
            }

            throw new ArithmeticException($"Failed to compare '{this.Value}' to '{other.Value}'");
        }
        #endregion
    }
}

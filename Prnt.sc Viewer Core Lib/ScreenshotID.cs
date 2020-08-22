using System;
using System.Linq;
using System.Text;

namespace TehGM.PrntScViewer
{
    public struct ScreenshotID : IEquatable<ScreenshotID>, IComparable<ScreenshotID>
    {
        private const string _charset = "0123456789abcdefghijklmnopqrstuvwxyz";

        public int NumericalValue { get; }
        private readonly string _stringValue;

        public ScreenshotID(string stringValue)
        {
            this._stringValue = stringValue.ToLowerInvariant();
            this.NumericalValue = ToInt32(stringValue);
        }

        #region Conversion
        public override string ToString()
            => this._stringValue;

        public static implicit operator string(ScreenshotID id)
            => id.ToString();

        public static explicit operator ScreenshotID(string value)
            => new ScreenshotID(value);

        public static explicit operator ScreenshotID(int value)
        {
            StringBuilder result = new StringBuilder();
            while (value > _charset.Length)
            {
                int remainder = value % _charset.Length;
                value /= _charset.Length;
                result.Insert(0, _charset[remainder]);
            }
            if (value > 0)
                result.Insert(0, _charset[value]);
            return new ScreenshotID(result.ToString());
        }

        public static implicit operator int(ScreenshotID id)
            => id.NumericalValue;

        private static int ToInt32(string stringValue)
        {
            int result = 0;
            for (int i = 0; i < stringValue.Length; i++)
            {
                int multiplier = (int)Math.Pow(_charset.Length, stringValue.Length - i - 1);
                int index = _charset.IndexOf(stringValue[i]);
                result += index * multiplier;
            }
            return result;
        }
        #endregion

        #region Increment and Decrement
        public static ScreenshotID operator ++(ScreenshotID id)
            => id.Increment();

        public ScreenshotID Increment()
        {
            char[] newValueChars = _stringValue.ToCharArray();
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
            char[] newValueChars = _stringValue.ToCharArray();
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
            => NumericalValue == other.NumericalValue;

        public override int GetHashCode()
            => NumericalValue.GetHashCode();

        public static bool operator ==(ScreenshotID left, ScreenshotID right)
            => left.Equals(right);

        public static bool operator !=(ScreenshotID left, ScreenshotID right)
            => !(left == right);
        #endregion

        #region IComparable
        public int CompareTo(ScreenshotID other)
        {
            if (_stringValue.Length > other._stringValue.Length)
                return 1;
            if (_stringValue.Length < other._stringValue.Length)
                return -1;
            if (Equals(other))
                return 0;

            for (int i = 0; i < _stringValue.Length; i++)
            {
                int charComparison = _stringValue[i].CompareTo(other._stringValue[i]);
                if (charComparison != 0)
                    return charComparison;
            }

            throw new ArithmeticException($"Failed to compare '{NumericalValue}' to '{other.NumericalValue}'");
        }
        #endregion
    }
}

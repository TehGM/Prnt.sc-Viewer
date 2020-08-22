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

        public ScreenshotID(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value of Screenshot ID is required", nameof(value));
            if (value.Any(c => !_charset.Contains(c)))
                throw new ArgumentException("Screenshot ID contains invalid character(s)", nameof(value));

            this._stringValue = value.ToLowerInvariant();
            this.NumericalValue = ToInt32(value);
        }

        public static bool Validate(string value)
            => !string.IsNullOrWhiteSpace(value) && value.All(c => _charset.Contains(c));

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
                if (index > 0)
                {
                    IncrementAtIndex(index - 1, ref value);
                    value[index] = _charset.First();
                }
                else
                {
                    value[index] = _charset.First();
                    char[] newValue = new char[value.Length + 1];
                    // skip first one, as IDs starting with 0 error out
                    newValue[0] = _charset[1];
                    Array.Copy(value, 0, newValue, 1, value.Length);
                    value = newValue;
                }
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

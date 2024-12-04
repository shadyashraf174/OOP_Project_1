using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MoneyProject {
    public enum CurrencyType {
        USD,
        EUR,
        RUB
    }

    public class Money : IComparable<Money>, IEquatable<Money> {
        public bool IsPositive { get; private set; } = true;
        public uint Dollars { get; private set; }
        public byte Cents { get; private set; }
        public CurrencyType Currency { get; private set; }

        public Money() {
            Random random = new Random();
            var currencies = Enum.GetValues<CurrencyType>();
            Currency = (CurrencyType)currencies.GetValue(random.Next(currencies.Length));
            Dollars = (uint)random.Next(1000);
            Cents = (byte)random.Next(0, 100);
        }

        public Money(bool isPositive, uint dollars, byte cents, CurrencyType currency) {
            IsPositive = isPositive;
            Dollars = dollars;
            Cents = cents;
            Currency = currency;
        }

        public Money(Money other) {
            IsPositive = other.IsPositive;
            Dollars = other.Dollars;
            Cents = other.Cents;
            Currency = other.Currency;
        }

        public Money(string moneyString) {
            ParseMoneyString(moneyString);
        }

        public string GetMoneyString() {
            string signStr = IsPositive == true ? "" : "-";
            return $"{Currency} {signStr}{Dollars}.{Cents:D2}";
        }

        public bool SetSign(bool isPositive) {
            if (isPositive == true || isPositive == false) {
                IsPositive = isPositive;
                return true;
            }
            return false;
        }

        public bool SetDollars(uint dollars) {
            Dollars = dollars;
            return true;
        }

        public bool SetCents(byte cents) {
            if (cents >= 0 && cents < 100) {
                Cents = cents;
                return true;
            }
            return false;
        }

        public void SetCurrency(CurrencyType currency) {
            Currency = currency;
        }

        public bool SetMoneyString(string moneyString) {
            return ParseMoneyString(moneyString);
        }

        public void AddMoney(bool isPositive, uint dollars, byte cents) {
            Money money = new Money(isPositive, dollars, cents, Currency);
            Add(money);
        }

        public void Add(Money other) {
            if (IsPositive == other.IsPositive) {
                uint totalCents = ConvertToCents(Dollars, Cents) +
                    ConvertToCents(other.Dollars, other.Cents);

                Dollars = totalCents / 100;
                Cents = (byte)(totalCents % 100);
            } else {
                Money absThis = new Money(true, Dollars, Cents, Currency);
                Money absOther = new Money(true, other.Dollars, other.Cents, Currency);
                Money result = absThis.CompareTo(absOther) >= 0 ? absThis.Subtract(absOther) : absOther.Subtract(absThis);

                IsPositive = result.IsPositive;
                Dollars = result.Dollars;
                Cents = result.Cents;
            }
        }

        public void SubtractMoney(bool isPositive, uint dollars, byte cents) {
            Money money = new Money(isPositive, dollars, cents, Currency);
            Subtract(money);
        }

        public Money Subtract(Money other) {
            if (IsPositive == other.IsPositive) {
                Money absThis = new Money(true, Dollars, Cents, Currency);
                Money absOther = new Money(true, other.Dollars, other.Cents, Currency);
                Money result = absThis.CompareTo(absOther) >= 0 ? absThis.Subtract(absOther) : absOther.Subtract(absThis);

                return result;
            } else {
                uint totalCents = ConvertToCents(Dollars, Cents) +
                    ConvertToCents(other.Dollars, other.Cents);

                Dollars = totalCents / 100;
                Cents = (byte)(totalCents % 100);
                return this;
            }
        }

        public void ConvertToCurrency(CurrencyType targetCurrency) {
            double conversionRate = GetConversionRate(Currency, targetCurrency);

            Currency = targetCurrency;

            uint totalCents = ConvertToCents(Dollars, Cents);
            totalCents = (uint)(totalCents * conversionRate);

            Dollars = totalCents / 100;
            Cents = (byte)(totalCents % 100);
        }

        public int CompareTo(Money other) {
            if (IsPositive != other.IsPositive) {
                return IsPositive.CompareTo(other.IsPositive);
            }

            if (Currency != other.Currency) {
                throw new ArgumentException("Money between different currency is not able compare");
            }

            if (Dollars != other.Dollars) {
                return Dollars.CompareTo(other.Dollars);
            }

            return Cents.CompareTo(other.Cents);
        }

        public bool Equals(Money other) {
            return IsPositive == other.IsPositive &&
                Dollars == other.Dollars &&
                Cents == other.Cents &&
                Currency == other.Currency;
        }

        private uint ConvertToCents(uint dollars, uint cents) {
            return dollars * 100 + cents;
        }

        private bool ParseMoneyString(string moneyString) {
            string[] parts = moneyString.Split(' ');

            if (parts.Length == 2) {
                string currencyPart = parts[0];
                string valuePart = parts[1];

                if (valuePart.Contains('.') && currencyPart.Length > 0) {
                    string[] valueParts = valuePart.Split('.');
                    if (valueParts.Length == 2) {
                        string sign = valueParts[0].StartsWith("-") ? "-" : "+";
                        string dollars = valueParts[0].TrimStart('+', '-');
                        string cents = valueParts[1];

                        if (uint.TryParse(dollars, out uint dollarsValue) && byte.TryParse(cents, out byte centsValue) &&
                            Enum.TryParse(typeof(CurrencyType), currencyPart, out object currencyType)) {
                            IsPositive = sign == "+";
                            Dollars = dollarsValue;
                            Cents = centsValue;
                            Currency = (CurrencyType)currencyType;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private double GetConversionRate(CurrencyType sourceCurrency, CurrencyType targetCurrency) {
            Dictionary<string, double> conversionRates = new Dictionary<string, double>
    {
        { "EUR-USD", 1.12 },
        { "EUR-RUB", 98.34 },
        { "USD-EUR", 0.98 },
        { "USD-RUB", 95.26 },
        { "RUB-EUR", 0.011 },
        { "RUB-USD", 0.012 },
        { "RUB-RUB", 1 },
        { "USD-USD", 1 },
        { "EUR-EUR", 1 }
    };

            string key = sourceCurrency.ToString() + "-" + targetCurrency.ToString();

            if (conversionRates.TryGetValue(key, out double rate)) {
                return rate;
            }

            throw new ArgumentException("Conversion rate not found for the specified currencies.");
        }

    }

    class Program {
        static void Main(string[] args) {
            Money money1 = new Money();
            Money money2 = new Money(true, 500, 50, CurrencyType.USD);

            Console.WriteLine("");
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("Money1:  " + money1.GetMoneyString());
            Console.WriteLine("Money2: " + money2.GetMoneyString());

            Console.WriteLine("------------------------------------------");

            money1.AddMoney(true, 200, 30);
            money2.SubtractMoney(false, 100, 75);

            Console.WriteLine("Money1 after addition: " + money1.GetMoneyString());
            Console.WriteLine("Money2 after subtraction: " + money2.GetMoneyString());

            Console.WriteLine("------------------------------------------");

            if (money1.Equals(money2)) {
                Console.WriteLine("Money1 is equal to Money2.");
            } else {
                Console.WriteLine("Money1 is not equal to Money2.");
            }

            money1.ConvertToCurrency(CurrencyType.EUR);

            Console.WriteLine("Money1 after convertation: " + money1.GetMoneyString());

            Console.WriteLine("------------------------------------------");

        }

    }
}

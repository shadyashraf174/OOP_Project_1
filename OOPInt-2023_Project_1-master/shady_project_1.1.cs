using System;

namespace MoneyProject
{
    public enum SortOfCurrency
    {
        USD,EUR,RUB, EGP
    }

    public class Money : IComparable<Money>, IEquatable<Money>
    {



        private SortOfCurrency SearchMoney;

        private bool isPositive = true;
        private uint dollars;
        private ushort cents;
        private SortOfCurrency currency;

        public bool IsPositive
        {
            get { return isPositive; }
            private set { isPositive = value; }
        }

        public uint Dollars
        {
            get { return dollars; }
            private set { dollars = value; }
        }

        public ushort Cents
        {
            get { return cents; }
            private set { cents = value; }
        }

        public SortOfCurrency Currency
        {
            get { return currency; }
            private set { currency = value; }
        }

        public Money()
        {
            Random random = new Random();
            var currencies = Enum.GetValues<SortOfCurrency>();
            Currency = (SortOfCurrency)currencies.GetValue(random.Next(currencies.Length));
            Dollars = (uint)random.Next(1000);
            Cents = (ushort)random.Next(0, 100);
        }

        public Money(bool isPositive, uint dollars, ushort cents, SortOfCurrency currency)
        {
            if (cents > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(cents), "cents value can be between 0-99");
            }
            IsPositive = isPositive;
            Dollars = dollars;
            Cents = cents;
            Currency = currency;
        }

        public Money(Money other)
        {
            CopyIsPositive(other);
            CopyDollars(other);
            CopyCents(other);
            CopyCurrency(other);
        }

        public bool Equals(Money other)
        {
            if (other == null)
                return false;

            return IsEqualTo(other);
        }


        private void CopyIsPositive(Money other)
        {
            IsPositive = other.IsPositive;
        }

        private void CopyDollars(Money other)
        {
            Dollars = other.Dollars;
        }

        private void CopyCents(Money other)
        {
            Cents = other.Cents;
        }

        private void CopyCurrency(Money other)
        {
            Currency = other.Currency;
        }

        public Money(string moneyString)
        {
            StringConvert(moneyString);
        }

        public Money(uint dollars, ushort cents, SortOfCurrency SearchMoney)
        {
            Dollars = dollars;
            Cents = cents;
            this.SearchMoney = SearchMoney;
        }

        public string ToString()
        {
            var sign = IsPositive ? "" : "-";
            var formattedDollarsAndCents = String.Format("{0}.{1:F2}", Dollars, Cents);
            return string.Join(" ", Currency, sign + formattedDollarsAndCents).Trim();
        }

        public void SetIsPositive(bool isPositive)
        {
            IsPositive = isPositive;
        }

        public class Wallet
        {
            private uint Dollars;

            public bool SetDollars(uint dollars)
            {
                Dollars = dollars;
                return true;
            }
        }

        public bool SetCentsValue(ushort cents)
        {
            if (cents >= 0 && cents <= 99)
            {
                Cents = cents;
                return true;
            }
            return false;
        }

        public void SetSortOfCurrency(SortOfCurrency currency)
        {
            Currency = currency;
        }

        public bool StringConvert(string moneyString)
        {
            return AttemtStringConvert(moneyString);
        }

        public void AdditionFunctionDetails(bool isPositive, uint dollars, ushort cents)
        {
            Money money = new Money(isPositive, dollars, cents, Currency);
            AdditionFunction(money);
        }

        public void AdditionFunction(Money other)
        {
            uint totalCents;

            if (this.IsPositive == other.IsPositive)
            {
                totalCents = this.Dollars * 100 + this.Cents + other.Dollars * 100 + other.Cents;
            }
            else
            {
                Money absThis = new Money(true, this.Dollars, this.Cents, this.Currency);
                Money absOther = new Money(true, other.Dollars, other.Cents, other.Currency);

                var result = absThis.CompareTo(absOther) >= 0 ? absThis.SubtractFunction(absOther) : absOther.SubtractFunction(absThis);

                this.IsPositive = result.IsPositive;
                totalCents = result.Dollars * 100 + result.Cents;
            }

            this.Dollars = totalCents / 100;
            this.Cents = (ushort)(totalCents % 100);
        }

        public void SubtractFunctionDetails(bool isPositive, uint dollars, ushort cents)
        {
            Money money = new Money(isPositive, dollars, cents, Currency);
            SubtractFunction(money);
        }

        public Money SubtractFunction(Money other)
        {
            if (this.Currency != other.Currency)
            {
                throw new ArgumentException("Cannot subtract money of different currencies.");
            }

            long thisTotalCents = (this.Dollars * 100 + this.Cents) * (this.IsPositive ? 1 : -1);
            long otherTotalCents = (other.Dollars * 100 + other.Cents) * (other.IsPositive ? 1 : -1);

            long resultTotalCents = thisTotalCents - otherTotalCents;

            bool resultIsPositive = resultTotalCents >= 0;
            if (!resultIsPositive)
            {
                resultTotalCents = -resultTotalCents;
            }

            uint resultDollars = (uint)(resultTotalCents / 100);
            ushort resultCents = (ushort)(resultTotalCents % 100);

            return new Money(resultIsPositive, resultDollars, resultCents, this.Currency);
        }

        public Money ConvertToSortOfCurrency(SortOfCurrency SearchMoney)
        {
            double conversionRate = GetConversionRate(Currency, SearchMoney);

            long totalCents = this.Dollars * 100 + this.Cents;
            uint convertedCents = (uint)(totalCents * conversionRate);

            uint dollars = convertedCents / 100;
            ushort cents = (ushort)(convertedCents % 100);

            return new Money(dollars, cents, SearchMoney);
        }

        public int CompareTo(Money other)
        {
            if (this.Currency != other.Currency)
            {
                throw new ArgumentException("Money between different currencies cannot be compared");
            }

            if (this.IsPositive && !other.IsPositive)
            {
                return 1;
            }

            if (!this.IsPositive && other.IsPositive)
            {
                return -1;
            }

            int dollarComparison = this.Dollars.CompareTo(other.Dollars);
            if (dollarComparison != 0)
            {
                return dollarComparison;
            }

            return this.Cents.CompareTo(other.Cents);
        }

        public bool IsEqualTo(Money other)
        {
            return IsPositive == other.IsPositive &&
                Dollars == other.Dollars &&
                Cents == other.Cents &&
                Currency == other.Currency;
        }

        private bool AttemtStringConvert(string moneyString)
        {
            string[] parts = moneyString.Split(' ');

            if (parts.Length == 2)
            {
                string currencyPart = parts[0];
                string valuePart = parts[1];

                if (valuePart.Contains('.') && currencyPart.Length > 0)
                {
                    string[] valueParts = valuePart.Split('.');
                    if (valueParts.Length == 2)
                    {
                        string sign = valueParts[0].StartsWith("-") ? "-" : "+";
                        string dollars = valueParts[0].TrimStart('+', '-');
                        string cents = valueParts[1];

                        if (uint.TryParse(dollars, out uint dollarsValue) && ushort.TryParse(cents, out ushort centsValue) &&
                            Enum.TryParse(typeof(SortOfCurrency), currencyPart, out object SortOfCurrency))
                        {
                            IsPositive = sign == "+";
                            Dollars = dollarsValue;
                            Cents = centsValue;
                            Currency = (SortOfCurrency)SortOfCurrency;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private double GetConversionRate(SortOfCurrency sourceCurrency, SortOfCurrency SearchMoney)
        {
            Dictionary<string, double> conversionRates = new Dictionary<string, double>
            {
                { "EUR-USD", 1.12 },
                { "EUR-RUB", 98.34 },
                { "USD-EUR", 0.98 },
                { "USD-RUB", 95.26 },
                { "RUB-EUR", 0.011 },
                { "RUB-USD", 0.012 },
                { "USD-EGP", 15.69 },
                { "EUR-EGP", 17.48 },
                { "RUB-EGP", 0.21 },
                { "EGP-USD", 0.063},
                { "EGP-EUR", 0.057},
                { "EGP-RUB", 4.77 },
                { "RUB-RUB", 1 },
                { "USD-USD", 1 },
                { "EUR-EUR", 1 },   
                { "EGP-EGP", 1 }      
        };

            string key = sourceCurrency.ToString() + "-" + SearchMoney.ToString();

            if (conversionRates.TryGetValue(key, out double rate))
            {
                return rate;
            }

            throw new ArgumentException("Conversion rate not found for the specified currencies.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Money money1 = new Money(true, 100, 0, SortOfCurrency.USD);
            Money money2 = new Money(true, 1000, 0, SortOfCurrency.EGP);

            Console.WriteLine("Money1: " + money1.ToString());
            Console.WriteLine("Money2: " + money2.ToString());

            Console.WriteLine("");
            money1.AdditionFunctionDetails(true, 200, 20);
            money2.SubtractFunctionDetails(true, 100, 50);

            Console.WriteLine("Money1 after addition: " + money1.ToString());
            Console.WriteLine("Money2 after subtraction: " + money2.ToString());

            Console.WriteLine("");
            Money money3 = money1.ConvertToSortOfCurrency(SortOfCurrency.EGP);

            Console.WriteLine("Money1 after conversion to EGP: " + money3.ToString());

            Money money4 = money2.ConvertToSortOfCurrency(SortOfCurrency.USD);

            Console.WriteLine("Money2 after conversion to USD: " + money4.ToString());
        }
    }
}

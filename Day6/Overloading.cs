using System;

public struct Fraction
{
    public int Numerator;
    public int Denominator;

    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        int newDenominator = a.Denominator * b.Denominator;

        return new Fraction(newNumerator, newDenominator);
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Numerator;
        int newDenominator = a.Denominator * b.Denominator;

        return new Fraction(newNumerator, newDenominator);
    }

    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }
}
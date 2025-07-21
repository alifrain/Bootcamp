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


    // Dioverload dengan operator +   // Contoh: Fraction a = new Fraction(1, 2);
    //         Fraction b = new Fraction(1, 3);
    //         Fraction c = a + b; // Hasilnya adalah Fraction(5, 6);
    // Hasilnya adalah penjumlahan dari kedua pecahan tersebut.

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        int newDenominator = a.Denominator * b.Denominator;

        return new Fraction(newNumerator, newDenominator);
    }

    // Overload dengan operator *  // Contoh: Fraction a = new Fraction(1, 2);  
    //         Fraction b = new Fraction(1, 3);
    //         Fraction c = a * b; // Hasilnya adalah Fraction(1, 6);
    // Hasilnya adalah perkalian dari kedua pecahan tersebut.

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
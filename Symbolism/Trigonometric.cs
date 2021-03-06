﻿using System;
using System.Collections.Generic;
using System.Linq;
using Symbolism.Denominator;
using Symbolism.Numerator;
using static Symbolism.ListConstructor;

using static Symbolism.Trigonometric.Constructors;

namespace Symbolism.Trigonometric
{
  public class Sin : Function
  {
    public static MathObject Mod(MathObject x, MathObject y)
    {
      if (!(x is Number) || !(y is Number)) throw new Exception();
      int result = Convert.ToInt32(Math.Floor(((Number)(x / y)).ToDouble().val));
      return x - y * result;
    }
    MathObject SinProc(params MathObject[] ls)
    {
      Symbol Pi = new Symbol("Pi");
      MathObject half = new Integer(1) / 2;
      MathObject u = ls[0];
      if (u == 0) return 0;
      if (u == Pi) return 0;
      if (u is DoubleFloat)
        return new DoubleFloat(Math.Sin(((DoubleFloat)u).val));
      if (u is Number && u < 0) return -sin(-u);
      if ((u as Product)?.elts[0] is Number && (u as Product).elts[0] < 0)
        return -sin(-u);
      MathObject n;
      if (u is Product &&
          ((u as Product).elts[0] is Integer || (u as Product).elts[0] is Fraction) &&
          (u as Product).elts[0] > half &&
          (u as Product).elts[1] == Pi)
      {
        n = (u as Product).elts[0];
        if (n > 2) return sin(Mod(n, 2) * Pi);
        if (n > 1) return -sin(n * Pi - Pi);
        return n > half ? sin((1 - n) * Pi) : new Sin(n * Pi);
      }
      // sin(k/n pi)
      // n is one of 1 2 3 4 6
      if (u is Product &&
          List<MathObject>(1, 2, 3, 4, 6).Any(elt =>
              elt == ((Product) u).elts[0].Denominator()) &&
          (u as Product).elts[0].Numerator() is Integer &&
          (u as Product).elts[1] == Pi)
      {
        MathObject k = (u as Product).elts[0].Numerator();
        n = (u as Product).elts[0].Denominator();
        if (n == 1) return 0;
        if (n == 2)
        {
          if (Mod(k, 4) == 1) return 1;
          if (Mod(k, 4) == 3) return -1;
        }
        if (n == 3)
        {
          if (Mod(k, 6) == 1) return (3 ^ half) / 2;
          if (Mod(k, 6) == 2) return (3 ^ half) / 2;
          if (Mod(k, 6) == 4) return -(3 ^ half) / 2;
          if (Mod(k, 6) == 5) return -(3 ^ half) / 2;
        }
        if (n == 4)
        {
          if (Mod(k, 8) == 1) return 1 / (2 ^ half);
          if (Mod(k, 8) == 3) return 1 / (2 ^ half);
          if (Mod(k, 8) == 5) return -1 / (2 ^ half);
          if (Mod(k, 8) == 7) return -1 / (2 ^ half);
        }
        if (n == 6)
        {
          if (Mod(k, 12) == 1) return half;
          if (Mod(k, 12) == 5) return half;
          if (Mod(k, 12) == 7) return -half;
          if (Mod(k, 12) == 11) return -half;
        }
      }
      // sin(Pi + x + y + ...)   ->   -sin(x + y + ...)
      if (u is Sum && (u as Sum).elts.Any(elt => elt == Pi))
        return -sin(u - Pi);
      // sin(x + n pi)
      Func<MathObject, bool> Product_n_Pi = elt =>
              (elt is Product) &&
              (
                  ((Product) elt).elts[0] is Integer ||
                  ((Product) elt).elts[0] is Fraction
              ) &&
              Math.Abs(((Number) ((Product) elt).elts[0]).ToDouble().val) >= 2.0 &&
              ((Product) elt).elts[1] == Pi;
      if (u is Sum && (u as Sum).elts.Any(Product_n_Pi))
      {
        MathObject pi_elt = (u as Sum).elts.First(Product_n_Pi);
        n = ((Product) pi_elt).elts[0];
        return sin((u - pi_elt) + Mod(n, 2) * Pi);
      }
      // sin(a + b + ... + n/2 * Pi)
      Func<MathObject, bool> Product_n_div_2_Pi = elt =>
          elt is Product &&
          (
              ((Product) elt).elts[0] is Integer ||
              ((Product) elt).elts[0] is Fraction
          ) &&
          ((Product) elt).elts[0].Denominator() == 2 &&
          ((Product) elt).elts[1] == Pi;
      if (!(u is Sum) || !(u as Sum).elts.Any(Product_n_div_2_Pi)) return new Sin(u);
      MathObject n_div_2_Pi = (u as Sum).elts.First(Product_n_div_2_Pi);
      MathObject other_elts = u - n_div_2_Pi;
      n = ((Product) n_div_2_Pi).elts[0].Numerator();
      if (Mod(n, 4) == 1) return new Cos(other_elts);
      if (Mod(n, 4) == 3) return -new Cos(other_elts);
      return new Sin(u);
    }
    public Sin(MathObject param)
    {
      name = "sin";
      args = new List<MathObject> { param };
      proc = SinProc;
    }
  }
  public class Cos : Function
  {
    public static MathObject Mod(MathObject x, MathObject y)
    {
      if (!(x is Number) || !(y is Number)) throw new Exception();
      int result = Convert.ToInt32(Math.Floor(((Number) (x / y)).ToDouble().val));
      return x - y * result;
    }
    MathObject CosProc(params MathObject[] ls)
    {
      Symbol Pi = new Symbol("Pi");
      MathObject half = new Integer(1) / 2;
      MathObject u = ls[0];
      if (ls[0] == 0) return 1;
      if (ls[0] == new Symbol("Pi")) return -1;
      if (ls[0] is DoubleFloat)
        return new DoubleFloat(Math.Cos(((DoubleFloat)ls[0]).val));
      if (ls[0] is Number && ls[0] < 0) return new Cos(-ls[0]);
      if ((ls[0] as Product)?.elts[0] is Number && ((ls[0] as Product).elts[0] as Number) < 0)
        return new Cos(-ls[0]).Simplify();
      // cos(a/b * Pi)
      // a/b > 1/2         
      MathObject n;
      if (ls[0] is Product &&
          (
              (ls[0] as Product).elts[0] is Integer ||
              (ls[0] as Product).elts[0] is Fraction
          ) &&
          ((ls[0] as Product).elts[0] as Number) > new Integer(1) / 2 &&
          (ls[0] as Product).elts[1] == Pi
          )
      {
        n = (ls[0] as Product).elts[0];
        if (n > 2) return cos(Mod(n, 2) * Pi);
        if (n > 1) return -cos(n * Pi - Pi);
        if (n > half) return -cos(Pi - n * Pi);
        return new Cos(n * Pi);
      }
      // cos(k/n Pi)
      // n is one of 1 2 3 4 6
      if (ls[0] is Product &&
          List<MathObject>(1, 2, 3, 4, 6)
              .Any(elt => elt == ((Product) ls[0]).elts[0].Denominator()) &&
          (ls[0] as Product).elts[0].Numerator() is Integer &&
          (ls[0] as Product).elts[1] == Pi
          )
      {
        MathObject k = (ls[0] as Product).elts[0].Numerator();
        n = (ls[0] as Product).elts[0].Denominator();
        if (n == 1)
        {
          if (Mod(k, 2) == 1) return -1;
          if (Mod(k, 2) == 0) return 1;
        }
        if (n == 2)
        {
          if (Mod(k, 2) == 1) return 0;
        }
        if (n == 3)
        {
          if (Mod(k, 6) == 1) return half;
          if (Mod(k, 6) == 5) return half;
          if (Mod(k, 6) == 2) return -half;
          if (Mod(k, 6) == 4) return -half;
        }
        if (n == 4)
        {
          if (Mod(k, 8) == 1) return 1 / (2 ^ half);
          if (Mod(k, 8) == 7) return 1 / (2 ^ half);
          if (Mod(k, 8) == 3) return -1 / (2 ^ half);
          if (Mod(k, 8) == 5) return -1 / (2 ^ half);
        }
        if (n == 6)
        {
          if (Mod(k, 12) == 1) return (3 ^ half) / 2;
          if (Mod(k, 12) == 11) return (3 ^ half) / 2;
          if (Mod(k, 12) == 5) return -(3 ^ half) / 2;
          if (Mod(k, 12) == 7) return -(3 ^ half) / 2;
        }
      }
      // cos(Pi + x + y + ...)   ->   -cos(x + y + ...)
      if (u is Sum && (u as Sum).elts.Any(elt => elt == Pi))
        return -cos(u - Pi);
      // cos(n Pi + x + y)
      // n * Pi where n is Exact && abs(n) >= 2
      Func<MathObject, bool> Product_n_Pi = elt =>
              (elt is Product) &&
              (
                  ((Product) elt).elts[0] is Integer ||
                  ((Product) elt).elts[0] is Fraction
              ) &&
              Math.Abs(((Number) ((Product) elt).elts[0]).ToDouble().val) >= 2.0 &&
              ((Product) elt).elts[1] == Pi;
      if (ls[0] is Sum && (ls[0] as Sum).elts.Any(Product_n_Pi))
      {
        MathObject pi_elt = (ls[0] as Sum).elts.First(Product_n_Pi);
        n = ((Product) pi_elt).elts[0];
        return cos((ls[0] - pi_elt) + Mod(n, 2) * Pi);
      }
      Func<MathObject, bool> Product_n_div_2_Pi = elt =>
          elt is Product &&
          (
              ((Product) elt).elts[0] is Integer ||
              ((Product) elt).elts[0] is Fraction
          ) &&
          ((Product) elt).elts[0].Denominator() == 2 &&
          ((Product) elt).elts[1] == Pi;
      // cos(a + b + ... + n/2 * Pi) -> sin(a + b + ...)
      if (!(ls[0] is Sum) || !(ls[0] as Sum).elts.Any(Product_n_div_2_Pi)) return new Cos(ls[0]);
      MathObject n_div_2_Pi = (ls[0] as Sum).elts.First(Product_n_div_2_Pi);
      MathObject other_elts = ls[0] - n_div_2_Pi;
      n = ((Product) n_div_2_Pi).elts[0].Numerator();
      if (Mod(n, 4) == 1) return -new Sin(other_elts);
      if (Mod(n, 4) == 3) return new Sin(other_elts);
      return new Cos(ls[0]);
    }
    public Cos(MathObject param)
    {
      name = "cos";
      args = new List<MathObject> { param };
      proc = CosProc;
    }
  }
  public class Tan : Function
  {
    MathObject TanProc(params MathObject[] ls)
    {
      if (ls[0] is DoubleFloat)
        return new DoubleFloat(Math.Tan(((DoubleFloat)ls[0]).val));
      return new Tan(ls[0]);
    }
    public Tan(MathObject param)
    {
      name = "tan";
      args = new List<MathObject> { param };
      proc = TanProc;
    }
  }
  public class Asin : Function
  {
    MathObject AsinProc(params MathObject[] ls)
    {
      if (ls[0] is DoubleFloat)
        return new DoubleFloat(Math.Asin(((DoubleFloat)ls[0]).val));
      return new Asin(ls[0]);
    }
    public Asin(MathObject param)
    {
      name = "asin";
      args = new List<MathObject> { param };
      proc = AsinProc;
    }
  }
  public class Atan : Function
  {
    MathObject AtanProc(params MathObject[] ls)
    {
      if (ls[0] is DoubleFloat)
        return new DoubleFloat(Math.Atan(((DoubleFloat)ls[0]).val));
      return new Atan(ls[0]);
    }
    public Atan(MathObject param)
    {
      name = "atan";
      args = new List<MathObject> { param };
      proc = AtanProc;
    }
  }
  public class Atan2 : Function
  {
    MathObject Atan2Proc(params MathObject[] ls)
    {
      //if (
      //    (ls[0] is DoubleFloat || ls[0] is Integer)
      //    &&
      //    (ls[1] is DoubleFloat || ls[1] is Integer)
      //    )
      //    return new DoubleFloat(
      //        Math.Atan2(
      //            (ls[0] as Number).ToDouble().val,
      //            (ls[1] as Number).ToDouble().val));
      if (ls[0] is DoubleFloat && ls[1] is DoubleFloat)
        return new DoubleFloat(
            Math.Atan2(
                ((DoubleFloat)ls[0]).val,
                ((DoubleFloat)ls[1]).val));
      if (ls[0] is Integer && ls[1] is DoubleFloat)
        return new DoubleFloat(
            Math.Atan2(
                ((Integer)ls[0]).val,
                ((DoubleFloat)ls[1]).val));
      if (ls[0] is DoubleFloat && ls[1] is Integer)
        return new DoubleFloat(
            Math.Atan2(
                ((DoubleFloat)ls[0]).val,
                ((Integer)ls[1]).val));
      if (ls[0] is Integer && ls[1] is Integer)
        return new DoubleFloat(
            Math.Atan2(
                ((Integer)ls[0]).val,
                ((Integer)ls[1]).val));
      return new Atan2(ls[0], ls[1]);
    }
    public Atan2(MathObject a, MathObject b)
    {
      name = "atan2";
      args = new List<MathObject> { a, b };
      proc = Atan2Proc;
    }
  }
  public static class Constructors
  {
    public static MathObject sin(MathObject obj) => new Sin(obj).Simplify();
    public static MathObject cos(MathObject obj) => new Cos(obj).Simplify();
    public static MathObject tan(MathObject obj) => new Tan(obj).Simplify();
    public static MathObject asin(MathObject obj) => new Asin(obj).Simplify();
    public static MathObject atan(MathObject obj) => new Atan(obj).Simplify();
  }
  public static class Extensions
  {
    public static Symbol Pi = new Symbol("Pi");
    public static MathObject ToRadians(this MathObject n) { return n * Pi / 180; }
    public static MathObject ToDegrees(this MathObject n) { return 180 * n / Pi; }
    public static MathObject ToRadians(this int n) { return new Integer(n) * Pi / 180; }
    public static MathObject ToDegrees(this int n) { return 180 * new Integer(n) / Pi; }
  }
}

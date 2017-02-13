using System.Linq;
using Symbolism.Denominator;
using Symbolism.Numerator;

namespace Symbolism
{
  namespace RationalizeExpression
  {
    public static class Extensions
    {
      static MathObject RationalizeSum(MathObject u, MathObject v)
      {
        MathObject m = u.Numerator();
        MathObject r = u.Denominator();
        MathObject n = v.Numerator();
        MathObject s = v.Denominator();
        if (r == 1 && s == 1) return u + v;
        return RationalizeSum(m * s, n * r) / (r * s);
      }
      public static MathObject RationalizeExpression(this MathObject u)
      {
        if (u is Equation)
          return new Equation(
              ((Equation) u).a.RationalizeExpression(),
              ((Equation) u).b.RationalizeExpression(),
              ((Equation) u).Operator);
        if (u is Power)
          return ((Power) u).bas.RationalizeExpression() ^ ((Power) u).exp;
        if (u is Product)
          return
              new Product
              {
                elts = ((Product) u).elts.Select(elt => elt.RationalizeExpression()).ToList()
              }.Simplify();

        if (!(u is Sum)) return u;
        MathObject f = ((Sum) u).elts[0];
        MathObject g = f.RationalizeExpression();
        MathObject r = (u - f).RationalizeExpression();
        return RationalizeSum(g, r);
      }
    }
  }
}

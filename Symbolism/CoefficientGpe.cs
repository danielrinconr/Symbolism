using System;
using System.Linq;
using Symbolism.Has;

namespace Symbolism
{
  namespace CoefficientGpe
  {
    public static class Extensions
    {
      public static Tuple<MathObject, int> CoefficientMonomialGpe(this MathObject u, MathObject x)
      {
        if (u == x) return Tuple.Create((MathObject)1, 1);
        if (u is Power &&
            ((Power) u).bas == x &&
            ((Power) u).exp is Integer &&
            ((Integer) ((Power) u).exp).val > 1)
          return Tuple.Create((MathObject)1, ((Integer) ((Power) u).exp).val);

        if (!(u is Product)) return u.FreeOf(x) ? Tuple.Create(u, 0) : null;
        int m = 0;
        MathObject c = u;
        foreach (Tuple<MathObject, int> f in ((Product) u).elts.Select(elt => elt.CoefficientMonomialGpe(x)))
        {
          if (f == null) return null;

          if (f.Item2 == 0) continue;
          m = f.Item2;
          c = u / (x ^ m);
        }
        return Tuple.Create(c, m);
      }
      public static MathObject CoefficientGpe(this MathObject u, MathObject x, int j)
      {
        if (!(u is Sum))
        {
          Tuple<MathObject, int> f = u.CoefficientMonomialGpe(x);
          if (f == null) return null;
          return f.Item2 == j ? f.Item1 : 0;
        }
        if (u == x) return j == 1 ? 1 : 0;
        MathObject c = 0;
        foreach (Tuple<MathObject, int> f in ((Sum) u).elts.Select(elt => elt.CoefficientMonomialGpe(x)))
        {
          if (f == null) return null;
          if (f.Item2 == j) c = c + f.Item1;
        }
        return c;
      }
    }
  }
}

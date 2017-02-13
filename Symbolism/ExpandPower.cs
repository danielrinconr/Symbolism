using Symbolism.ExpandProduct;

namespace Symbolism
{
  namespace ExpandPower
  {
    public static class Extensions
    {
      static int Factorial(int n)
      {
        int result = 1;
        for (int i = 1; i <= n; i++)
        {
          result *= i;
        }
        return result;
        // return Enumerable.Range(1, n).Aggregate((acc, elt) => acc * elt);
      }
      public static MathObject ExpandPower(this MathObject u, int n)
      {
        if (!(u is Sum)) return u ^ n;
        MathObject f = ((Sum)u).elts[0];
        MathObject r = u - f;
        MathObject s = 0;
        int k = 0;
        while (true)
        {
          if (k > n) return s;
          int c =
            Factorial(n)
            /
            (Factorial(k) * Factorial(n - k));
          s = s + (c * (f ^ (n - k))).ExpandProduct(r.ExpandPower(k));
          k++;
        }
        /*
                return u ^ n;
        */
      }
    }
  }
}

using System.Linq;

namespace Symbolism
{
  namespace Denominator
  {
    public static class Extensions
    {
      public static MathObject Denominator(this MathObject obj)
      {
        if (obj is Fraction) return ((Fraction)obj).denominator;
        if (obj is Power)
        {
          if (((Power)obj).exp is Integer &&
              ((Integer)((Power)obj).exp).val < 0)
            return (obj ^ -1);
          if (((Power)obj).exp is Fraction &&
              ((Fraction)((Power)obj).exp).ToDouble().val < 0)
            return (obj ^ -1);
          return 1;
        }
        if (obj is Product)
        {
          return
              new Product { elts = ((Product)obj).elts.Select(elt => elt.Denominator()).ToList() }
              .Simplify();
        }
        return 1;
      }
    }
  }
}

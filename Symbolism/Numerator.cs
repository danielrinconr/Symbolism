using System.Linq;

namespace Symbolism
{
  namespace Numerator
  {
    public static class Extensions
    {
      public static MathObject Numerator(this MathObject obj)
      {
        if (obj is Fraction) return ((Fraction)obj).numerator;
        if (obj is Power)
        {
          if (((Power)obj).exp is Integer &&
              ((Integer)((Power)obj).exp).val < 0)
            return 1;
          if (((Power)obj).exp is Fraction &&
              ((Fraction)((Power)obj).exp).ToDouble().val < 0)
            return 1;
          return obj;
        }
        if (obj is Product)
        {
          return
              new Product { elts = ((Product)obj).elts.Select(elt => elt.Numerator()).ToList() }
              .Simplify();
        }
        return obj;
      }
    }
  }


}

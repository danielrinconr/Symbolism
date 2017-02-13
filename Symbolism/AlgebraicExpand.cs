using System.Linq;
using Symbolism.ExpandPower;
using Symbolism.ExpandProduct;

namespace Symbolism
{
  namespace AlgebraicExpand
  {
    public static class Extensions
    {
      public static MathObject AlgebraicExpand(this MathObject u)
      {
        if (u is Equation)
        {
          var eq = (Equation)u;
          return eq.a.AlgebraicExpand() == eq.b.AlgebraicExpand();
        }
        if (u is Sum)
        {
          return new Sum { elts = ((Sum)u).elts.Select(elt => elt.AlgebraicExpand()).ToList() }
          .Simplify();
        }
        if (u is Product)
        {
          var v = ((Product)u).elts[0];
          return v.AlgebraicExpand()
              .ExpandProduct((u / v).AlgebraicExpand());
        }
        if (u is Power)
        {
          var bas = ((Power)u).bas;
          var exp = ((Power)u).exp;
          if (exp is Integer && (exp as Integer).val >= 2)
            return bas.AlgebraicExpand().ExpandPower((exp as Integer).val);
          return u;
        }
        if (u is Function)
        {
          return new Function
          {
            name = ((Function)u).name,
            proc = ((Function)u).proc,
            args = ((Function)u).args.ConvertAll(elt => elt.AlgebraicExpand())
          }.Simplify();
        }
        return u;
      }
    }
  }
}

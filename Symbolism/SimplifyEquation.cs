using System.Linq;

namespace Symbolism.SimplifyEquation
{
  public static class Extensions
  {
    public static MathObject SimplifyEquation(this MathObject expr)
    {
      // 10 * x == 0   ->   x == 0
      // 10 * x != 0   ->   x == 0
      if ((expr as Equation)?.a is Product && ((Product)((Equation)expr).a).elts.Any(elt => elt is Number) && (((Equation)expr).b == 0))
        return new Equation(
            new Product { elts = ((Product)((Equation)expr).a).elts.Where(elt => !(elt is Number)).ToList() }.Simplify(),
            0,
            ((Equation)expr).Operator).Simplify();

      // x ^ 2 == 0   ->   x == 0
      // x ^ 2 != 0   ->   x == 0

      if (expr is Equation &&
          ((Equation)expr).b == 0 && (((Equation)expr).a as Power)?.exp is Integer && ((Integer)((Power)(expr as Equation).a).exp).val > 0)
        return ((Power)((Equation)expr).a).bas == 0;
      if (expr is And) return ((And)expr).Map(elt => elt.SimplifyEquation());
      if (expr is Or) return ((Or)expr).Map(elt => elt.SimplifyEquation());
      return expr;
    }
  }
}

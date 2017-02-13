using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbolism
{
  namespace Substitute
  {
    public static class Extensions
    {
      public static MathObject Substitute(this MathObject obj, MathObject a, MathObject b)
      {
        if (obj == a) return b;

        if (obj is Equation)
        {
          switch (((Equation)obj).Operator)
          {
            case Equation.Operators.Equal:
              return (((Equation)obj).a.Substitute(a, b) == ((Equation)obj).b.Substitute(a, b)).Simplify();
            case Equation.Operators.NotEqual:
              return (((Equation)obj).a.Substitute(a, b) != ((Equation)obj).b.Substitute(a, b)).Simplify();
            case Equation.Operators.LessThan:
              return (((Equation)obj).a.Substitute(a, b) < ((Equation)obj).b.Substitute(a, b)).Simplify();
            case Equation.Operators.GreaterThan:
              return (((Equation)obj).a.Substitute(a, b) > ((Equation)obj).b.Substitute(a, b)).Simplify();
          }
          throw new Exception();
        }
        if (obj is Power) return ((Power)obj).bas.Substitute(a, b) ^ ((Power)obj).exp.Substitute(a, b);
        if (obj is Product)
          return
              new Product { elts = ((Product)obj).elts.ConvertAll(elt => elt.Substitute(a, b)) }
              .Simplify();
        if (obj is Sum)
          return
              new Sum { elts = ((Sum)obj).elts.ConvertAll(elt => elt.Substitute(a, b)) }
              .Simplify();
        if (!(obj is Function)) return obj;
        Function obj_ = ((Function)obj).Clone() as Function;
        obj_.args = ((Function)obj).args.ConvertAll(arg => arg.Substitute(a, b));
        return obj_.Simplify();
      }
      public static MathObject SubstituteEq(this MathObject obj, Equation eq)
      { return obj.Substitute(eq.a, eq.b); }
      public static MathObject SubstituteEqLs(this MathObject obj, List<Equation> eqs)
      { return eqs.Aggregate(obj, (a, eq) => a.SubstituteEq(eq)); }
      public static MathObject Substitute(this MathObject obj, MathObject a, int b)
      { return obj.Substitute(a, new Integer(b)); }
      public static MathObject Substitute(this MathObject obj, MathObject a, double b)
      { return obj.Substitute(a, new DoubleFloat(b)); }
    }
  }
}

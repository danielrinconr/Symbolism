using System;
using System.Linq;

namespace Symbolism
{
  namespace Has
  {
    public static class Extensions
    {
      public static bool Has(this MathObject obj, MathObject a)
      {
        if (obj == a) return true;
        if (obj is Equation) return ((Equation)obj).a.Has(a) || ((Equation)obj).b.Has(a);
        if (obj is Power) return (((Power)obj).bas.Has(a) || ((Power)obj).exp.Has(a));
        if (obj is Product) return ((Product)obj).elts.Any(elt => elt.Has(a));
        if (obj is Sum) return ((Sum)obj).elts.Any(elt => elt.Has(a));
        if (obj is Function) return ((Function)obj).args.Any(elt => elt.Has(a));
        return false;
      }
      public static bool Has(this MathObject obj, Func<MathObject, bool> proc)
      {
        if (proc(obj)) return true;
        if (obj is Equation) return ((Equation)obj).a.Has(proc) || ((Equation)obj).b.Has(proc);
        if (obj is Power) return ((Power)obj).bas.Has(proc) || ((Power)obj).exp.Has(proc);
        if (obj is Product) return ((Product)obj).elts.Any(elt => elt.Has(proc));
        if (obj is Sum) return ((Sum)obj).elts.Any(elt => elt.Has(proc));
        if (obj is Function) return ((Function)obj).args.Any(elt => elt.Has(proc));
        return false;
      }
      public static bool FreeOf(this MathObject obj, MathObject a) => !obj.Has(a);
    }
  }
}

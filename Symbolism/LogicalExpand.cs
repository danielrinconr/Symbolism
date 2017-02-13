using System.Collections.Generic;
using System.Linq;

namespace Symbolism.LogicalExpand
{
  public static class Extensions
  {
    public static MathObject LogicalExpand(this MathObject obj)
    {
      while (true)
      {
        if (obj is Or)
        {
          return ((Or) obj).Map(elt => elt.LogicalExpand());
        }
        if (!(obj is And) || !((And) obj).args.Any(elt => elt is Or) || ((And) obj).args.Count() <= 1) return obj;
        List<MathObject> before = new List<MathObject>();
        Or or = null;
        List<MathObject> after = new List<MathObject>();
        foreach (MathObject elt in ((And) obj).args)
        {
          if (elt is Or && or == null) or = elt as Or;
          else if (or == null) before.Add(elt);
          else after.Add(elt);
        }
        obj = or.Map(or_elt => new And(new And {args = before}.Simplify().LogicalExpand(), or_elt, new And {args = after}.Simplify().LogicalExpand()).Simplify());
      }
    }
  }
}

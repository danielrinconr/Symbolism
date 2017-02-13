using System.Collections.Generic;
using System.Linq;

namespace Symbolism.SimplifyLogical
{
  public static class Extensions
  {
    static bool HasDuplicates(this IEnumerable<MathObject> ls)
    {
      return ls.Any(elt => ls.Count(item => item.Equals(elt)) > 1);
    }
    static IEnumerable<MathObject> RemoveDuplicates(this IEnumerable<MathObject> seq)
    {
      List<MathObject> ls = new List<MathObject>();
      foreach (MathObject elt in seq.Where(elt => ls.Any(item => item.Equals(elt)) == false))
        ls.Add(elt);
      return ls;
    }
    public static MathObject SimplifyLogical(this MathObject expr)
    {
      while (true)
      {
        if (expr is And && ((And) expr).args.HasDuplicates())
          return new And { args = ((And) expr).args.RemoveDuplicates().ToList() };
        if (expr is Or && ((Or) expr).args.HasDuplicates())
        {
          expr = new Or { args = ((Or) expr).args.RemoveDuplicates().ToList() };
          continue;
        }
        if (expr is Or) return ((Or) expr).Map(elt => elt.SimplifyLogical());
        return expr;
      }
    }
  }
}

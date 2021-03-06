﻿using System;
using System.Linq;

namespace Symbolism
{
  namespace DeepSelect
  {
    public static class Extensions
    {
      public static MathObject DeepSelect(this MathObject obj, Func<MathObject, MathObject> proc)
      {
        var result = proc(obj);
        if (result is Power)
        {
          return
              (result as Power).bas.DeepSelect(proc) ^
              (result as Power).exp.DeepSelect(proc);
        }
        if (result is Or)
          return (result as Or).Map(elt => elt.DeepSelect(proc));
        if (result is And)
          return (result as And).Map(elt => elt.DeepSelect(proc));
        if (result is Equation)
          return
              new Equation(
                  (result as Equation).a.DeepSelect(proc),
                  (result as Equation).b.DeepSelect(proc),
                  (result as Equation).Operator);
        if (result is Sum)
          return
              new Sum { elts = (result as Sum).elts.Select(elt => elt.DeepSelect(proc)).ToList() }.Simplify();
        if (result is Product)
          return
              new Product { elts = (result as Product).elts.Select(elt => elt.DeepSelect(proc)).ToList() }.Simplify();
        return result;
      }
    }
  }
}

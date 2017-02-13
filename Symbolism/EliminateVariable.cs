using System;
using System.Collections.Generic;
using System.Linq;
using Symbolism.AlgebraicExpand;
using Symbolism.Has;
using Symbolism.IsolateVariable;
using Symbolism.SimplifyEquation;
using Symbolism.Substitute;

namespace Symbolism.EliminateVariable
{
  public static class Extensions
  {
    public static MathObject CheckVariableEqLs(this List<Equation> eqs, Symbol sym)
    {
      while (true)
      {
        // (a == 10, a == 0)   ->   10 == 0   ->   false
        if (eqs.EliminateVariableEqLs(sym) == false) return false;
        // (1/a != 0  &&  a != 0)   ->   a != 0
        if (!eqs.Any(eq => eq.Operator == Equation.Operators.NotEqual && eq.a == sym && eq.b == 0) ||
            !eqs.Any(eq => eq.Operator == Equation.Operators.NotEqual && eq.a == 1 / sym && eq.b == 0))
          return new And { args = eqs.Select(eq => eq as MathObject).ToList() };
        eqs = eqs.Where(eq => (eq.Operator == Equation.Operators.NotEqual && eq.a == 1 / sym && eq.b == 0) == false).ToList();
      }
    }
    public static MathObject CheckVariable(this MathObject expr, Symbol sym)
    {
      // 1 / x == 0
      // 1 / x^2 == 0
      if (expr is Equation &&
          ((Equation)expr).Operator == Equation.Operators.Equal &&
          ((Equation)expr).b == 0 &&
          ((Equation)expr).a.Has(sym) &&
          ((Equation)expr).SimplifyEquation() is Equation &&
          ((Equation)((Equation)expr).SimplifyEquation()).a is Power &&
          ((Power)((Equation)((Equation)expr).SimplifyEquation()).a).exp is Integer &&
          ((Integer)((Power)((Equation)((Equation)expr).SimplifyEquation()).a).exp).val < 0)
        return false;
      if (expr is And)
      {
        MathObject result = ((And)expr).Map(elt => elt.CheckVariable(sym));

        if (!(result is And)) return result;
        List<Equation> eqs = ((And)expr).args.Select(elt => elt as Equation).ToList();
        return eqs.CheckVariableEqLs(sym);
      }
      if (expr is Or &&
          ((Or)expr).args.All(elt => elt is And))
        return ((Or)expr).Map(elt => elt.CheckVariable(sym));
      return expr;
    }
    // EliminateVarAnd
    // EliminateVarOr
    // EliminateVarLs
    // EliminateVar
    // EliminateVars
    public static MathObject EliminateVariableEqLs(this List<Equation> eqs, Symbol sym)
    {
      if (eqs.Any(elt =>
              elt.Operator == Equation.Operators.Equal &&
              elt.Has(sym) &&
              elt.AlgebraicExpand().Has(sym) &&
              elt.IsolateVariableEq(sym).Has(obj => obj is Equation && ((Equation)obj).a == sym && ((Equation)obj).b.FreeOf(sym))
              ) == false)
        return new And { args = eqs.Select(elt => elt as MathObject).ToList() };
      Equation eq = eqs.First(elt =>
          elt.Operator == Equation.Operators.Equal &&
          elt.Has(sym) &&
          elt.AlgebraicExpand().Has(sym) &&
          elt.IsolateVariableEq(sym).Has(obj => obj is Equation && ((Equation)obj).a == sym && ((Equation)obj).b.FreeOf(sym)));
      IEnumerable<Equation> rest = eqs.Except(new List<Equation> { eq });
      MathObject result = eq.IsolateVariableEq(sym);
      // sym was not isolated
      if (result is Equation &&
          ((result as Equation).a != sym || (result as Equation).b.Has(sym)))
        return new And { args = eqs.Select(elt => elt as MathObject).ToList() };
      if (result is Equation)
      {
        Equation eq_sym = result as Equation;
        return new And { args = rest.Select(elt => elt.Substitute(sym, eq_sym.b)).ToList() }.Simplify();
        // return new And() { args = rest.Select(rest_eq => rest_eq.SubstituteEq(eq_sym)).ToList() };
        // rest.Map(rest_eq => rest_eq.Substitute(eq_sym)
      }
      // Or(
      //     And(eq0, eq1, eq2, ...)
      //     And(eq3, eq4, eq5, ...)
      // )
      if (result is Or && (result as Or).args.All(elt => elt is And))
      {
        (result as Or).args.ForEach(elt => (elt as And).args.AddRange(rest));
        return new Or { args = (result as Or).args.Select(elt => EliminateVariable(elt, sym)).ToList() };
      }

      if (!(result is Or)) throw new Exception();
      Or or = new Or();
      foreach (Equation eq_sym in (result as Or).args.Cast<Equation>())
      {
        or.args.Add(new And { args = rest.Select(rest_eq => rest_eq.Substitute(sym, eq_sym.b)).ToList() }.Simplify());
      }

      return or;
      // (result as Or).Map(eq_sym => new And() { args = rest.Select(rest_eq => rest_eq.SubstituteEq(eq_sym)).ToList() });
      // (result as Or).Map(eq_sym => rest.Map(rest_eq => rest_eq.Substitute(eq_sym))
    }
    public static MathObject EliminateVariable(this MathObject expr, Symbol sym)
    {
      if (expr is And)
      {
        IEnumerable<Equation> eqs = ((And)expr).args.Select(elt => elt as Equation);
        return EliminateVariableEqLs(eqs.ToList(), sym);
      }
      if (expr is Or)
      {
        return new Or { args = ((Or)expr).args.Select(and_expr => and_expr.EliminateVariable(sym)).ToList() };
        // expr.Map(and_expr => and_expr.EliminateVar(sym))
      }
      throw new Exception();
    }
    public static MathObject EliminateVariables(this MathObject expr, params Symbol[] syms)
    { return syms.Aggregate(expr, (result, sym) => result.EliminateVariable(sym)); }
  }
}

namespace Symbolism
{
  namespace ExpandProduct
  {
    public static class Extensions
    {
      public static MathObject ExpandProduct(this MathObject r, MathObject s)
      {
        while (true)
        {
          if (r is Sum)
          {
            MathObject f = ((Sum)r).elts[0];

            return f.ExpandProduct(s) + (r - f).ExpandProduct(s);
          }
          if (s is Sum)
          {
            var r1 = r;
            r = s;
            s = r1;
            continue;
          }
          return r * s;
        }
      }
    }
  }
}

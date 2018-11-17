#### Working with Nulls in C# by Jason Roberts ####

A value type may sometimes need to additionally representa null value.

Nullable types are instances of the ```System.Nullable<T>struct```. A nullable type can represent the correct range of values for its underlying value type, plus an additional null value.

```
public Nullable<int> DaysSinceLastLogin { get; set; }

//shorthand
public int? DaysSinceLastLogin { get; set; }
```

```
//bad
if (player.IsNoob == null)
{
    ...               
}

//good
if (player.IsNoob.HasValue)
{
    ...               
}
```

#### Null Object Pattern ####

```
public interface ISpecialDefence
{
    int CalculateDamageReduction(int totalDamage);
}

 public class DiamondSkinDefence : ISpecialDefence
 {
    public int CalculateDamageReduction(int totalDamage)
    {
        return 1;
    }
}

public class IronBonesDefence : ISpecialDefence
{
   public int CalculateDamageReduction(int totalDamage)
   {
       return 5;
   }
}

 public class NullDefence : ISpecialDefence
 {
    public int CalculateDamageReduction(int totalDamage)
    {
        //no operation /  "do nothing" behavior
        return 0; 
    }
}
```

```
public abstract class SpecialDefence
{
  public abstract int CalculateDamageReduction(int totalDamage);  
  public static SpecialDefence Null { get; } = new NullDefence();
  
  private class NullDefence : SpecialDefence
  {
      public override int CalculateDamageReduction(int totalDamage)
      {
          //no operation /  "do nothing" behavior
          return 0;
      }
   }
}

public class DiamondSkinDefence : SpecialDefence
{
   public override int CalculateDamageReduction(int totalDamage)
   {
       return 1;
   }
}

public class IronBonesDefence : SpecialDefence
{
   public override int CalculateDamageReduction(int totalDamage)
   {
       return 5;
   }
}
```

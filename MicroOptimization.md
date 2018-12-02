#### Boxing/Unboxing ####
```
int i = 123;

//heavy operation
object 0 = i;
```

```
static void NotOptimizedCode()
{
    for(int i = 0; i < 100000000; i++)
    {
        Sub(1);
    }
}

static void OptimizedCode()
{
    object a = new object();
    for(int i = 0; i < 100000000; i++)
    {
        Sub(a);
    }
}
```

#### For Loop Decrement optimization ####

```
for (int i = max - 1;  i >= 0;  i--) 
{
}
```

#### String Interpolation ####

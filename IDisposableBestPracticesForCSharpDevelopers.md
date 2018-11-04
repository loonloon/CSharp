IDisposable Best Practices for C# Developers by Elton Stoneman

__IDisposable__ provides a mechanism for releasing unmanaged resouces.

#### Unmanaged Resouces ####

![img1](https://user-images.githubusercontent.com/5309726/47963359-22cdd180-e066-11e8-8d37-832d07a898c3.png)

#### Best Practice #1 Dispose of IDisposable objects as soon as you can ####
```
using(var state = new DatabaseState())
{
  state.GetDate().Dump();
}
```

```
varstate = new DatabaseState();

try
{
  state.GetDate().Dump();
}
finally
{
  state.Dispose();
}
```

#### What is garbage colllector?####

Manages the allocation and release of memory for your application

#### Best Practice #2 If you use IDisposable objects as instance fields, implement IDisposable ####

```
public class DatabaseState: IDisposable
{
  private SqlConnection _connection;
  
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
  
  protected virtual void Dispose(bool disposing)
  {
    if(disposing)
    {
      if(_connection != null)
      {
        _connection.Dispose();
        _connection= null;
      }
    }
  }
}
```

#### Best Practice #3 Allow Dispose() to be called multiple times and don't throw exceptions ####

```
if(_connection != null)
{
  _connection.Dispose();
  _connection= null;
}
```

#### Best Practice #4 Implement IDisposableto support disposing resources in a class hierarchy ####

```
public class xxx: IDisposable
{
}
```

```
public void Dispose()
{
  Dispose(true);
  GC.SuppressFinalize(this);
}
```

```
protected virtual void Dispose(bool disposing)
{
}
```

#### Best Practice #5 If you use unmanaged resources, declare a finalizer which cleans them up ####

```
~UnmanagedDatabaseState()
{
  //only dispose unmanaged resources
  Dispose(false);
}

protected override void Dispose(bool disposing)
{
  if(disposing)
  {
    if(_command != null)
    {
      _command.Dispose();
      _command = null;
    }
  }
  
  //managed resources...
  if(_unmanagedPointer != IntPtr.Zero)
  {
    Marshal.FreeHGlobal(_unmanagedPointer);
    _unmanagedPointer= IntPtr.Zero;
  }
  
  base.Dispose(disposing);
}
```
#### Best Practice #7 If you implement an interface and use IDisposable fields, extend your interface from IDisposable ####

```
public class BookFeedRepository: IBookFeedRepository
{
  private BookFeedContext_context;
  //...
}

public  interface IBookFeedRepository: IDisposable
{
  //...
}
```

#### Best Practice #8 If you implement IDisposable, don’t implement it explicitly ####

```
public class DifficultToDiscover: IDisposable
{
  //Bad practice
  void IDisposable.Dispose()
  {
  //...
  }
}
```

#### Versioning ####

<table>
    <tbody>
        <tr>
            <th>Version</th>
            <th>Features</th>
        </tr>
        <tr>
            <td>8</td>
            <td>
                <ul>
                    <li>Async Streams</li>
                    <li>Nullable reference type (Compiler warning)</li>
                    <li>Default interface implementation</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>7</td>
            <td>
                <ul>
                    <li>Numeric literals</li>
                    <li>Out variable</li>
                    <li>Pattern variable</li>
                    <li>Local method</li>
                    <li>Expression bodied</li>
                    <li>Deconstructor</li>
                    <li>Tuples</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>6</td>
            <td>
                <ul>
                    <li>Null conditions</li>
                    <li>String interpolation</li>
                    <li>Exception filter</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>

#### Value and Reference Types ####

<table>
    <tbody>
        <tr>
            <th>Value</th>
            <th>Reference</th>
        </tr>
        <tr>
            <td>bool</td>
            <td>class</td>
        </tr>
        <tr>
            <td>byte</td>
            <td>interface</td>
        </tr>
        <tr>
            <td>char</td>
            <td>delegate</td>
        </tr>
        <tr>
            <td>decimal</td>
            <td>object</td>
        </tr>
        <tr>
            <td>double</td>
            <td>string</td>
        </tr>
        <tr>
            <td>enum</td>
            <td></td>
        </tr>
        <tr>
            <td>float</td>
            <td></td>
        </tr>
        <tr>
            <td>int</td>
            <td></td>
        </tr>
        <tr>
            <td>long</td>
            <td></td>
        </tr>
        <tr>
            <td>sbyte</td>
            <td></td>
        </tr>
        <tr>
            <td>short</td>
            <td></td>
        </tr>
        <tr>
            <td>struct</td>
            <td></td>
        </tr>
        <tr>
            <td>uint</td>
            <td></td>
        </tr>
        <tr>
            <td>ulong</td>
            <td></td>
        </tr>
        <tr>
            <td>ushort</td>
            <td></td>   
        </tr>
    </tbody>
</table>

#### Retangular / Multidimentional, Jagged Array ####
![arrays](https://user-images.githubusercontent.com/5309726/52907555-dc138880-329e-11e9-9982-eb0d4fffe6cd.png)

#### What is Stack and Heap? ####
<table>
    <tbody>
        <tr>
            <th>Stack (LIFO)</th>
            <th>Heap</th>
        </tr>
        <tr>
            <td>A block of memory for storing local variables and parameters.</td>
            <td>A block of memory in which objects (reference types) reside.</td>
        </tr>
        <tr>
            <td>The stack logically grows and shrinks as a function is entered and exited.</td>
            <td>Whenever a new object is created, it is allocated on the heap, and a reference type to that object is returned.</td>
        </tr>
    </tbody>
</table>

![stack-headp](https://user-images.githubusercontent.com/5309726/52907578-475d5a80-329f-11e9-862d-886e54ecb2ef.png)

#### What is Boxing and Unboxing? ####

<table>
    <tbody>
        <tr>
            <th>Boxing</th>
            <th>Unboxing</th>
        </tr>
        <tr>
            <td>Is the process of converting a value type to the type object or to any interface type implemented by this value type.</td>
            <td>Unboxing extracts the value type from the object.</td>
        </tr>
        <tr>
            <td>Implicit</td>
            <td>Explicit</td>
        </tr>
    </tbody>
</table>

#### What is Extension Method? ####
Allow an existing type to be extended with new methods without altering the definition of the original type. An extension method is a static method of a static class. 

#### What is Yield? ####
```
public class ContactListStore : IStore<ContactModel>
{
    public IEnumerable<ContactModel> GetEnumerator()
    {
        var contacts = new List<ContactModel>();

        Console.WriteLine("ContactListStore: Creating contact 1");
        contacts.Add(new ContactModel { FirstName = "Bob", LastName = "Blue" });

        Console.WriteLine("ContactListStore: Creating contact 2");
        contacts.Add(new ContactModel { FirstName = "Jim", LastName = "Green" });

        Console.WriteLine("ContactListStore: Creating contact 3");
        contacts.Add(new ContactModel { FirstName = "Susan", LastName = "Orange" });

        return contacts;
    }
}

static void Main(string[] args)
{
    var store = new ContactListStore();
    var contacts = store.GetEnumerator();

    Console.WriteLine("Ready to iterate through the collection.");
    Console.ReadLine();
}

Console Output 
ContactListStore: Creating contact 1 
ContactListStore: Creating contact 2 
ContactListStore: Creating contact 3 
Ready to iterate through the collection.

Note: The entire collection was loaded into memory without even asking for a single item in the list
```

```
public class ContactYieldStore : IStore<ContactModel>
{
    public IEnumerable<ContactModel> GetEnumerator()
    {
        Console.WriteLine("ContactYieldStore: Creating contact 1");
        yield return new ContactModel { FirstName = "Bob", LastName = "Blue" };

        Console.WriteLine("ContactYieldStore: Creating contact 2");
        yield return new ContactModel { FirstName = "Jim", LastName = "Green" };

        Console.WriteLine("ContactYieldStore: Creating contact 3");
        yield return new ContactModel { FirstName = "Susan", LastName = "Orange" };
    }
}

static void Main(string[] args)
{
    var store = new ContactYieldStore();
    var contacts = store.GetEnumerator();

    Console.WriteLine("Ready to iterate through the collection.");
    Console.ReadLine();
}

Console Output 
Ready to iterate through the collection.
Note: The collection wasn't executed at all. This is due to the "deferred execution" nature of IEnumerable. Constructing an item will only occur when it is really required.
Let's call the collection again and obverse the behaviour when we fetch the first contact in the collection.
```

```
static void Main(string[] args)
{
    var store = new ContactYieldStore();
    var contacts = store.GetEnumerator();

    Console.WriteLine("Ready to iterate through the collection");
    Console.WriteLine("Hello {0}", contacts.First().FirstName);
    Console.ReadLine();
}

Console Output 
Ready to iterate through the collection 
ContactYieldStore: Creating contact 1 
Hello Bob

Nice! Only the first contact was constructed when the client "pulled" the item out of the collection.
```

#### Explicit Interface #### 
Implementing multiple interfaces can sometimes result in a collision between member signatures.

````
interface I1 { void Foo(); }
interface I2 { int Foo(); }

public class Widget : I1, I2
{
   public void Foo()
   {
      Console.WriteLine ("Widget's implementation of I1.Foo");
   }

   int I2.Foo()
   {
      Console.WriteLine ("Widget's implementation of I2.Foo");
      return 42;
   }
}

Widget w = new Widget();
w.Foo(); // Widget's implementation of I1.Foo
((I1)w).Foo(); // Widget's implementation of I1.Foo
((I2)w).Foo(); // Widget's implementation of I2.Foo
````

#### What is Managed and Unmanaged Code? ####

<table>
    <tbody>
        <tr>
            <th>Managed</th>
            <th>Unmanaged</th>
        </tr>
        <tr>
            <td>Any language that is written in .NET Framework is managed code and will execute only under the management of a Common Language Runtime virtual machine.</td>
            <td>The code, which is developed outside .NET framework is known as unmanaged code, such as C++.</td>
        </tr>
    </tbody>
</table>

#### What is the difference between a struct and a class? ####

<table>
    <tbody>
        <tr>
            <th>struct</th>
            <th>class</th>
        </tr>
        <tr>
            <td>Is value type in C# and it inherits from System.Value Type.</td>
            <td>Is reference type in C# and it inherits from the System.Object Type.</td>
        </tr>
        <tr>
            <td>Used for smaller amounts of data.</td>
            <td>Usually used for large amounts of data.</td>
        </tr>
        <tr>
            <td>Can’t be inherited to other type.</td>
            <td>Can be inherited to other class.</td>
        </tr>
        <tr>
            <td>Can't be abstract.</td>
            <td>Can be abstract type.</td>
        </tr>
        <tr>
            <td>Do not have permission to create any default constructor.</td>
            <td>Can create a default constructor.</td>
        </tr>
    </tbody>
</table>

#### What is the difference between Interface and Abstract Class? ####

<table>
    <tbody>
        <tr>
            <th>Interface</th>
            <th>Abstract class</th>
        </tr>
        <tr>
            <td>All the methods have to be abstract.</td>
            <td>Can have non-abstract methods.</td>
        </tr>
        <tr>
            <td>-</td>
            <td>Can declare or use any variables.</td>
        </tr>
        <tr>
            <td>-</td>
            <td>Need to use abstract keyword to declare abstract methods.</td>
        </tr>
        <tr>
            <td>Can't define constructor.</td>
            <td>Can use constructor.</td>
        </tr>
        <tr>
            <td>Can be used as multiple inheritance.</td>
            <td>Can’t be used for multiple inheritance.</td>
        </tr>
    </tbody>
</table>

#### What is the difference between constant and readonly? ####

<table>
    <tbody>
        <tr>
            <th>const</th>
            <th>readonly</th>
        </tr>
        <tr>
            <td>Evaluated at compile time.</td>
            <td>Evaluated at run time.</td>
        </tr>
        <tr>
            <td>Cannot be declared as static (implicitly static).</td>
            <td>Can be initialized by code in the constructor.</td>
        </tr>
    </tbody>
</table>

#### What is the difference between ref, out and in keywords? ####

<table>
    <tbody>
        <tr>
            <th>ref</th>
            <th>out</th>
            <th>in</th>
        </tr>
        <tr>
            <td colspan="3">Indicates that an argument is passed by reference</td>
        </tr>
        <tr>
            <td>Requires that the variable be initialized before it is passed.</td>
            <td>-</td>
            <td>Arguments cannot be modified by the called method.</td>
        </tr>
    </tbody>
</table>

```
//ref
void Method(ref int refArgument)
{
    refArgument = refArgument + 44;
}

int number = 1;
Method(ref number);
Console.WriteLine(number); // Output: 45

//out
int initializeInMethod;
OutArgExample(out initializeInMethod);
Console.WriteLine(initializeInMethod); // value is now 44

void OutArgExample(out int number)
{
    number = 44;
}

//in
int readonlyArgument = 44;
InArgExample(readonlyArgument);
Console.WriteLine(readonlyArgument); // value is still 44

void InArgExample(in int number)
{
   error CS8331
   //number = 19;
}
```

#### What is the difference between string and StringBuilder? ####
<table>
    <tbody>
        <tr>
            <th></th>
            <th>string</th>
            <th>StringBuilder</th>
        </tr>
        <tr>
            <td>Object</td>
            <td>Immutable</td>
            <td>Mutable</td>        
        </tr>
        <tr>
            <td>Performance</td>
            <td>Is slow because its’ create a new instance to override or change the previous value.</td>
            <td>Is very fast because it will use same instance to perform any operation like insert value in existing string.</td>
        </tr>
    </tbody>
</table>

#### What is delegates and usages? ####
A delegate can be seen as a placeholder for a/some method(s). By defining a delegate, you are saying to the user of your class, "Please feel free to **assign, any method that matches this signature**, to the delegate and it will be called each time my delegate is called"

#### What is multicast delegate? #### 
* Also called as combined delegate, multiple delegate objects can be assigned to one delegate instance by using the + operator (same type in return or argument). 
* The multicast delegate contains a list of the assigned delegates. When the multicast delegate is called, it invokes the delegates in the list, in order. 
* The - operator can be used to remove a component delegate from a multicast delegate.

#### What are the differences between delegates and events? ####
An Event declaration **adds a layer of abstraction and protection on the delegate instance**. This protection **prevents clients of the delegate from resetting the delegate and its invocation list** and only allows adding or removing targets from the invocation list.

```

```

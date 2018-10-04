#### C# Attributes: Power and Flexibility for Your Code by Jason Roberts ####

#### What are attributes? ####
When we add attribute to the class, we are describe and decorate the class. E.g. We add sticky note or add to the code supply additional metadata.

#### Uses of attributes ####
* Configure conditional code compilation
* Control debugging experience
* Mark code as deprecated
* Add metadata without code comments
* Define data validation rules
* Control serialization/deserialization
* Control runtime rendering/display
* Expose internals
* Mark methods as test methods
* Specify design time data/appearance
* Configure serverless functions bindings

#### Applying Attributes to Code Elements ####
* Assembly
* Classes & structs
* Interfaces & enums
* Methods
* Fields
* Properties
* Constructors
* Delegates
* Events
* Parameters
* Return values

#### Attribute Inheritance ####
- If class 2 inherit from class 1, we will also inherit the atrribute. So both Class 1 and 2 have MyCustomAttribute.
![class](https://user-images.githubusercontent.com/5309726/46453593-ee4fc700-c7d4-11e8-87b4-5a524752c5f8.png)

#### Controlling the Debugging Experience ####
#### `DebuggerDisplay` ####
![debuggerdisplaycode](https://user-images.githubusercontent.com/5309726/46456700-535de980-c7e2-11e8-9615-730b66bd145f.png)

![debuggerdisplay](https://user-images.githubusercontent.com/5309726/46455405-85208180-c7dd-11e8-865b-bfc37396a14e.png)

#### `DebuggerBrowsable` ####
![debuggerbrowsable](https://user-images.githubusercontent.com/5309726/46455491-e0527400-c7dd-11e8-92a2-4b8032cfe4ae.png)

![debuggerbrowsablecode](https://user-images.githubusercontent.com/5309726/46456770-9b7d0c00-c7e2-11e8-81f7-af60b34ecaaf.png)


#### `DebuggerTypeProxy` ####
![debuggertypeproxycode](https://user-images.githubusercontent.com/5309726/46456819-c6fff680-c7e2-11e8-8409-0a52b6c0ad93.png)

![debuggertypeproxy](https://user-images.githubusercontent.com/5309726/46456610-0f6ae480-c7e2-11e8-995e-0b295aebdf42.png)

#### Making code as deprecated #### 
`[Obsolete()]`

#### Conditionally compiling code ####
```
[Conditional("DEBUG")]
private void OutputDebugInfo()
{
  Console.WriteLine("Debug Mode"); 
}
```

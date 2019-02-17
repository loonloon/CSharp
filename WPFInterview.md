#### What is INotifyPropertyChanged used for? ####
For notifying any observer, usually a WPF control, that the property it is bound to has changed.

#### What is the purpose of MVVM? ####
To provide a design pattern that allows for separation of concerns. The UI is independent of the data and the data is independent of the UI.

#### What object do most ViewModels inherit from? ####
Either ObservableObject or ViewModelBase.

#### What are static and dynamic resources? ####
<table>
    <tbody>
        <tr>
            <th>Static Resource</th>
            <th>Dynamic Resource</th>
        </tr>
        <tr>
            <td>Assigned to target once and it will not update the target if the resource dictionary is changed.</td>
            <td>Defer loading resource until it is needed and it will update the target if the resource dictionary is changed.</td>     
        </tr>
    </tbody>
</table>
		
#### How many types of triggers in WPF? ####
<table>
    <tbody>
        <tr>
            <th>Trigger</th>
            <th>Description</th>
        </tr>
        <tr>
            <td>Property</td>
            <td>Watches for a dependency property to have a certain value.</td>     
        </tr>
        <tr>
            <td>Data</td>
            <td>Get active when Binding Data matches specified condition.</td>     
        </tr>
        <tr>
            <td>MultiTrigger</td>
            <td>Watches for a multiple dependency property to have a certain value.</td>     
        </tr>
        <tr>
            <td>MultiDataTrigger</td>
            <td>Get active when multiple Binding Data matches specified condition.</td>     
        </tr>
        <tr>
            <td>Event</td>
            <td>Perform action when RoutedEvent of FrameworkElement raise.</td>     
        </tr>
    </tbody>
</table>	

#### What is Virtualization in WPF? ####
Improves the rendering performance of UI elements.

<table>
    <tbody>
        <tr>
            <th>Virtualization Mode</th>
            <th>Description</th>
        </tr>
        <tr>
            <td>Standard</td>
            <td>VirtualizingStackPanel creates an item container for each visible item and discards it when it is no longer needed (such as when the item is scrolled out of view).</td>     
        </tr>
        <tr>
            <td>Recycling</td>
            <td>Reuses item containers instead of creating a new one each time.</td>     
        </tr>
    </tbody>
</table>
		

#### What are Converters in WPF? ####
When the data is flown from a source to a target, we can change the value or type of object before set on the target property.

<table>
    <tbody>
        <tr>
            <th>Method</th>
            <th>Description</th>
        </tr>
        <tr>
            <td>Convert</td>
            <td>Called when data is flowing from a source to a target.</td>     
        </tr>
        <tr>
            <td>ConvertBack</td>
            <td>Called when data is flowing from a target to a source. It is basically useful in two-way binding scenarios.</td>     
        </tr>
    </tbody>
</table>

#### What is the Control Template in WPF? ####
The ControlTemplate contains the tree of elements that define the desired look. After you define a ControlTemplate you can attach it to any Control or Page by setting it's TemplateProperty.
 
#### What is a Routed event? ####
<table>
    <tbody>
        <tr>
            <th>Event</th>
            <th>Description</th>
            <th></th>
        </tr>
        <tr>
            <td>Bubbling</td>
            <td>Events which are first raised by the control than raised by the other controls in the control hierarchy. It allows Bubble up to the tree till the Root Element.  First Tunneling events are raised than bubbling events raised.</td>     
            <td><img src="https://user-images.githubusercontent.com/5309726/52910688-282def80-32d6-11e9-8e46-99124e2159b6.png" width="400" /></td>
        </tr>
        <tr>
            <td>Tunneling</td>
            <td>Reverse of the Bubbling events. Tunneling Events raised first in the controls hierarchy. These events are raised by the Root elements. This allows events to tunnel down the tree.</td>     
            <td><img src="https://user-images.githubusercontent.com/5309726/52910694-3e3bb000-32d6-11e9-996c-52ccc9af6838.png" width="400" /></td>
        </tr>
        <tr>
            <td>Direct</td>
            <td>Direct Event is generally raised by the control itself. The behavior of this event is same as the .net general event.</td>     
            <td></td>
        </tr>
    </tbody>
</table>

#### What is Attached Properties and how to register it? ####
It is intended to be used as a type of global property that is settable on any object.

```
<DockPanel>
  <CheckBox DockPanel.Dock="Top" IsBubbleSource="true">Hello</CheckBox>
</DockPanel>

public static readonly DependencyProperty IsBubbleSourceProperty = DependencyProperty.RegisterAttached(
  "IsBubbleSource",
  typeof(Boolean),
  typeof(AquariumObject),
  new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender)
);
public static void SetIsBubbleSource(UIElement element, Boolean value)
{
  element.SetValue(IsBubbleSourceProperty, value);
}
public static Boolean GetIsBubbleSource(UIElement element)
{
  return (Boolean)element.GetValue(IsBubbleSourceProperty);
}
```

#### What is Dependency Property? ####
http://www.wpftutorial.net/dependencyproperties.html

#### How Does Data Binding Works? ####
![binding-process](https://user-images.githubusercontent.com/5309726/52910758-1e58bc00-32d7-11e9-8101-074252251fab.png)

* Bindings are the bridge between a view and its view model.
* Allows source objects to raise PropertyChange events to tell Bindings to update the Target property (Dependency Properties).

#### Data Binding Mode ####

<table>
    <tbody>
        <tr>
            <th>Mode</th>
            <th>Description</th>
        </tr>
        <tr>
            <td>Default</td>
            <td>The default value varies for each dependency property. In general, user editable control properties, such as those text boxes and check boxes, default to two-way bindings.</td>     
        </tr>
        <tr>
            <td>OneTime</td>
            <td>Updates the binding target when the application starts or when the data context changes.</td>     
        </tr>
        <tr>
            <td>OneWay</td>
            <td>Updates the binding target (target) property when the binding source (source) changes. This type of binding is appropriate if the control being bound is implicitly read-only.</td>     
        </tr>
        <tr>
            <td>OneWayToSource</td>
            <td>Updates the source property when the target property changes.</td>     
        </tr>
        <tr>
            <td>TwoWay</td>
            <td>Causes changes to either the source property or the target property to automatically update the other.</td>     
        </tr>
    </tbody>
</table>

#### What is DataContext? ####
Every FrameworkElement can be associated with a DataContext which will be used as the default data source during binding, if no other data source is specified in the binding code. Also, the children of this FrameworkElement auotmatically inherit this setting.

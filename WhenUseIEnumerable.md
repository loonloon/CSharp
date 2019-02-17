
![zeichnung-ienumerable-icollection-ilist](https://user-images.githubusercontent.com/5309726/50049554-0459fc00-0123-11e9-8169-1b6d82dc6ed1.png)

<table>
    <tbody>
        <tr>
            <th>Interface</th>
            <th>Scenario</th>
        </tr>
        <tr>
            <td>IEnumerable, IEnumerable<T></td>
            <td>The only thing you want is to iterate over the elements in a collection. You only need read-only access to that collection.</td>     
        </tr>
      <tr>
            <td>ICollection, ICollection<T></td>
            <td>You want to modify the collection or you care about its size.</td>     
        </tr>
      <tr>
            <td>IList, IList<T></td>
            <td>You want to modify the collection and you care about the ordering and / or positioning of the elements in the collection.</td>     
        </tr>
      <tr>
            <td>List, List<T></td>
            <td>Since in object oriented design you want to depend on abstractions instead of implementations, you should never have a member of your own implementations with the concrete type List/List.</td>     
        </tr>
    </tbody>
</table>

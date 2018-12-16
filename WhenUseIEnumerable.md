Interface | Scenario
-- | --
IEnumerable, IEnumerable<T> | The only thing you want is to iterate over the elements in a collection. You only need read-only access to that collection.
ICollection, ICollection<T> | You want to modify the collection or you care about its size.
IList, IList<T> | You want to modify the collection and you care about the ordering and / or positioning of the elements in the collection.
List, List<T> | Since in object oriented design you want to depend on abstractions instead of implementations, you should never have a member of your own implementations with the concrete type List/List.

![zeichnung-ienumerable-icollection-ilist](https://user-images.githubusercontent.com/5309726/50049554-0459fc00-0123-11e9-8169-1b6d82dc6ed1.png)

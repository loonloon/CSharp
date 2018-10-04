#### C# Concurrent Collections by Simon Robinson ####

#### An atomic method ####
1. Other threads always see the operation as either not started or completed, never half done.
2. Will ether finish or fail without modifying data.

#### Why Locks aren't always so great? ####
1. Very hard to locking right.
2. Easy to cause deadlock.
3. Not scalable.

#### What concurrent collections are there? ####
1. Concurrent Dictionary
2. Concurrent Queue (FIFO Producer Consumer Collection)
3. Concurrent Stack (LIFO Producer Consumer Collection)
4. Concurrent Bag (Unorder Producer Consumer Collection)

#### Concurrent Dictionary ####
- Do not protect you from race conditions between method calls.

Wont't throw exceptions if they fail  | Will always succeed
------------------------------------- | --------------------
`TryGetValue()` | `GetOrAdd()`
`TryAdd()`      | `AddOrUpdate()`
`TryRemove()`   |
`TryUpdate()`   |

#### Using indexer (will throw exception if not exists): ####
`int psStock = stock["apple"];`

#### Using `TryGetValue()`: ####
`int psStock;` <br />
`bool success = stock.TryGetValue("apple", out psStock);`

#### Using `TryUpdate()` #### 
- to avoid overwriting the actions of another thread, but require to pass in old value.
- Below example ensure the data will not be corrupted, but the value still invali in mutltithreading environment.

`int temp = stock["apple"];`

`bool success = stock.TryUpdate("apple", temp + 1, temp);`


`if(!success) {`

  //will not work, because another thread might change the value after set value to temp.

  `temp = stock["apple"];`

`
}
`
#### Using `AddOrUpdate()` for modifying values #### 
- It repeatedly evaluates the delegate and uses `TryUpdate()` until `TryUpdate()` succeeds.
- So the __delegate may execute more than once__.
- The return values from `AddOrUpdate()` could become out of date as soon as you have it.

#### Key Principle ####
- Do each operation in 1 method call.
- Example 1, OK because it only access the dictionary once.

`int psStock = stock.AddOrUpdate("apple", 1 (key, oldValue) => oldValue + 1);`

`Console.WriteLine("New value is " + psStock);`

- Example 2, NG because it access the dictionary twice. Other thread may modified the value before the console display the value.

`int psStock = stock.AddOrUpdate("apple", 1 (key, oldValue) => oldValue + 1);`

`Console.WriteLine("New value is " + stock["apple"]);`

- Example 3, OK because it does not matter if another thread sneak before `if (newStockLevel < 0)`, the logic still working fine.

`var newStockLevel = _stock.AddOrUpdate(item, -1, (key, oldValue) => oldValue - 1);`

`if (newStockLevel < 0) {`

`_stock.AddOrUpdate(item, 1, (key, oldValue) => oldValue + 1);`

`return false;`

`}`

#### Using `GetOrAdd()` for looking up values ####

#### Using `BlockingCollection` ####
- If there are no items in the collection, the `Take()` function will simply wait and blocking the thread until an item does become available to take.
- ___ConcurrentBag will perform badly if producers and comsumers are on different threads___.

#### Performance ####
-Don't use concurrent collections unless you really need the thread safety.
- `ConcurrentDictionary` and `ConcurrentBag` use ___fine-grained locking___. Its internally has different buckets (Hash table), and separate lock for each bucket. Different thread can access different bucket at the same time. 
- In `ConcurrentDictionary` methods, `IsEmpty()`, `Count`, `Clear()`, `ToArray()`, `CopyTo()`, `Values`, `Keys` are lock extensive.
- In `ConcurrentBag` methods, `IsEmpty()`, `Count`, `Clear()`, `ToArray()`, `CopyTo()`, `GetEnumerator() (foreach loop)` are lock extensive.

#### Good Practice ####
* Avoid querying the state of concurrent collections too often.
  * Answer can be out of date as soon as you have it.
  * Hurts performance.
* If enumerating `Dictionary` while modifying, it will thows exception.
* You can enumerate `ConcurrentDictionary` while it's being modified, ___but the result aren't guaranteed___.
  * For `ConcurrentStack`, `ConcurrentBag` and `ConcurrentQueue`, snapshot of collection taken and subsequent changes ignored.
  * In `ConcurrentDictionary`, `ToArray()`, `Values`, `Keys` methods will take snapshot (slow) of the collection.
* ___Use concurrent collection when multiple writers situation___. 
* How about ___multiple readers___ using standard collections? It often work but risk, read values might changed under hood. Solutions?
  * Use concurrent collection/
  * Use immutable collection.

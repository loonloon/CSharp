
#### Asynchronous Programming in .NET – Common Mistakes and Best Practices by rubik's code ####

#### Async Void ####

* If an exception is thrown in async Task method it will be captured and placed within Task object. 
* If an exception is thrown inside an async void method, it will be raised directly on the SynchronizationContext that was active.

```
private async void ThrowExceptionAsync()
{
  throw new Exception("Async exception");
}

public void AsyncVoidExceptions_CannotBeCaughtByCatch()
{
  try
  {
    ThrowExceptionAsync();
  }
  catch (Exception)
  {
    // The exception is never caught here!
    throw;
  }
}
```
* Don’t provide an easy way to notify calling code that they’ve completed. 
* It is very hard to test them. Unit testing frameworks, such as xUnit or NUnit, work only for async methods that are returning Task or Task<T>.
* The only exception might be in case of asynchronous event handlers, which must return void.

#### There is no Thread ####

When we are using our computer, we are having multiple programs running at the same time, which is achieved by running instructions from the different process one at the time on the CPU.

Since these instructions are interleaved and CPU switches from one to another rapidly (context switch) we get an illusion that they are running at the same time. This process is called concurrency. 

Now, when we are having multiple cores in our CPU, we are able to run multiple streams of these instructions on each core. This is called parallelism. Now, it is important to understand that both of these concepts are available on the CPU level. On the OS level, we have a concept of threads – a sequence of instructions that can be managed independently by a scheduler.

```
public async Task WriteMyDeviceAcync
{
  byte[] data = ...
  myDevice.WriteAsync(data, 0, data.Length);
}
```

Now, let’s go down the rabbit hole. WriteAsync will start overlapped I/O operation on the device’s underlying HANDLE. After that, OS will call the device driver and ask it to start write operation. That is done in two steps. Firstly, the write request object is created – I/O Request Packet or IRP. Then, once device driver receives IRP, it issues a command to the actual device to write the data. There is one important fact here, the device driver is not allowed to block while processing IRP, not even for synchronous operations.

This makes sense since this driver can get other requests too, and it shouldn’t be a bottleneck. Since there is not much more than it can do, device driver marks IRP as “pending” and returns it to the OS. IRP is now “pending”, so OS returns to WriteAsync. This method returns an incomplete task to the WriteMyDeviceAcync, which suspends the async method, and the calling thread continues executing.

After some time, device finishes writing, it sends a notification to the CPU and magic starts happening. That is done via an interrupt, which is at CPU-level event that will take control of the CPU. The device driver has to answer on this interrupt and it is doing so in ISR – Interrupt Service Routine. ISR in return is queuing something called Deferred Procedure Call (DCP), which is processed by the CPU once it is done with the interrupts.

DCP will mark the IRP as “complete” on the OS level, and OS schedules Asynchronous Procedure Call (APC) to the thread that owns the HANDLE. Then I/O thread pool thread is borrowed briefly to execute the APC, which notifies the task is complete. UI context will capture this and knows how to resume.

Notice how instructions that are handling the wait – ISR and DCP are executed on the CPU directly, “below” the OS and “below” the existence of the threads. In an essence, there is no thread, not on OS level and not on device driver level, that is handeling asynchronous mechanism.

#### Foreach and Properties ####

Now, this code is even though it is written in an asynchronous manner, it will block executing of the flow everytime `WaitThreeSeconds` is awaited. 

This is a real-world situation, for example, `WaitThreeSeconds` is calling some sort of the Web API, let’s say it performs an HTTP GET request passing data for a query. Sometimes we have situations where we want to do that, but if we implement it like this, we will wait for each request-response cycle to be completed before we start a new one. That is inefficient.

```
// Bad
var listOfInts = new List<int>() { 1, 2, 3 };

foreach (var integer in listOfInts)
{
    await WaitThreeSeconds(integer);
}

private async Task WaitThreeSeconds(int param)
{
    Console.WriteLine($"{param} started ------ ({DateTime.Now:hh:mm:ss}) ---");
    await Task.Delay(3000);
    Console.WriteLine($"{ param} finished ------({ DateTime.Now:hh: mm: ss}) ---");
}

```

```
// Good
var listOfInts = new List<int>() { 1, 2, 3 };
var tasks = new List<Task>();

foreach (var integer in listOfInts)
{
    var task = WaitThreeSeconds(integer);
    tasks.Add(task);
}

await Task.WhenAll(tasks);
```

### Async all the way ###
There are two common mistakes that lie within sync/async code mix:

* Blocking in asynchronous code
* Making asynchronous wrappers for synchronous methods

```
public async Task InitiateWaitTask()
{
    var delayTask = WaitAsync();
    delayTask.Wait();
}

private static async Task WaitAsync()
{
    await Task.Delay(1000);
}
```

Why this code can deadlock? Well, that is one long story about SynchronizationContext, which is used for capturing the context of the running thread. 

To be more exact, when incomplete Task is awaited, the current context of the thread is stored and used later when the Task is finished. 
This context is the current SynchronizationContext, ie. current abstraction of the threading within an application. GUI and ASP.NET applications have a SynchronizationContext that permits only one chunk of code to run at a time. 

However, ASP .NET Core applications don’t have a SynchronizationContext so they will not deadlock. To sum it up, you shouldn’t block asynchronous code.

Today, a lot of APIs have pairs of asynchronous and methods, for example, `Start()` and `StartAsync()`, `Read()` and `ReadAsync()`. 

We may be tempted to create these in our own purely synchronous library, but the fact is that we probably shouldn’t. As the Stephen Toub perfectly described in his blog post, if a developer wants to achieve responsiveness or parallelism with synchronous API, they can simply wrap the invocation with `Task.Run()`. There is no need for us to do that in our API.

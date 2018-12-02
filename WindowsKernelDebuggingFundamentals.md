
#### Windows Kernel Debugging Fundamentals by Bruce Mackenzie-Low ####

#### Debugging Crashes vs. Hangs  ####

#### System Crashes ####
* Crashes are unexpected, unhandled exceptions that occur in kernel mode
* The state of the operating system becomes questionable
* The operating system stops to avoid corrupting data
* Also known as the Blue Screen of Death (BSOD), Stops and Bugchecks
* The contents of RAM is saved in a file called Memory.dmp

#### System Hangs ####
* Hangs occur when processors or peripheral devices become unresponsive
* Software induced hangs include:
  * Depleted (耗尽) system resources (memory pool)
  * Runaway high priority compute-bound threads
  * Synchronization mechanisms (deadlocks & spinlocks)
  
* Hardware originated hangs include:
  * Faulty devices causing false interrupts
  * Failling processors
  * Corrupted RAM
  
* Memory dump must be manually forced on a hung system

#### Common Culprits (罪魁祸首) for Crashes ####
* Old antivirus software
* New drivers
  * Storport (over 50 hotfixes in Winodes 2003)
  * MPIO (over dozen hotfixes in Winodes 2008)
 * Incompatible drivers
   * Old Storport driver with a new miniport driver
 * Too many filter drivers
   * Antivirus, Deduplication, Disk quoto, Mirroring
 * Memory corruption
 * Hardware failures
 * Operating system bugs
 * Depleted system resources (memory pool)
 * Broken applications with deadlocks or spinlock hangs
 * High priority compute-bound (runaway) applications
 * Old antivirus software
 * New drivers
 * Incompatible driver
 * Broken hardware
 
 #### Determining Driver Dependencies ####
 * Dependency Walker (Depends.exe)
 
 #### How Memory Dumps are Created ####
 
![dump](https://user-images.githubusercontent.com/5309726/49303326-66e8b080-f504-11e8-8ec1-3e08617750f8.png)

 #### Types of Memory Dumps ####
 
 Dump      | Description
 --------- | --------------------------------------------------------------------------------------------------------------
 Small     | 64K size, containing minimal debugging information (stop code, parameters, stack, drivers).
 Kernel    | Medium size, containing kernel data structures, drivers and current process & thread information.
 Complete  | Large size, containing complete contents of memory. It's takes time.
 Automatic | New to Windows Server 2012 and 8, same as kernel memory dump but uses a smaller page file when system managed page files are used. Useful for PCs with SSD and lots of RAM.

Formerly called EFD *(Enterprise for Dummies)*.  It's now called **Sepia** *(Simple Enterprise Patterns in Action)*.  The subtitle is *How to stop writing the same code over and over again.*

This library is **not yet ready for prime time**, use at your own peril.  Breaking changes are a given.

Features
--------

- [`Schematron`](./src/Sepia/Schematron/) - Rules based validation for XML based on [ISO/IEC 19757 - Document Schema Definition Languages - Part 3](http://www.dsdl.org/).
- [`Calendaring`](./src/Sepia/Calendaring/) - Calendaring and Scheduling based on [RFC 5545 - Internet Calendaring and 
Scheduling Core Object Specification (iCalendar)](http://tools.ietf.org/html/rfc5545) and
[and RFC 6350 - vCard Format Specification](http://tools.ietf.org/html/rfc6350).
- [`Open ID Connect`](./src/Sepia/OpenIdConnect/) - Authentication with [OpenID](http://openid.net/developers/specs/). 
- [`Multi-lingual text`](./src/Sepia/MultilingualText.cs) - A collection of text that represents the same concept in multiple [BCP 47](http://tools.ietf.org/html/bcp47) language tags .
- [`Tag`](./src/Sepia/Tag.cs) - A classification system that is not dependent upon an object's properties.
- [`TimeRange`](./src/Sepia/TimeRange.cs) - Inclusive start and exclusive for `DateTimeOffset`. Also supports a partial date/time.
- [`Guard`](./src/Sepia/Guard.cs) - Prevents bad data from entering the system.

Lessons learnt
--------------

- Avoid over specification.  Just do what is required.
- Do not design for the for future, its never turns out to be what you expected.
- Classes should do one and only one thing well.  Rely on other classes to do things you are not interested in.
- Write documentation.  How else will someone now about your feature or be able to support it.
- Write unit tests.  They give confidence that the feature is working.  They also raise the confidence level when refactoring.
- Unit tests should be available for feature requirements and edge cases.
- Always write a unit test for a bug.
- Loosely coupled designs are easier to change.
- Avoid null collections.  Its better to return an empty collection.
- Log everything so that when production failures occurs you can determine what is happening.

Conventions
-----------

A property containing a time (DateTimeOffset) should be named *x*On, where *x* indicates the usages. Some examples are StartsOn, EndsOn, CreatedOn, DeletedOn.
 
Use the "o" format specifier when converting a time to a string, unless presenting the time to an end user.

Documentation
-------------

The Sepia (.Net) class reference is built by Sandcastle http://shfb.codeplex.com/ using the project file (Source\Sepia.shfbproj).  A compiled HTML is generated at Source\Documentation\Sepia.chm.
The Source\Documentation\Install_Sepia.bat can be used to install the class reference into Microsoft Help viewer.

> Will be moving to [Nudoq](http://www.nudoq.org) soon.

All methods should use XML commenting tags.

Code examples are placed in the SepiaExample project, so that we can verify that the code is correct.  The snippet is referenced like this:

    /// <example>
    ///   <code title="Multilingual Hello World" source="SepiaExamples\TextExample.cs" region="Hello World" language="C#" />
    /// </example>


Immutability
------------

Eric Lippert talks about immutability in <http://blogs.msdn.com/b/ericlippert/archive/2007/11/13/immutability-in-c-part-one-kinds-of-immutability.aspx>. Sepia supprts **Popsicle** immutability.

Time
----

Matt Johnson has a good discussion of calendar vs instantaneous time at <http://stackoverflow.com/questions/4331189/datetime-vs-datetimeoffset>. 

We will always use instantaneous time as represented by DateTimeOffset. DateTimeOffset supports UTC which is instantaneous and 
also records the end user perception of time.  We will simply refer to it as time; not as date-time because a date implies a calendar system.

Comparing time is problematic because the clock sources (different nodes on a network) will vary slightly and we cannot guarantee that two times 
come from the same clock source.  Clock skew is the difference between two clocks and is unavoidable (10 minutes is common in the internet). We need
to take into consideration the clock skew when comparing times.

The DateTimeOffset is extended to support skewed comparison (FuzzyEquals and FuzzyCompare)  These methods take an optional acceptable clock 
drift into consideration. Equality is defined as |T0 - T1| <= Drift.

Determining if a time is within time period (start and end) is cause for confusion.  Are the time period components inclusive, exclusive or
some other combination.  Most people perceive that a period starts on a specific time and is finished when the end time is reached.  This implies 
that start time is inclusive while end time is exclusive.  The IsIn and FuzzyIsIn extension is added to DateTimeOffset. Normally a "never ends" 
would be DateTimeOffset.MaxValue, however some platforms may have different max values so a nullable time is used to indicate "never ends".


The Network Time Protocol can reduce clock skew but not eliminated it.

Use the "o" format specifier when converting a time to a string, unless presenting the time to an end user.

Many objects in an enterprise only exist during a specified time interval (start and end time).  The Lifetime class implements this behaviour with the
properties StartsOn and EndsOn.  The indicators IsAlive, IsNotAlive and IsExpired all use the fuzzy time extensions.  Normally, I would default EndsOn
to DateTimeOffset.MaxValue, but this value may not be presentable in systems.  So a nullable time is use to indicate "never ends".

Unit Tests
----------
We use MS Unit Test (Microsoft.VisualStudio.TestTools.UnitTesting).  Each C# project should have a corresponding Tests project.  The namespace for
for the Tests project is the same as the C# project.  Unit tests classes have the name of the class under test with 'Test' appended.

Unit test methods should be short and sweet.  Just test one thing.  Each method should have a one sentence summary tag, that explains the reason for the test.  A good
rule of thumb is to assume that "Assert that" precedes the sentence, such as "Inclusive start and exclusive end times are used.".  Unit test method names tend
to be the same as the method under test.

        /// <summary>
        ///   Hash codes are the same for equal objects.
        /// </summary>
        [TestMethod]
        public void Hashing()
        {
            var a = new Text("en", "hello world");
            var b = new Text("en", "hello world");
            var c = new Text("en-AU", "g'day mate");

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        }

Remember to test the edge cases, these tend to be the most buggy. 

The ExceptionAssert.Throws<Exception> can be use to test that a method throws an expected Exception.

XML Comments
------------
*TODO*

Logging
-------
Common Logging is used by Sepia.  This allows the application to select a specific logging implementation at runtime.

Performance Monitoring
----------------------
*TODO*

TODOs
-----

- Use MSBuild and/or NANT to build debug and release distributions with documentation and then run the unit tests and maybe even code
coverage.  Also, look at NUGET.

- When we go online, need to sort Web HTML documentation and project stats (code changes, downloads, unit test, code coverage, etc.)

- A recurringTime (RFC vCalendar???)

- A high performance thread safe in-memory cache.

- IOC and service locator patterns.

- What is the pattern for configuration (system parameters)?

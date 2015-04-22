#  ![](https://raw.github.com/kbilsted/StatePrinter/master/StatePrinter/gfx/stateprinter.png) StatePrinter 

[![Join the chat at https://gitter.im/kbilsted/StatePrinter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/kbilsted/StatePrinter?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://ci.appveyor.com/api/projects/status/vx0nl4y4iins506u/branch/master?svg=true)](https://ci.appveyor.com/project/kbilsted/stateprinter/branch/master)
[![Nuget](https://img.shields.io/nuget/dt/stateprinter.svg)](http://nuget.org/packages/stateprinter)
[![Nuget](https://img.shields.io/nuget/v/stateprinter.svg)](http://nuget.org/packages/stateprinter)
[![Nuget](https://img.shields.io/nuget/vpre/stateprinter.svg)](http://nuget.org/packages/stateprinter)
[![Coverage Status](https://coveralls.io/repos/kbilsted/StatePrinter/badge.svg?branch=master)](https://coveralls.io/r/kbilsted/StatePrinter?branch=master)
[![Stats](https://img.shields.io/badge/Code_lines-3,3_K-ff69b4.svg)]()
[![Stats](https://img.shields.io/badge/Doc_lines-1,4_K-ff69b4.svg)]()

# What is Stateprinter
A simple framework for **automatic** `Assert.AreEquals()`, `Assert.That()` and `ToString()` 

StatePrinter is a little library that turn any object-graph into a string representation. It is mainly intended automating writing `ToString` methods and help automating aspects of writing and maintaining unit tests. Code which is rather dreary and boring to write. 

Why you should take StatePrinter for a spin

* *No more manual .ToString()* - it is much easier to write robus and self-sufficient `ToString()` methods. 
* *No more manual writing/updating Asserts* - both for new tests and when the code changes, all tests can automatically be corrected.
* *No more screens full of asserts*. Especially testing against object-graphs is a bliss. 
* Very configurable and extensible.
* It is part of the back-end engine of other projects
  * The very nice [ApprovalTests framework](http://approvaltests.sourceforge.net/).



# How do I get started

The documentation is split into
* [Automating ToStrings](https://github.com/kbilsted/StatePrinter/blob/master/doc/AutomatingToStrings.md)

and

* [Automating unit testing](https://github.com/kbilsted/StatePrinter/blob/master/doc/AutomatingUnitTesting.md)
* [The problems with traditional unit testing (that stateprinter solves)](https://github.com/kbilsted/StatePrinter/blob/master/doc/TheProblemsWithTraditionalUnitTesting.md)

and

* [Configuration and exension](https://github.com/kbilsted/StatePrinter/blob/master/doc/HowToConfigure.md) 


# Where can I get it?
Install Stateprinter from the package manager console:

```
PM> Install-Package StatePrinter
```

And for pre-release versions

```
PM> Install-Package StatePrinter -Pre
```


# How can I get help?
For quick questions, [Stack Overflow](http://stackoverflow.com/questions/tagged/stateprinter?sort=newest) is your best bet. For harder questions, bugs, issues or feature requests, [create a GitHub Issue (and let's chat)](https://github.com/kbilsted/StatePrinter/issues/new).



# How can I help out
Everyone is encouraged to help improve this project. Here are a few ways you can help:
* Blog about your experinces with the tool. We highly need publicity. I'll gladly link from here to your blog.
* [Report bugs](https://github.com/kbilsted/StatePrinter/issues/new)
* [Fix issues](https://github.com/kbilsted/StatePrinter/issues/) and submit pull requests
* Write, clarify, or fix [the documentation](doc/)
* [Suggest](https://github.com/kbilsted/StatePrinter/issues/new) or add new features


*StatePrinter has been awarded a ReSharper group lincense, to share among all active contributers*.



# History
Version History: http://github.com/kbilsted/StatePrinter/blob/master/CHANGELOG.md

This file describes the latest pushed changes. For documentation of earlier releases see:
[1.0.6](https://github.com/kbilsted/StatePrinter/blob/1.0.6/README.md), [1.0.5](https://github.com/kbilsted/StatePrinter/blob/1.0.5/README.md), [1.0.4](https://github.com/kbilsted/StatePrinter/blob/1.0.4/README.md)

Upgrading from v1.xx to v2.0.x should be a matter of configuring the `Configuration.LegacyBehaviour`



# Requirements
Requires .NET 3.5 or newer.




# License
Stateprinter is under the Apache License 2.0, meaning that you can freely use this in other open source or commercial products. If you use it for commercial products please have the courtesy to leave me an email with a 'thank you'. 



Have fun!

Kasper B. Graversen

# Multiagent System Example

This project showcases a distributed agent system that is being used on a flotilla of vehicles
to acquire near realtime (<10 ms latency) scientific data while moving on average at 100 km/h. Data comes from various equipment such as
a [high-precision inertial GPS system](http://www.datrontechnology.co.uk/industry/surveying-systems/aerial-survey/inertial/),
a [laser crack measurement system (LCMS)](http://www.pavemetrics.com/applications/road-inspection/laser-crack-measurement-system/),
multiple [high-resolution cameras](https://www.alliedvision.com/en/products/cameras.html), ...
The production system also has realtime analysis agents that handle tasks such as detecting lane markings from photos of the road,
finding the current road segment number from the current position, extrapolating the current position when the GPS signal is lost, ...

Each agent can optionaly display an interactive GUI, which can be shown locally, remotely or even on multiple computers at the same time.
The interface design is specific to each agent and is able to show raw/aggregated data, display charts, execute commands, ...
Agents can also subscribe to the data source of other agents and will automatically reconnect in case of failure.

The system is built with a reactive architecture and supports point-to-point or broadcast remote commands.
It is based on WCF, [Reactive.NET](https://github.com/dotnet/reactive) and [TCP Qbservable](http://davesexton.com/blog/post/LINQ-to-Cloud-IQbservable-Over-the-Wire.aspx) (2013)
that later became [Qactive](https://github.com/RxDave/Qactive/) in 2016.

The first production version of the project was completed in 2014. Starting from scratch today,
certainly using [Akka.NET](https://github.com/akkadotnet/akka.net/) would be a no brainer,
but back then, it was deemed not yet ready to meet the system requirements in terms of features, performance (especially latency) and stability.
Even if it is not using a full-featured and dedicated open-source agent framework, this project still offers a good example of the power and relative simplicity gained
by using a reactive architecture, especially regarding concurrency and data flow.

This repository contains four solutions:

- `DLC.Framework`: core library mostly for IO, reactive programming and concurrency (especially multithreading inside GUI);
- `DLC.Multiagent`: multiagent system core functionalities, including an agent management UI
- `DLC.Scientific.Core`: core agents definition and scientific data analysis functions
- `DLC.Scientific.Acquisition`
	- `Core`: definition and base implementation for raw data providers and agents
	- `Modules`: specific raw data providers implementation
	- `Agents`: specific agents implementation
	- `Tools`: related system tools

## DLC.Multiagent Class Diagram

![DLC.Multiagent class diagram](./docs/assets/multiagent-class-diagram.png)
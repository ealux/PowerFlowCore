
<p align="center"><img src="content/main.png" alt="alt text" width="300" height="141"/></p>
<h3 align="center"><i><b>Solver for Power Flow Problem</b></i></h3>
<div align="center">
    
  <a href=""> [![NuGet](https://img.shields.io/nuget/v/PowerFlowCore)](https://www.nuget.org/packages/PowerFlowCore) </a>
  <a href=""> [![license](https://img.shields.io/github/license/ealux/PowerFlowCore)](https://github.com/ealux/PowerFlowCore/blob/dev/LICENSE.md) </a>

</div>

## What's new:

* **0.13.3** - Major performance improvement. Step 2.
  * Improve performance again</li>
  * Eliminate SolvableGrid class (move solvers list to Grid class)
* 0.13.2 - Major performance improvement.
* 0.13.1 - Sparse algebra. Performance improvement.
* 0.12.1 - Samples. Stabilizing. Cleanup.

## Features:
* Three-phase AC mode grids calculations
* Flexible system to set up configuration of calculations
* [`Newton-Raphson`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverNR.cs) and [`Gauss-Seidel`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Solvers/SolverGS.cs) solvers
* [`Load models`](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore/Data/LoadModels/Models) with variant structure
* Algorithms on graphs (connectivity etc.)
* Network operational limits control
* Parallel calculations from box 

Samples are presented in [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project. Library benchmarking is presented in [PowerFlowCore.Benchmark](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark) project.

## Quick example

Next example assumes that `Node` and `Branch` classes inherits [`INode`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/GridElements/INode.cs) and [`IBranch`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/GridElements/IBranch.cs) interfaces respectively. 
More examples can be found in [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project.

Create grid:
```csharp
using PowerFlowCore;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;

var nodes = new List<INode>()        // Create collection of Nodes
{
    new Node(){Num = 1, Type = NodeType.PQ,    Unom=115,  Vpre = 0,     S_load = new Complex(10, 15)},
    new Node(){Num = 2, Type = NodeType.PQ,    Unom=230,  Vpre = 0,     S_load = new Complex(10, 40)},
    new Node(){Num = 3, Type = NodeType.PV,    Unom=10.5, Vpre = 10.6,  S_load = new Complex(10, 25),   S_gen = new Complex(50, 0), Q_min=-15, Q_max=60},
    new Node(){Num = 4, Type = NodeType.Slack, Unom=115,  Vpre = 115}
};

var branches = new List<IBranch>()   // Create collection of Branches
{
    new Branch(){Start=2, End=1, Y=1/(new Complex(0.5, 10)), Ktr=Complex.FromPolarCoordinates(0.495,    15 * Math.PI/180), Ysh = new Complex(0, -55.06e-6)},
    new Branch(){Start=2, End=3, Y=1/(new Complex(10,  20)), Ktr=Complex.FromPolarCoordinates(0.045652, 0 * Math.PI/180), Ysh = new Complex(0, 0)},
    new Branch(){Start=1, End=4, Y=1/(new Complex(8,   15)), Ktr=1},
    new Branch(){Start=1, End=4, Y=1/(new Complex(20,  40)), Ktr=1}
};

var grid = new Grid(nodes, branches);   // Create Grid object
```

Inspect connectivity:

```csharp
bool connected = grid.IsConnected();
```

Calculate grid (for more details look [Calculate()](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Engine/Engine.cs) methods):

```csharp
bool success = false;           // To save calculation result

grid.Calculate();               // Default calculation
// or
(grid, success) = grid.Calculate(new CalculationOptions() { IterationsCount = 5 }});        // Calculation with options and saving results
// or
grid.Calculate(out success);    // Calculate with result short saving 
// or
grid.ApplySolver(SolverType.GaussSeidel, new CalculationOptions() { IterationsCount = 3 })  // Apply multiple solvers
    .ApplySolver(SolverType.NewtonRaphson)
    .Calculate(out success);
```

## Basic concepts

### Namespaces
Provided tools are located in several namespaces:

```csharp
using PowerFlowCore;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;
using PowerFlowCore.Algebra;
```
### Components

#### INode, IBranch

[`INode`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/GridElements/INode.cs) and [`IBranch`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/GridElements/IBranch.cs) interfaces encapsulate properties to work with internal solver. These interfaces should be inherited by custom **class** or **struct** to use in solver. Being passed to the solver are converted to the original interface.

#### Grid

Central term is [`Grid`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/Grid.cs) object from `PowerFlowCore.Data` namespace. 
To create [`Grid`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/Grid.cs) object collections of `INode` and `IBranch` should be explicitly given to the constructor:

```csharp
public Grid(IEnumerable<INode> nodes, IEnumerable<IBranch> branches) { ... }
```

Another way to create `Grid` is to use `IConverter` object that encapsulated collection of `INode` and `IBranch`:

```csharp
public Grid(IConverter converter) { ... }
```

Besides collections of nodes and branches [`Grid`](https://github.com/ealux/PowerFlowCore/blob/master/PowerFlowCore/Data/Grid.cs) contains:
* Admittance matrix - **Y**
* Vector of nodes nominal voltages - **Unominal**
* Vector of nodes initial voltages (for calculations) - **Uinit**
* Vector of nodes calculated voltages - **Ucalc**
* Vector of nodes power injections (=generation-load) - **S**
* Collection of load models - **LoadModels**
* Description:
  * Load nodes count - **PQ_Count** 
  * Generator nodes count - **PV_Count** 
  * Slack bus nodes count - **Slack_Count** 


## License

Published under [MIT license](https://github.com/ealux/PowerFlowCore/blob/master/LICENSE.md)

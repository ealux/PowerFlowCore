
<p align="center"><img src="content/main.png" alt="alt text" width="400" height="186"/></p>
<h2 align="center"><b>Solver for Power Flow Problem</b></h2>

### Features:
* Three-phase AC mode grids calculations
* Flexible system to set up configuration of calculations
* `Newton-Raphson` and `Gauss-Seidel` solvers
* `Load models` with variant structure
* Algorithms on graphs (connectivity etc.)
* Network operational limits control
* Parallel calculations from box 

Samples are presented in [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project. Library benchmarking is presented in [PowerFlowCore.Benchmark](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Benchmark) project.

### Quick example

Next example assumes that `Node` and `Branch` classes inherits `INode` and `IBranch` interfaces respectively. 
More examples can be found in [PowerFlowCore.Samples](https://github.com/ealux/PowerFlowCore/tree/master/PowerFlowCore.Samples) project.

Create grid:
```csharp
using PowerFlowCore;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;

var nodes = new List<Node>()        // Create collection of Nodes
{
    new Node(){Num = 1, Type = NodeType.PQ,    Unom=115,  Vpre = 0,     S_load = new Complex(10, 15)},
    new Node(){Num = 2, Type = NodeType.PQ,    Unom=230,  Vpre = 0,     S_load = new Complex(10, 40)},
    new Node(){Num = 3, Type = NodeType.PV,    Unom=10.5, Vpre = 10.6,  S_load = new Complex(10, 25),   S_gen = new Complex(50, 0), Q_min=-15, Q_max=60},
    new Node(){Num = 4, Type = NodeType.Slack, Unom=115,  Vpre = 115}
};

var branches = new List<Branch>()   // Create collection of Branches
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

### Basic concepts

#### Namespaces
Provided tools are located in several namespaces:

```csharp
using PowerFlowCore;
using PowerFlowCore.Data;
using PowerFlowCore.Solvers;
```
#### Components

##### INode, IBranch

##### Grid

Central term is `Grid` object from `PowerFlowCore.Data` namespace. To create `Grid` object collections of `INode` and `IBranch` should be explicitly given to the constructor:

```csharp
public Grid(IEnumerable<INode> nodes, IEnumerable<IBranch> branches) { ... }
```

Another way to create `Grid` is to use `IConverter` object that encapsulated collection of `INode` and `Branch`:

```csharp
public Grid(IConverter converter) { ... }
```

### License

Published under [MIT license](https://github.com/ealux/PowerFlowCore/blob/master/LICENSE.md)
# NextMachina - Functional Hierarchical State Machine Library

A lightweight and flexible C# library for implementing hierarchical state machines. It offers a fluent API (DSL) to define states, transitions, and actions.

## Features

*   **Hierarchical States**: Support for composite states (sub-states) with initial state management.
*   **Fluent DSL**: Clear and readable state machine definition.
*   **Actions**: Support for `OnEntry`, `OnExit`, and `OnState` actions.
*   **Conditional Transitions**: Definition of guards (`When`) to control transitions.
*   **Generic**: Strong typing for States, Inputs, and Outputs.

## Installation

The project is a .NET standard solution. You can integrate it directly into your solution or reference the `NextMachina` project.

## Usage Example

Here is a simple example showing how to define and use a state machine.

```csharp
using NextMachina.Engine;
using static NextMachina.Dsl.StateMachineDsl<MyStateEnum, int, string>;
using static NextMachina.Dsl.TransitionState<MyStateEnum, int>;

// 1. Define your states (e.g., with an Enum)
public enum MyStateEnum { Idle, Working, Paused }

// 2. Create the state machine via the DSL
var stateMachine = StateMachine(MyStateEnum.Idle)
    .State(MyStateEnum.Idle)
        .Transitions(
            To(MyStateEnum.Working).When(input => input == 1)
        )
    .State(MyStateEnum.Working)
        .OnEntry(input => "Started working")
        .OnExit(input => "Stopped working")
        .Transitions(
            To(MyStateEnum.Paused).When(input => input == 0),
            To(MyStateEnum.Idle).When(input => input == -1)
        )
    .State(MyStateEnum.Paused)
        .Transitions(
            To(MyStateEnum.Working).When(input => input == 1)
        )
    .BuildDefinition()
    .Create();

// 3. Use the state machine
// TransitionFrom takes the current state and an input, and returns the new state and any outputs.
var (nextState, outputs) = stateMachine.TransitionFrom(MyStateEnum.Idle, 1);
// nextState will be MyStateEnum.Working
```

## Project Structure

*   `src/`: Source code of the library.
    *   `Dsl/`: Implementation of the Domain Specific Language for configuration.
    *   `Engine/`: Execution engine of the state machine.
    *   `Definitions/`: Data structures defining the state graph.
*   `tests/`: Unit tests (NUnit) demonstrating various use cases (composite states, transitions, etc.).

## Contributing

Contributions are welcome. Make sure all tests pass before submitting a Pull Request.

```bash
dotnet test
```

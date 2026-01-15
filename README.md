# Hierarchical State Machine (HSM)

Une bibliothèque C# légère et flexible pour implémenter des machines à états hiérarchiques. Elle propose une API fluide (DSL) pour définir les états, les transitions et les actions.

## Fonctionnalités

*   **États Hiérarchiques** : Support des états composites (sous-états) avec gestion des états initiaux.
*   **DSL Fluide** : Définition claire et lisible de la machine à états.
*   **Actions** : Support des actions `OnEntry`, `OnExit` et `OnState`.
*   **Transitions Conditionnelles** : Définition de gardes (`When`) pour contrôler les transitions.
*   **Générique** : Typage fort pour les États, les Entrées (Inputs) et les Sorties (Outputs).

## Installation

Le projet est une solution .NET standard. Vous pouvez l'intégrer directement dans votre solution ou référencer le projet `HierarchicalStateMachine`.

## Exemple d'utilisation

Voici un exemple simple montrant comment définir et utiliser une machine à états.

```csharp
using HierarchicalStateMachine.Engine;
using static HierarchicalStateMachine.Dsl.StateMachineDsl<MyStateEnum, int, string>;
using static HierarchicalStateMachine.Dsl.TransitionState<MyStateEnum, int>;

// 1. Définir vos états (par exemple avec une Enum)
public enum MyStateEnum { Idle, Working, Paused }

// 2. Créer la machine à états via le DSL
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

// 3. Utiliser la machine à états
// TransitionFrom prend l'état actuel et une entrée, et retourne le nouvel état et les sorties éventuelles.
var (nextState, outputs) = stateMachine.TransitionFrom(MyStateEnum.Idle, 1);
// nextState sera MyStateEnum.Working
```

## Structure du Projet

*   `src/` : Code source de la bibliothèque.
    *   `Dsl/` : Implémentation du Domain Specific Language pour la configuration.
    *   `Engine/` : Moteur d'exécution de la machine à états.
    *   `Definitions/` : Structures de données définissant le graphe d'états.
*   `tests/` : Tests unitaires (NUnit) démontrant les différents cas d'usage (états composites, transitions, etc.).

## Contribuer

Les contributions sont les bienvenues. Assurez-vous que tous les tests passent avant de soumettre une Pull Request.

```bash
dotnet test
```

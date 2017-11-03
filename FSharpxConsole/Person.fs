module Person

open System
open FSharpx.Functional
open FSharpx.Collections
open FSharpx.Choice
open FSharpx.Validation

type Person =
  private    
    { Name: string
      Age: uint8 }

let (|Person|) {Name = n; Age = a} = (n, a)

type NameError = | NameEmpty

type AgeError = | AgeTooHigh

type PersonError =
    | NameE of NameError
    | AgeE of AgeError

let validator pred error value =
    if pred value
    then Choice1Of2 value
    else Choice2Of2 (NonEmptyList.singleton error)

let validateName =
    validator 
        (not << String.IsNullOrWhiteSpace)
        (NameE NameEmpty)

let validateAge =
    validator
        (flip (<) 140uy)
        (AgeE AgeTooHigh)

let createPerson name age: Choice<Person, NonEmptyList<PersonError>> =
    returnM {Name = name; Age = age }
    <* validateName name
    <* validateAge age
module FSharpxConsole

open System
open FSharpx
open FSharpx.Option

open FSharpx.Functional
open FSharpx.Collections
open FSharpx.Choice
open FSharpx.Validation

type Person =
  { Name: string
    Age: uint8 }

type NameError = | NameEmpty

type AgeError = | AgeTooHigh

type PersonError =
    | NameE of NameError
    | AgeE of AgeError


module WithOption =
    let check pred value =  
        if pred value
        then Some value
        else None

    let personOptional name age = 
        maybe {
            // Rule #1: Name should not be empty
            let! n = check (not << String.IsNullOrWhiteSpace) name
            // Rule #2: Age cannot be more than 140
            let! a = check ((<) 140uy) age    
            return { Name = n; Age = a }
        }  

module WithChoice =
    let check pred error value =
        if pred value
        then Choice1Of2 value
        else Choice2Of2 error

    let checkName = check (not << String.IsNullOrWhiteSpace) (NameE NameEmpty)
    let checkAge = check (flip (<) 140uy) (AgeE AgeTooHigh)

    let personChoice name age =
        choose {
            let! n = checkName  name
            let! a = checkAge age
            return { Name = n; Age = a }
        }

module WithValidation =
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

    let validatePerson (p: Person): Choice<Person, NonEmptyList<PersonError>> =
        returnM p
     <* validateName p.Name
     <* validateAge p.Age

open WithValidation

[<EntryPoint>]
let main _ =
    let p = { Name = ""; Age = 200uy }
    match validatePerson p with
    | Choice1Of2 person  -> printfn "Success: %A" person 
    | Choice2Of2 es -> printfn "Failure: %A" es
    0 // return an integer exit code
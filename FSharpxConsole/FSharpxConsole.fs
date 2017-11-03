module FSharpxConsole

open Person

[<EntryPoint>]
let main _ =
    // Compile error FS1093:
    // The union cases or fields of the type 'Person' are not accessible from this code location
    // let batman = {Name = "Bruce Wayne"; Age = 34uy}
    
    let batman = createPerson "Bruce Wayne" 34uy
    0 // return an integer exit code
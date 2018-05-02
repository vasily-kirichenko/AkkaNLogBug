open System
open Akkling
open Akkling
open System

[<EntryPoint>]
let main _ =
    let config = """
akka {  
    stdout-loglevel = DEBUG
    loglevel = DEBUG
    log-config-on-start = on        
    loggers = ["Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog"]
    actor {                
        debug {  
              receive = on 
              autoreceive = on
              lifecycle = on
              event-stream = on
              unhandled = on
        }
    }  
}"""
    let sys = System.create "test" (Configuration.parse config)

    let slave =
        props (actorOf2 (fun ctx msg ->
            logInfof ctx "Got %A" msg
            ignored()
        ))
        
    let master = 
        props (actorOf2 (fun ctx msg ->
            logInfof ctx "Got %A, spawing a slave..." msg
            let s = spawn ctx "slave-actor" slave
            s <! "From master to slave!"
            ignored()
        ))


    let a = spawn sys "master-actor" master
    a <! "oops!" 
    Console.ReadKey() |> ignore
    0

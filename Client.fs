namespace midterm_project

open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating
open WebSharper.UI.Notation

[<JavaScript>]
module Templates =

    type MainTemplate = Templating.Template<"index.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>

[<JavaScript>]
module Server =

    [<Rpc>]
    let DoSomething input =
        let R (s: string) = System.String(s.ToCharArray())
        async {
            return R input
        }
    let Echo msg =
        async {
            return sprintf " %s." msg
        }

[<JavaScript>]
module Client =

    let index () =
        let doggo = Var.Create ""
        Templates.MainTemplate.MainForm()
            .OnSend(fun e ->
                async {
                    let! size =  Server.DoSomething e.Vars.Size.Value
                    let! active = Server.DoSomething e.Vars.Active.Value
                    let! family = Server.DoSomething e.Vars.Family.Value
                    let! fur = Server.DoSomething e.Vars.Fur.Value
                    let dog = (if size = "large" then (if active = "sporty" then (if family = "yes" then (if fur = "long" then "golden retriever" else if fur="short" then "boxer" else "Please choose fur type") 
                                                                                      else if family = "no" then (if fur = "long" then "alaskan malamute" else if fur="short" then "greyhound" else "Please choose fur type") else "Please choose if you want a family dog or not") 
                                                        else if active = "couch" then (if family = "yes" then (if fur = "long" then "newfoundland" else if fur = "short" then "great dane" else "Please choose fur type")
                                                                                         else if family = "no" then (if fur = "long" then "chow chow" else if fur = "short" then "bullmastiff (also a good family dog)" else "Please choose fur type") else "Please choose if you want a family dog or not") else "Please choose if you want a family dog or not") 
                               else if size = "small" then (if active = "sporty" then (if family = "yes" then (if fur = "long" then "papillon" else if fur = "short" then "beagle" else "Please choose fur type") 
                                                                                        else if family = "no" then (if fur = "long" then "jack russell terrier (has a longer fur version)" else if fur = "short" then "jack russell terrier" else "Please choose fur type") else "Please choose if you want a family dog or not") 
                                                             else if active = "couch" then (if family = "yes" then (if fur = "long" then "bichon frisé" else if fur = "short" then "dachshound" else "Please choose fur type") 
                                                                                              else if family = "no" then (if fur = "long" then "chihuahua" else if fur = "short" then "chihuahua" else "Please choose fur type")else "Please choose if you want a family dog or not")else "Please choose activeness") else "Please choose a size")
                    let! out = Server.Echo <| sprintf "%s" dog
                    doggo := out
                } |> Async.StartImmediate)
            .Dog(doggo.View)
            .Doc()

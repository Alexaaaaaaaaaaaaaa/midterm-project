namespace midterm_project

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.UI.Notation

type EndPoint =
    | [<EndPoint "/">] Quiz
    | [<EndPoint "/dogs">] Breeds

[<JavaScript>]
module Templates =

    type MainTemplate = Templating.Template<"wwwroot/index.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>


[<JavaScript>]
module Pages =
    let QuizPage () =
        let doggo = Var.Create ""
        Templates.MainTemplate.QuizPage()
            .OnSend(fun e ->
                async {
                    let size = "large"
                    let active = "sporty"
                    let family = "yes"
                    let fur = "long"
                    let dog = (if size = "large" then (if active = "sporty" then (if family = "yes" then (if fur = "long" then "golden retriever" else if fur="short" then "boxer" else "Please choose fur type") 
                                                                                      else if family = "no" then (if fur = "long" then "alaskan malamute" else if fur="short" then "greyhound" else "Please choose fur type") else "Please choose if you want a family dog or not") 
                                                        else if active = "couch" then (if family = "yes" then (if fur = "long" then "newfoundland" else if fur = "short" then "great dane" else "Please choose fur type")
                                                                                         else if family = "no" then (if fur = "long" then "chow chow" else if fur = "short" then "bullmastiff (also a good family dog)" else "Please choose fur type") else "Please choose if you want a family dog or not") else "Please choose if you want a family dog or not") 
                               else if size = "small" then (if active = "sporty" then (if family = "yes" then (if fur = "long" then "papillon" else if fur = "short" then "beagle" else "Please choose fur type") 
                                                                                        else if family = "no" then (if fur = "long" then "jack russell terrier (has a longer fur version)" else if fur = "short" then "jack russell terrier" else "Please choose fur type") else "Please choose if you want a family dog or not") 
                                                             else if active = "couch" then (if family = "yes" then (if fur = "long" then "bichon frisé" else if fur = "short" then "dachshound" else "Please choose fur type") 
                                                                                              else if family = "no" then (if fur = "long" then "chihuahua" else if fur = "short" then "chihuahua" else "Please choose fur type")else "Please choose if you want a family dog or not")else "Please choose activeness") else "Please choose a size")
                    doggo := sprintf "%s" dog
                } |> Async.StartImmediate)
            .Dog(doggo.View)
            .Doc()

    let BreedsPage () =
        Templates.MainTemplate.BreedsPage()
            .Doc()

[<JavaScript>]
module App =
    open WebSharper.Sitelets
    open WebSharper.UI.Html

    // Create a router for our endpoints
    let router = Router.Infer<EndPoint>()
    // Install our client-side router and track the current page
    let currentPage = Router.InstallHash EndPoint.Quiz router

    type Router<'T when 'T: equality> with
        member this.LinkHash (ep: 'T) = "#" + this.Link ep

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Router<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
            let isActive = if endpoint = act then "nav-link active" else "nav-link"
            li [attr.``class`` "nav-item"] [
                a [
                    attr.``class`` isActive
                    attr.href (ctx.LinkHash act)
                ] [text txt]
            ]
        [
            "Quiz" => EndPoint.Quiz
            "Breeds" => EndPoint.Breeds
        ]

    [<SPAEntryPoint>]
    let Main () =
        let renderInnerPage (currentPage: Var<EndPoint>) =
            currentPage.View.Map (fun endpoint ->
                match endpoint with
                | Quiz      -> Pages.QuizPage()
                | Breeds    -> Pages.BreedsPage()
            )
            |> Doc.EmbedView
       
        Templates.MainTemplate()
            .MenuBar(MenuBar router EndPoint.Quiz)
            .Body(renderInnerPage currentPage)
            .Bind()

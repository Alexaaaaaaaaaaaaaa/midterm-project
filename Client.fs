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
                    // Grab user choices
                    let! size = e.Vars.Size
                    let! active = e.Vars.Active
                    let! family = e.Vars.Family
                    let! fur = e.Vars.Fur
                    // Compute dog based on selection
                    let dog =
                        match size, active, family, fur with
                        // Sanity checks for unset/invalid values.
                        | size, _, _, _ when size <> "large" && size <> "small" ->
                            "Please choose size"
                        | _, act, _, _ when act <> "sporty" && act <> "couch" ->
                            "Please choose activeness"
                        | _, _, fam, _ when fam <> "yes" && fam <> "no" ->
                            "Please choose if you want a family dog or not"
                        | _, _, _, fur when fur <> "long" && fur <> "short" ->
                            "Please choose fur type"
                        // Compute dog
                        | "large", "sporty", "yes", "long" ->
                            "golden retriever"
                        | "large", "sporty", "yes", "short" ->
                            "boxer"
                        | "large", "sporty", "no", "long" ->
                           "alaskan malamute"
                        | "large", "sporty", "no", "short" ->
                            "greyhound"
                        | "large", "couch", "yes", "long" ->
                            "newfoundland"
                        | "large", "couch", "yes", "short" ->
                            "great dane"
                        | "large", "couch", "no", "long"
                        | "large", "couch", "no", "short" ->
                            "bullmastiff (also a good family dog)"
                        | "small", "sporty", "yes", "long" ->
                            "papillon"
                        | "small", "sporty", "yes", "short" ->
                            "beagle"
                        | "small", "sporty", "no", "long" ->
                            "jack russell terrier (has a longer fur version)"
                        | "small", "sporty", "no", "short" ->
                            "jack russell terrier"
                        | "small", "couch", "yes", "long" ->
                            "bichon frisé"
                        | "small", "couch", "yes", "short" ->
                            "dachshound"
                        | "small", "couch", "no", "long" ->
                            "chihuahua" // CHECK
                        | "small", "couch", "no", "short" ->
                            "chihuahua"
                        | _ ->
                            "Not sure" // Fallback, should never happen
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

namespace midterm_project

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/dogs">] Breeds

module Templating =
    open WebSharper.UI.Html

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
            let isActive = if endpoint = act then "nav-link active" else "nav-link"
            li [attr.``class`` "nav-item"] [
                a [
                    attr.``class`` isActive
                    attr.href (ctx.Link act)
                ] [text txt]
            ]
        [
            "Quiz" => EndPoint.Home
            "Breeds" => EndPoint.Breeds
        ]

    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates.MainTemplate()
                .Title(title)
                .MenuBar(MenuBar ctx action)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Html

    open type WebSharper.UI.ClientServer

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Quiz" [
            h1 [] [text "Take this test to see which dog is for you!"]
            div [] [client (Client.Main())]
        ]

    let BreedPage ctx =
        Templating.Main ctx EndPoint.Breeds "Dog Breeds" [
            h1 [] [text "Dog breeds"]
            h3 [] [text "This is a list of the dog breeds used in this quiz."]
            p [] [text "Golden retriever, boxer, alaskan malamute, greyhound, chow chow, bullmastiff, papillon, beagle, newfoundland, great dane, jack russell terrier, bichon frise, dachshound, chihuahua"]
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.Breeds -> BreedPage ctx
        )


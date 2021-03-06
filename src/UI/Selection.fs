namespace Percyqaz.Flux.UI

open Percyqaz.Flux.Input

[<AbstractClass>]
type ISelection(nt: NodeType) =

    member this.NodeType = nt
    abstract member FocusTree : ISelection list
    
    abstract member Select : unit -> unit
    abstract member Focus : unit -> unit

    abstract member OnFocus : unit -> unit
    abstract member OnUnfocus : unit -> unit
    abstract member OnSelected : unit -> unit
    abstract member OnDeselected : unit -> unit

and [<RequireQualifiedAccess>] NodeType =
    | None
    | Leaf
    | Switch of (unit -> ISelection)
    member this._IsLeaf =
        match this with
        | Leaf -> true
        | _ -> false
    member this._IsNone =
        match this with
        | None -> true
        | _ -> false

module Selection =

    let mutable private focusTree : ISelection list = []
    let mutable private selected : bool = false

    let private diff (xs: ISelection list) (ys: ISelection list) =
        let mutable xs = xs
        let mutable ys = ys
        let mutable difference_found = false
        while not (difference_found || xs.IsEmpty || ys.IsEmpty) do
            if xs.Head = ys.Head then
                xs <- List.tail xs
                ys <- List.tail ys
            else
                difference_found <- true
        for x in xs do x.OnUnfocus()
        for y in ys do y.OnFocus()

    let focus_tree(tree: ISelection list) =

        if selected then
            match List.tryHead focusTree with
            | Some w -> w.OnDeselected()
            | None -> ()
            selected <- false

        diff (List.rev focusTree) (List.rev tree)
        focusTree <- tree
    
    let select_tree(tree: ISelection list) =

        if tree <> focusTree && selected then
            match List.tryHead focusTree with
            | Some w -> w.OnDeselected()
            | None -> ()

        diff (List.rev focusTree) (List.rev tree)
            
        if tree <> focusTree || not selected then
            match List.tryHead tree with
            | Some w -> w.OnSelected()
            | None -> ()
            
        selected <- true
        focusTree <- tree

    let focus(item: ISelection) =
        match item.NodeType with
        | NodeType.None -> focus_tree []
        | NodeType.Leaf -> focus_tree item.FocusTree
        | NodeType.Switch f -> focus_tree (f().FocusTree)

    let select(item: ISelection) =
        match item.NodeType with
        | NodeType.None -> select_tree []
        | NodeType.Leaf -> select_tree item.FocusTree
        | NodeType.Switch f -> select_tree (f().FocusTree)

    let rec up() =
        if selected then
            focus_tree focusTree
        else
            match focusTree with
            | [] -> ()
            | head :: rest ->
                head.OnUnfocus()
                focusTree <- rest

                match List.tryHead rest with
                | Some h ->
                    match h.NodeType with
                    | NodeType.None -> up()
                    | NodeType.Leaf -> ()
                    | NodeType.Switch _ -> up()
                | None -> ()

type DragAndDropEvent<'T> =
    {
        Origin: float32 * float32
        Target: float32 * float32
        Payload: 'T
    }

type IDropzone<'T> =
    abstract member Drop : DragAndDropEvent<'T> -> unit

type DragAndDropAction<'T>(payload: 'T) =
    let origin = Mouse.pos()

    let mutable _acceptor : IDropzone<'T> option = None

    member this.Accept(acceptor: IDropzone<'T>) =
        _acceptor <- Some acceptor

    member this.Unaccept(acceptor: IDropzone<'T>) =
        if _acceptor.IsSome && _acceptor.Value = acceptor then _acceptor <- None

    member this.Drop() =
        match _acceptor with
        | Some a ->
            let target = Mouse.pos() 
            a.Drop { Origin = origin; Target = target; Payload = payload }
        | None -> ()

module DragAndDrop =
    
    let mutable private ref : DragAndDropAction<obj> option = None
    let mutable private ty : System.Type = null

    let drag(payload: 'T) =
        match ref with
        | Some r -> ()
        | None -> 
            let action = DragAndDropAction payload
            ref <- Some(unbox action : DragAndDropAction<obj>)
            ty <- typeof<'T>

    let drop() =
        match ref with
        | Some r -> r.Drop()
        | None -> ()

    let accept<'T>(zone: IDropzone<'T>) =
        match ref with
        | Some r when typeof<'T> = ty -> (unbox r : DragAndDropAction<'T>).Accept(zone)
        | _ -> ()

    let unaccept<'T>(zone: IDropzone<'T>) =
        match ref with
        | Some r when typeof<'T> = ty -> (unbox r : DragAndDropAction<'T>).Unaccept(zone)
        | _ -> ()

    let current<'T>() =
        match ref with
        | Some r when typeof<'T> = ty -> Some(unbox r : DragAndDropAction<'T>)
        | _ -> None
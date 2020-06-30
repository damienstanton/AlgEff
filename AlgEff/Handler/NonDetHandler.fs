﻿namespace AlgEff.Handler

open AlgEff.Effect

type PickTrue<'ctx, 'ret when 'ctx :> NonDetContext and 'ctx :> ConcreteContext<'ret>>(context : 'ctx) =
    inherit SimpleHandler<'ctx, 'ret, Unit, Unit>()

    override __.Start = Unit

    override this.TryStep<'stx>(Unit, effect, cont) =

        let step Unit (nonDetEff : NonDetEffect<_>) cont =
            match nonDetEff.Case with
                | Decide eff ->
                    let next = eff.Cont(true)
                    cont Unit next

        this.Adapt<_, 'stx> step Unit effect cont

    override __.Finish(Unit) = Unit

type PickMax<'ctx, 'ret when 'ctx :> NonDetContext and 'ctx :> ConcreteContext<'ret> and 'ret : comparison>(context : 'ctx) =
    inherit SimpleHandler<'ctx, 'ret, Unit, Unit>()

    override __.Start = Unit

    override this.TryStep(_, effect, cont) =

        let step Unit (nonDetEff : NonDetEffect<_>) cont =
            match nonDetEff.Case with
                | Decide eff ->
                    let stxT, resT = eff.Cont(true) |> cont Unit
                    let stxF, resF = eff.Cont(false) |> cont Unit
                    if resT > resF then
                        stxT, resT
                    else
                        stxF, resF

        this.Adapt<_, 'stx> step Unit effect cont

    override __.Finish(Unit) = Unit

type PickAll<'ctx, 'ret when 'ctx :> NonDetContext and 'ctx :> ConcreteContext<'ret>>(context : 'ctx) =
    inherit Handler<'ctx, 'ret, List<'ret>, Unit, Unit>()

    override __.Start = Unit

    override this.TryStep<'stx>(_, effect, cont) =

        let step Unit (nonDetEff : NonDetEffect<_>) (cont : HandlerCont<'ctx, 'ret, List<'ret>, Unit, 'stx>) : ('stx * List<'ret>) =
            match nonDetEff.Case with
                | Decide eff ->
                    let stxT, resT = eff.Cont(true) |> cont Unit
                    let stxF, resF = eff.Cont(false) |> cont Unit
                    Unit, List.append resT resF

        this.Adapt<_, 'stx> step Unit effect cont

    override __.Finish(Unit) = Unit

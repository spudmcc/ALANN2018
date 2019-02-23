﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module ProcessGoal


open Types
open NodeFunctions
open TermUtils
open TermFormatters
open Events

let (|Selective|NonSelective|) t = if isSelective t then Selective else NonSelective

let isPredictiveOp = function | PreImp | ConImp -> true | _ -> false

// find max choice (unifies ? =/> G)
let postCondition = function | Term(op, [_; postCondition]) when op |> isPredictiveOp -> postCondition | term -> term

let satisfyingBelief state (event : Event) =
    let matches = 
        state.Beliefs.GetBeliefs()
        |> Seq.filter (fun b -> (postCondition b.Term) = event.Term)

    if Seq.isEmpty matches then
        None
    else
        Some(Seq.maxBy (fun (b : Belief) -> exp b.TV / (float32(b.Stamp.SC))) matches)

let processGoal attention state (event : Event) =
    match Choice.bestAnswer state event with
    | Some belief when state.Term = event.Term ->
        if exp(event.TV.Value) > Params.DECISION_THRESHOLD then
            match event.Term with
            | Term(Inh, [Term(IntSet, [Word "left"]); Word "action"]) ->
                raiseActionExecutionEvent (Actions.MoveLeft)
            | Term(Inh, [Term(IntSet, [Word "right"]); Word "action"]) ->
                raiseActionExecutionEvent (Actions.MoveRight)
            | _ -> ()

            raiseDisplaySolutionEvent (sprintf "Executing solution for %s!" (ft event.Term))
        [makeAnsweredEventGoal attention event belief] 
    | _ -> getInferenceBeliefs attention state event

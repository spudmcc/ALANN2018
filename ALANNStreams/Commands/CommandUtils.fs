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

module CommandUtils

open Types
open ALANNSystem
open TermFormatters

let getNodeFromTerm term =
    let numStreams = Params.NUM_TERM_STREAMS 
    let hashRoute numStreams t = abs(t.GetHashCode() % numStreams )
    let i = hashRoute numStreams term

    stores.[i].TryGetValue(term)

let getNodeCount() =
    let rec loop acc n =
        match n with
        | 0 -> acc + stores.[n].Count
        | _ -> loop (acc + stores.[n].Count) (n - 1)

    loop 0 (stores.GetLength(0) - 1)

let printCommandWithString str str2 =
     myprintfn (sprintf "%sCOMMAND: %s '%s'" Params.COMMAND_PREFIX str str2)

let printCommand str =
     myprintfn (sprintf "%sCOMMAND: %s" Params.COMMAND_PREFIX str)

let printBeliefStr str =
    myprintfn (sprintf "%s%s" Params.BELIEF_PREFIX str)

let showBeliefs beliefs =
    beliefs
    |> List.iter (fun b -> printBeliefStr (formatBelief b))
    match beliefs with
    | [] -> ()
    | _ ->
        beliefs
        |> List.averageBy (fun b -> b.TV.C)
        |> (fun avg -> printBeliefStr (sprintf "AVERAGE CONF = %f" avg))
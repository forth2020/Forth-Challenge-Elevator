\ ----------------------------------------------------------------------
\ @file : conditional_compilation.fs
\ ----------------------------------------------------------------------
\
\ Last change: KS 10.09.2023 12:03:32
\ @author: Klaus Schleisiek
\ @copyright: public domain
\
\ Traditionally, string comparison has been used to process [IF].
\ This version uses FIND instead.
\ Multiline comment \* ...
\                      ... *\ has been added, because it is trivial.
\ Conditional clauses may be commented out using (, \, or \*
\ ----------------------------------------------------------------------

Defer [ELSE]  ( -- )  immediate

: [IF]        ( flag -- )   ?EXIT postpone [ELSE] ; immediate
: [THEN]      ( -- )        ; immediate
: [NOTIF]     ( flag -- )   0= postpone [IF] ; immediate
: [IFDEF]     ( <name> -- ) postpone [DEFINED]  postpone [IF] ; immediate
: [IFUNDEF]   ( <name> -- ) postpone [DEFINED]  postpone [NOTIF] ; immediate

\ ----------------------------------------------------------------------
\ NEXT-WORD returns the xt of a word in the search order.
\ Words, which are not found, will be skipped.
\ 0 will be returned when the end of file is reached.
\ ----------------------------------------------------------------------
: next-word  ( -- xt | 0 )
   BEGIN BEGIN  BL word  dup c@ WHILE  find IF EXIT THEN drop  REPEAT  drop
         refill 0=
   UNTIL 0
;
: *\   ( -- )  ; immediate \ end of multi-line comment
: \*   ( -- )  BEGIN  next-word   dup 0= swap   ['] *\ =   or UNTIL ; immediate

Variable Nestlevel   0 Nestlevel !  \ nesting level counter

: nest    ( -- )  1 Nestlevel +! ;
: unnest  ( -- )  Nestlevel @ 1 - 0 max Nestlevel ! ; \ don't decrement below zero

: [if]-decode  ( xt -- flag )
   ['] [IF]      case? IF  nest            false  EXIT THEN
   ['] [NOTIF]   case? IF  nest            false  EXIT THEN
   ['] [IFDEF]   case? IF  nest            false  EXIT THEN
   ['] [IFUNDEF] case? IF  nest            false  EXIT THEN
   ['] [ELSE]    case? IF  Nestlevel @ 0=         EXIT THEN
   ['] [THEN]    case? IF  Nestlevel @ 0= unnest  EXIT THEN
   ['] \         case? IF  postpone \      false  EXIT THEN  \ needed to be able to e.g. comment out [THEN]
   ['] (         case? IF  postpone (      false  EXIT THEN  \ needed to be able to e.g. comment out [THEN]
   ['] \*        case? IF  postpone \*     false  EXIT THEN  \ needed to be able to e.g. comment out [THEN]
   0= abort" [THEN] missing"                                 \ end-of-file reached?
   \ all oter xt's are ignored
   false
;
:noname  ( -- )  BEGIN  next-word [if]-decode UNTIL ; IS [ELSE]


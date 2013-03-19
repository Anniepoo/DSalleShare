:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_parameters)).

:- ensure_loaded(panel).

start :-
        http_server(http_dispatch, [port(6788)]).

:- http_handler('/TDV', send_data, []).

:- dynamic silo_data/1.

silo_data(tdv_control('0,0,0,0,0,0')).

send_data(_Request) :-
	silo_data(tdv_control(Data)),
        format('Content-type: text/plain~n~n'),
        format('~w', [Data]).

:- http_handler('/TDVcontrol', set_data, []).

set_data(Request) :-
	http_parameters(Request,
			[
			 data(Data, [optional(true)])
			]),
	ground(Data),!,
	retractall(silo_data(tdv_control(_))),
	asserta(silo_data(tdv_control(Data))),
        format('Content-type: text/plain~n~n'),
        format('OK~n', []).
set_data(_) :-
        format('Content-type: text/plain~n~n'),
        format('NODATA~n', []).





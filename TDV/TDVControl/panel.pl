:- module(panel, []).
/** <module> Control panel web page

*/
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_parameters)).
:- use_module(library(http/html_write)).
:- use_module(library(csv)).

:- http_handler('/panel', panel_handler, []).

panel_handler(Request) :-
	http_parameters(Request,
			[
			 data(Data), [default('0,0,0,0,0,0')]
			]),
	atom_codes(Data, CData),
	phrase(csv(LData), CData),
	reply_html_page(
	    mobile,
	    title('TDV'),
	    \sex_changing_crazo_buttons(LData)).

sex_changing_crazo_buttons(LData) -->
	html([
	    h1('sex changing crazo_buttons'),
	    table([
		\row_of_avs(0, LData),
		\row_of_avs(1, LData),
		\row_of_avs(2, LData),
		\row_of_avs(3, LData),
		\row_of_avs(4, LData),
		\row_of_avs(5, LData)
		  ])
	     ]).


row_of_avs(Row, LData) -->
	html([
	    tr([
		\av_button(Row, 0, LData),
		\av_button(Row, 1, LData),
		\av_button(Row, 2, LData),
		\av_button(Row, 3, LData),
		\av_button(Row, 4, LData),
		\av_button(Row, 5, LData)
	       ])]).

av_button(Row, Column, LData) -->
	{
	     replace(Column , LData, Row, NewData)

	},
	html([
	    a(href=

(* ::Package:: *)

zero[f_,lo_,hi_,\[CurlyEpsilon]_]:=
Module[
{x=(lo+hi)/2,xlo=lo,xhi=hi},
If[f[hi]<=0||f[lo]>=0,Print["INVALID BISECTION"]];
While[Abs[f[x]]>\[CurlyEpsilon],
If[f[x]>0,
xhi=x,
xlo=x
];x=(xlo+xhi)/2
];x
];
fixedSteps[\[Alpha]_,p_,lo_,hi_,N_,heuristic_]:=
Union[
Module[
{xs={lo}},
Table[
AppendTo[xs,Last[xs]+\[Alpha] heuristic[p,Last[xs]]],
{i,1,N}
];
xs
]
];
steps[p_,lo_,hi_,N_,heuristic_:(Abs[#1[#2]/Derivative[4][#1][#2]]^(1/4)&)]:=
fixedSteps[
zero[Last[fixedSteps[#,p,lo,hi,N,heuristic]]-hi&,.00001,100,(hi-lo)1*^-12],
p,lo,hi,N,heuristic];
Needs["FunctionApproximations`"];
Quiet[Needs["Combinatorica`"]];
SymPart[l_List,spec_?NumberQ]:=l[[spec]];
SymBS[l_List?(NumberQ[Total[#]]&),k_]:=BinarySearch[l,k]
approximation[xs_,ys_,outTangents_,inTangents_]:=Function[
h,
With[
{i=Floor[SymBS[xs,h]]},
With[
{t=If[i<Length[xs],(h-SymPart[xs,i])/(SymPart[xs,i+1]-SymPart[xs,i]),0],
\[CapitalDelta]=SymPart[xs,i+1]-SymPart[xs,i]},
(2t^3-3t^2+1)SymPart[ys,i]+
\[CapitalDelta](t^3-2t^2+t)SymPart[outTangents,i]+
If[i<Length[ys],
(-2 t^3+3t^2)SymPart[ys,i+1]+
\[CapitalDelta](t^3-t^2)SymPart[inTangents,i+1],
0
]
]
]
];
bestContinuousInterpolatingPiecewiseCubic[f_,x_,h_,\[CurlyEpsilon]_]:=Table[Expand[MiniMaxApproximation[(f[h]-(f[x[[i]]]+((f[x[[i+1]]]-f[x[[i]]]) (h-x[[i]]))/(x[[i+1]]-x[[i]])))/((h-x[[i]]) (h-x[[i+1]])),{h,{x[[i]]+\[CurlyEpsilon] (x[[i+1]]-x[[i]]),x[[i+1]]-\[CurlyEpsilon] (x[[i+1]]-x[[i]])},1,0}][[2,1]] (h-x[[i]]) (h-x[[i+1]])+(f[x[[i]]]+((f[x[[i+1]]]-f[x[[i]]]) (h-x[[i]]))/(x[[i+1]]-x[[i]]))],{i,1,Length[x]-1}
];
keyFrames[x_,y_,tgin_,tgout_]:=StringReplace[
ToString[
TableForm[
Map[
ToString[CForm[#]]&,
{ConstantArray["key =",Length[x]],
x,
y,
tgin,
tgout}\[Transpose],{2}
]
]
],{"\""->"","\n\n"->"\n"}]
